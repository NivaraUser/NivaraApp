using Autofac;
using Nivara.Core.CompanyDetail;
using Nivara.Core.CompanyRole;

namespace Nivara.Core.Implementation.Modules
{
    public class BehaviourModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ICompaniesServices>().As<CompaniesServices>().InstancePerLifetimeScope();
            builder.RegisterType<ICompanyRolesServices>().As<CompanyRolesServices>().InstancePerLifetimeScope();
        }
    }
}
