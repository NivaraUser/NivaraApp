using Autofac;
using Nivara.Core.Implementation.Modules;
using Nivara.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Nivara.Web.Implementation
{
    public static class Bootstrapper
    {
        /// <summary>Integrates container with web portal</summary>
        /// <returns>Container instance</returns>
        public static IContainer IntegrateContainer()
        {
            var builder = new ContainerBuilder();

            RegisterControllers(builder);
            RegisterModules(builder);

            IContainer container = builder.Build();

            return container;
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(BehaviourModule).Assembly)
            .AsImplementedInterfaces().InstancePerLifetimeScope()
            .WithParameters(ExtensionParameters.ResolvedParameters());
        }

        private static void RegisterControllers(ContainerBuilder builder)
        {
            //builder.RegisterControllers(Assembly.GetExecutingAssembly());
        }
    }
}
