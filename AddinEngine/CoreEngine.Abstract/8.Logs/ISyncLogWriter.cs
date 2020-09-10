using System;

namespace CoreEngine.Abstract
{
    public interface ISyncLogWriter
    {
        void Write(string str);
        string Guid { get; set; }
        void Write(string jobName, string secName, string guidKey, string cmdName, string cmdString, string exception);
    }
}
