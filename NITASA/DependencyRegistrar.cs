using Autofac;
using Autofac.Builder;
using Autofac.Integration.Mvc;
using NITASA.Core.Infrastructure;
using NITASA.Core.Infrastructure.DependencyManagement;
using NITASA.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NITASA
{
    public class DependencyRegistrar: IDependencyRegistrar
    {

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            string dbName = ConfigurationManager.ConnectionStrings["NITASAConnection"].ConnectionString;

            builder.RegisterType<NTSDBContext>().As<NTSDBContext>().WithParameter("name", dbName).InstancePerLifetimeScope();
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());
            //builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
        }

        public int Order { get; set; }
    }
}