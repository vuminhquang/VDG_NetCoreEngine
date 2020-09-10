using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddinEngine.Abstract
{
    public interface ISyncCommand
    {
        Dictionary<string, object> ExecuteParameters { get; }
        IServiceProvider           ServicesProvider  {get;}
        ICommandProvider           Provider          { get; }

        void PrepareParameters(IDictionary<string, object> SectionObjects);
        Task<object> Execute();
        string DescribeException(Exception exception);
    }
}