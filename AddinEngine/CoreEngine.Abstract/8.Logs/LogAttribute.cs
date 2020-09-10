using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using AddinEngine.Abstract;
using MethodBoundaryAspect.Fody.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace CoreEngine.Abstract
{
    public sealed class LogAttribute : OnMethodBoundaryAspect, IDisposable
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            //TODO: More on log

            // switch (args.Instance)
            // {
                // case ISyncSection section:
                // {
                //     if (args.Method.Name == "CreateProviders")
                //     {
                //         if (!(args.Arguments[1] is IServiceProvider serviceProvider))
                //         {
                //             throw new Exception("Wrong CreateProviders function in ISection");
                //         }
                //         //Only need to create the logwriter for this scope
                //         //LogWriter will Create DB Connection before CreateProviders to avoid using same transaction with providers
                //         //var guid = Guid.NewGuid();
                //         var guid = section.Guid;
                //         foreach (var writer in serviceProvider.GetServices<ISyncLogWriter>())
                //         {
                //             //Just to make it initialized for the scope
                //             writer.Guid = guid;
                //         }
                //     }
                //     break;
                // }
                // case ISyncDataSourceProvider provider:
                // {
                //     foreach (var logWriter in provider.ServicesProvider.GetServices<ISyncLogWriter>())
                //     {
                //         logWriter.Write("On Entry From ISyncDataSourceProvider");
                //         //var cmdString = args.Method.GetMethodBody().LocalVariables[0].ToString();
                //     }
                //     break;
                // }
            // }
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            if (args.ReturnValue is Task t)
                t.ContinueWith(task =>
                {
                    Console.WriteLine("On exit");

                    // if not success, OnException will work with
                    if (t.IsFaulted) return;

                    switch (args.Instance)
                    {
                        case ISyncCommand command:
                        {
                            foreach (var logWriter in command.ServicesProvider.GetServices<ISyncLogWriter>())
                            {
                                // var sectionName = command.Provider.Section.SectionConfig.Name;
                                // var jobName     = command.Provider.Section.JobName;
                                // var cmdName     = command.CommandConfig.Name;
                                // // Remember: the logWriter is Scope Variable -> this hold the guid assigned at start the scope (when section call CreateProvider
                                // // The LogAttribute is created each time a function with [Log] annotation is called -> can not let it handle guid, since it will be created new once
                                // var guidKey   = logWriter.Guid;
                                // var cmdString = command.CommandConfig.CommandString;
                                //
                                // logWriter.Write(jobName, sectionName, guidKey.ToString(), cmdName, cmdString, null);
                            }
                            break;
                        }
                    }
                });
        }

        public override void OnException(MethodExecutionArgs args)
        {
            var command = args.Instance as ISyncCommand;

            if (command == null) return;
            foreach (var logWriter in command.ServicesProvider.GetServices<ISyncLogWriter>())
            {
                // var sectionName = command.Provider.Section.SectionConfig.Name;
                // var jobName     = command.Provider.Section.JobName;
                // var cmdName     = command.CommandConfig.Name;
                // // Remember: the logWriter is Scope Variable -> this hold the guid assigned at start the scope (when section call CreateProvider
                // // The LogAttribute is created each time a function with [Log] annotation is called -> can not let it handle guid, since it will be created new once
                // var guidKey   = logWriter.Guid;
                // var cmdString = command.CommandConfig.CommandString;
                //
                // var exceptionDescription = command.DescribeException(args.Exception);
                //
                // Console.WriteLine("On Command Exception " + exceptionDescription);
                //
                // logWriter.Write(jobName, sectionName, guidKey.ToString(), cmdName, cmdString, exceptionDescription);
            }
        }

        public void Dispose()
        {
        }
    }
}
