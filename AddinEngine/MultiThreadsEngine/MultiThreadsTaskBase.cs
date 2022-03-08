using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreadsEngine
{
    public enum Status
    {
        Running,
        Paused
    }

    /// <summary>
    ///     The center of engine
    /// </summary>
    /// <typeparam name="TJob">Any type can use to describe the Jobs</typeparam>
    public abstract class MultiThreadsTaskBase<TJob> : ICancellableTask, IDisposable
    {
        private readonly int _maxThreads;
        // private readonly ManualResetEventSlim _resume = new ManualResetEventSlim();
        private bool _stopping;
        private Task _currMainTask = null;
        private CancellationToken _stoppingToken;
        private readonly object @lock = new object();
        private readonly ObservableConcurrentQueue<TJob> _jobQueue;

        protected MultiThreadsTaskBase(int maxThreads = 8)
        {
            _maxThreads = maxThreads;
            _jobQueue = new ObservableConcurrentQueue<TJob>();
            _jobQueue.ContentChanged += OnResumeEvent;
            ProcessStatus = Status.Paused;
        }

        public void AddJob(TJob job)
        {
            _jobQueue.Enqueue(job);
        }
        
        public void SetCancellationToken(CancellationToken stoppingToken)
        {
            _stoppingToken = stoppingToken;
        }
        
        // public static ConcurrentBag<Task> Tasks = new ConcurrentBag<Task>();
        private Status ProcessStatus { get; set; }

        public string Id { get; set; }

        public void Cancel()
        {
            ProcessStatus = Status.Paused;
            _stopping = true;
        }

        public void Dispose()
        {
            _currMainTask?.Wait(_stoppingToken);
            // _resume?.Dispose();
        }

        private void OnResumeEvent(object sender, NotifyConcurrentQueueChangedEventArgs<TJob> args)
        {
            if (_stopping) return;
            
            Console.WriteLine($"Receive Queue Changed Event in MultiThread Task {args.Action}");
            if (args.Action == NotifyConcurrentQueueChangedAction.Enqueue)
            {
                _currMainTask = Run(_stoppingToken);
            }
        }

        private Task Run(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                if (ProcessStatus == Status.Running)
                    return;
                lock (@lock)
                {
                    if (ProcessStatus == Status.Paused)
                    {
                        Resume();
                    }
                    else
                    {
                        return;
                    }
                }

                try
                {
                    await Process(Task.CurrentId.GetValueOrDefault(), stoppingToken);
                }
                finally
                {
                    lock (@lock)
                    {
                        Pause();
                    }
                }
            }, stoppingToken);
        }

        private async Task Process(int mainTaskNum, CancellationToken stoppingToken)
        {
            using var throttler = new SemaphoreSlim(_maxThreads);
            var allTasks = new List<Task>();
            while (!_stopping && _jobQueue.TryDequeue(out var item))
            {
                if (StoppingTokenIsCancellationRequested(stoppingToken))
                {
                    Console.WriteLine("Task {0} cancelled", mainTaskNum);
                    break;
                }
                
                var t = Task.Run(async () =>
                {
                    try
                    {
                        var taskNum = Task.CurrentId;
                        await throttler.WaitAsync(stoppingToken);
                        await ExecuteTask(item, taskNum, stoppingToken);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error on run task {Task.CurrentId} with job {item}, {e}");
                    }
                    finally
                    {
                        Console.WriteLine($"Release task with job {item}");
                        throttler.Release();
                    }
                }, stoppingToken);
                allTasks.Add(t);
            }

            await Task.WhenAll(allTasks);
        }

        protected abstract Task ExecuteTask(TJob job, int? taskNum, CancellationToken stoppingToken);

        private void Pause()
        {
            ProcessStatus = Status.Paused;
        }

        private void Resume()
        {
            ProcessStatus = Status.Running;
        }
        
        protected bool StoppingTokenIsCancellationRequested(CancellationToken stoppingToken)
        {
            return stoppingToken.IsCancellationRequested || _stopping;
        }
    }
}