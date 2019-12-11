using GitWatch.Data.Auth;
using GitWatch.Data.Services;
using GitWatch.Domain;
using GitWatch.Domain.Repositories;
using GitWatch.Domain.Services;
using Grace.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitWatch.DependencyInjection
{
    public class GraceContainer : IContainer
    {
        private DependencyInjectionContainer _container;
        private readonly IConfiguration _configuration;

        internal GraceContainer(IConfiguration configuration)
        {
            _configuration = configuration;
            _container = ConfigureContainer(configuration);
        }

        private static DependencyInjectionContainer ConfigureContainer(IConfiguration configuration)
        {
            var container = new DependencyInjectionContainer();
            ConfigureContainer(container, configuration);
            return container;
        }

        public static void ConfigureContainer(IInjectionScope scope, IConfiguration configuration)
        {
            scope.Configure(c => c.ExportInstance(configuration).As<IConfiguration>());

            scope.Configure(c => c.Export<ProjectRepository>().As<IProjectRepository>());
            scope.Configure(c => c.Export<ProjectService>().As<IProjectService>());

        }

        public T Get<T>()
        {
            return _container.Locate<T>();
        }

        public void Replace<I, T>()
        {
            _container.Dispose();
            _container = ConfigureContainer(_configuration);
            _container.Configure(c => c.Export<T>().As<I>());
        }

        public void Replace<T>(T instance)
        {
            _container.Dispose();
            _container = ConfigureContainer(_configuration);
            _container.Configure(c => c.ExportInstance(instance).As<T>());
        }

        public object GetService(Type serviceType)
        {
            return _container.Locate(serviceType);
        }
    }
}
