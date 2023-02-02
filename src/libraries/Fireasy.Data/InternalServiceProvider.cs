// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Data
{
    internal class InternalServiceProvider : DisposableBase, IServiceProvider
    {
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _rootServiceProvider;
        private readonly IServiceProvider _innerServiceProvider;

        public InternalServiceProvider(IServiceScope serviceScope, IServiceProvider innerServiceProvider)
        {
            _serviceScope = serviceScope;
            _rootServiceProvider = serviceScope.ServiceProvider;
            _innerServiceProvider = innerServiceProvider;
        }

        object? IServiceProvider.GetService(Type serviceType)
        {
            var obj = _innerServiceProvider.GetService(serviceType);
            if (obj == null)
            {
                obj = _rootServiceProvider.GetService(serviceType);
            }

            return obj;
        }

        protected override bool Dispose(bool disposing)
        {
            if (_innerServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _serviceScope.Dispose();

            return true;
        }
    }
}
