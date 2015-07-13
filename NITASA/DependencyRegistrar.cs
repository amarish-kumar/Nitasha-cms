using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;
using NITASA.Core.Infrastructure;
using NITASA.Core.Infrastructure.DependencyManagement;
using NITASA.Data;
using NITASA.Services.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using NITASA.Areas.Admin.Helper;
using NITASA.Services.Caching;

namespace NITASA
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            string dbName = ConfigurationManager.ConnectionStrings["NITASAConnection"].ConnectionString;

            builder.RegisterType<NTSDBContext>().As<NTSDBContext>().WithParameter("name", dbName).InstancePerLifetimeScope();

            builder.RegisterType<UserRoleCacheManager>().As<ICacheManager>().Named<ICacheManager>("cms_cache_static").SingleInstance();

            builder.RegisterType<AclService>().As<IAclService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("cms_cache_static"))
                .InstancePerLifetimeScope();
        }

        public int Order { get; set; }
    }
}