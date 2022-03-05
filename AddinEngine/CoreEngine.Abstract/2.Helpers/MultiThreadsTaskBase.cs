using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AddinEngine.Abstract
{
    public enum Status
    {
        Running,
        Paused
    }

    /// <summary>
    /// The center of engine
    /// </summary>
    /// <typeparam name="Job"></typeparam>
    public abstract class MultiThreadsTaskBase<Job> : ICancellableTask, IDisposable
    {
        // public static ConcurrentBag<Task> Tasks = new ConcurrentBag<Task>();
        public Status ProcesStatus { get; protected set; }

        private int _maxThreads;
        private readonly ManualResetEventSlim _resume = new ManualResetEventSlim();
        public ObservableConcurrentQueue<Job> JobQueue { get; }
        private List<Task> allTasks;
        private bool _stopping = false;


        protected MultiThreadsTaskBase(int maxThreads = 8)
        {
            _maxThreads             =  maxThreads;
            JobQueue                =  new ObservableConcurrentQueue<Job>();
            JobQueue.ContentChanged += OnResumeEvent;
            ProcesStatus = Status.Paused;
        }

        private void OnResumeEvent(object sender, ObservableConcurrentQueue<Job>.NotifyConcurrentQueueChangedEventArgs<Job> args)
        {
            if (args.Action == ObservableConcurrentQueue<Job>.NotifyConcurrentQueueChangedAction.Enqueue)
            {
                Resume();
            }
        }

        public Task Run(CancellationToken stoppingToken)
        {
            return Task.Run(async () => await Process(Task.CurrentId.GetValueOrDefault(), stoppingToken), stoppingToken);
        }

        private async Task Process(int mainTaskNum, CancellationToken stoppingToken)
        {
            allTasks = new List<Task>();
            var throttler = new SemaphoreSlim(initialCount: _maxThreads);
            while (!_stopping)
            {
                if (JobQueue.Count == 0)
                {
                    Console.Write("Pause");
                    Pause(stoppingToken);
                }
                
                if (StoppingTokenIsCancellationRequested(stoppingToken))
                {
                    Console.WriteLine("Task {0} cancelled", mainTaskNum);
                    break;
                }

                Console.Write("Resume");

                if (JobQueue.Count > 0 || allTasks.Count > 0)
                {
                    try
                    {
                        await throttler.WaitAsync(stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        //when user cancel the operation
                        //break the loop instead of thrown exception 
                        break;
                    }

                    if (StoppingTokenIsCancellationRequested(stoppingToken))
                    {
                        //throttler.Release();
                        break;
                    }

                    if (allTasks.Count > _maxThreads)
                    {
                        allTasks.Remove(await Task.WhenAny(allTasks));
                    }
                    

                    // if (PagesQueue.Count > 0)
                    if (JobQueue.TryDequeue(out var item))
                    {
                        var t = Task.Run(async () =>
                        {
                            try
                            {
                                await ExecuteTask(item, mainTaskNum, stoppingToken);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine($"Error on run task {Thread.CurrentThread.ManagedThreadId}, {e}");
                            }
                            finally
                            {
                                Console.WriteLine($"Release task {Thread.CurrentThread.ManagedThreadId}");
                                throttler.Release();
                            }
                        }, stoppingToken);
                        allTasks.Add(t);
                        // Tasks.Add(t);//This will not remove any task once add into. When close the program, this will be cleaned once
                    }
                    // else
                    // {
                    //     //when there are no task waiting in the queue, means the job is done
                    //     break;
                    // }
                }

                await Task.Delay(5, stoppingToken);
            }

            await Stop();
            if (_maxThreads - throttler.CurrentCount > 0)
            {
                throttler.Release(_maxThreads - throttler.CurrentCount);
            }
        }

        protected abstract Task ExecuteTask(Job job, int mainTaskNum, CancellationToken stoppingToken);

        private void Pause(CancellationToken stoppingToken)
        {
            ProcesStatus = Status.Paused;
            _resume.Wait(stoppingToken);
            _resume.Reset();
        }

        private void Resume()
        {
            ProcesStatus = Status.Running;
            _resume.Set();
            _stopping = false;
        }

        private async Task Stop()
        {
            await Task.WhenAll(allTasks);
            ProcesStatus = Status.Paused;
            _stopping = true;
        }

        public void Dispose()
        {
            _resume?.Dispose();
        }

        public string Id { get; set; }
        public void Cancel()
        {
            ProcesStatus = Status.Paused;
            _stopping = true;
            _resume.Set();
        }

        protected bool StoppingTokenIsCancellationRequested(CancellationToken stoppingToken)
        {
            return stoppingToken.IsCancellationRequested || _stopping;
        }
    }
}
