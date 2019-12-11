using System;
using System.Collections.Generic;
using System.Text;

namespace GitWatch.Domain
{
    public interface IContainer : IServiceProvider
    {
        T Get<T>();
        void Replace<I, T>();
        void Replace<I>(I instance);

    }
}
