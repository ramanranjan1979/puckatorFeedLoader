using PuckatorFeedCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace puckatorFeedLoader
{
    internal static class ConfigureService
    {
        internal static void Configure()
        {
            HostFactory.Run(configure =>
            {
                configure.Service<PuckatorFeedCreationService>(service =>
                {
                    service.ConstructUsing(s => new PuckatorFeedCreationService());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                //Setup Account that window service use to run.  
                configure.RunAsLocalSystem();
                configure.SetServiceName("Modizson.PuckatorFeedCreation.Service");
                configure.SetDisplayName("Modizson.PuckatorFeedCreation.Service");
                configure.SetDescription("Modizson.PuckatorFeedCreation.Service Creates Feed Files");
            });
        }
    }
    
    
}
