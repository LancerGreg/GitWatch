using GitWatch.Domain;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitWatch.DependencyInjection
{
    public static class ContainerFactory
    {
        public static IContainer GetContainer(IConfiguration configuration)
        {
            return new GraceContainer(configuration);
        }
    }
}
