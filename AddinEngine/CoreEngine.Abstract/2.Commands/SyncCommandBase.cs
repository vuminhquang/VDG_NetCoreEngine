using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreEngine.Abstract;

namespace AddinEngine.Abstract
{
    public abstract class SyncCommandBase : ISyncCommand
    {
        protected readonly IServiceProvider _services;
        public ICommandProvider Provider { get; }

        private SyncCommandBase(){}

        protected SyncCommandBase(ICommandProvider provider, IServiceProvider services)
        {
            Provider = provider;

            _services = services;
            ExecuteParameters = new Dictionary<string, object>();
        }

        [Log]
        public async Task<object> Execute()
        {
            return await DoExecute();
        }

        public virtual string DescribeException(Exception exception)
        {
            return exception.ToString();
        }

        protected abstract Task<object> DoExecute();

        public abstract void PrepareParameters(IDictionary<string, object> SectionObjects);

        public virtual Dictionary<string, object> ExecuteParameters { get; }

        public IServiceProvider ServicesProvider => _services;

    }
}
