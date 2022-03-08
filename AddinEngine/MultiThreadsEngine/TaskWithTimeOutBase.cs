using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreadsEngine
{
    public abstract class TaskWithTimeOutBase<TContext>
    {
        protected int _timeSlot;

        /// <summary>
        /// </summary>
        /// <param name="timeSlot">How much time (in milliseconds) the Task can be run before cancellationToken Triggerred</param>
        protected TaskWithTimeOutBase(int timeSlot)
        {
            _timeSlot = timeSlot;
        }

        public Task Run(TContext context, CancellationToken cancellationToken)
        {
            var cts = new CancellationTokenSource();
            var internalCancellationToken = cts.Token;

            try
            {
                var t = ExecuteTask(context, internalCancellationToken);
                var doneTask = Task.WhenAny(t, Task.Delay(_timeSlot, cancellationToken));

                //Call Result instead of using await, means we block this thread, BUT, since the thread is ONLY use to watch for the job to be doing inside timeSlot
                //Then, synchronous code is OK, no need to spend more resource for async here
                if (doneTask.Result != t) //If out off time slot first, then we cancel t
                {
                    Console.WriteLine("Time Slot reached, send cancel signal");
                    cts.Cancel();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                cts.Dispose();
            }

            return Task.CompletedTask;
        }

        // Run Task with no CancelationToken from outside
        public Task Run(TContext context)
        {
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            try
            {
                var t = ExecuteTask(context, cancellationToken);
                var doneTask = Task.WhenAny(t, Task.Delay(_timeSlot));
                if (doneTask.Result != t) //If out off time slot first, then we cancel t
                {
                    Console.WriteLine("Time Slot reached, send cancel signal");
                    cts.Cancel();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                cts.Dispose();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///     If the executing is more than time slot, cancellationToken should be triggerred then
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken">Always check for cancellation, such as:  while (!token.IsCancellationRequested)</param>
        /// <returns></returns>
        protected abstract Task ExecuteTask(TContext context, CancellationToken cancellationToken);
    }
}