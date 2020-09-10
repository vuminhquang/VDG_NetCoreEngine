using System;

namespace CoreEngine.Abstract
{
    public abstract class SyncLogWriterBase : ISyncLogWriter
    {
        protected IServiceProvider _services;
        public string Guid { get; set; }

        protected SyncLogWriterBase(IServiceProvider services)
        {
            _services = services;
        }

        public virtual void Write(string str)
        {
            Console.WriteLine($"{Guid} {str}");
        }

        public abstract void Write(string jobName, string sectionName, string guidKey, string cmdName, string cmdString, string exception);
    }
}
