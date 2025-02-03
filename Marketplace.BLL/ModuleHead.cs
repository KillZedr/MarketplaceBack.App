using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL
{
    public static class ModuleHead
    {
        internal struct InterfaceToImplementation
        {
            public Type Implementation;
            public Type Interface;
        }

        public static void RegisterModule(IServiceCollection services)
        {
            var currentAssembly = Assembly.GetAssembly(typeof(ModuleHead));
            var allTypesAssembly = currentAssembly.GetTypes();

            var serviceTypes = allTypesAssembly
                .Where(type => type.IsAssignableTo(typeof(IService)) && !type.IsInterface);


            var interfaceToImplementationMap = serviceTypes.Select(serviceType =>
            {
                var implementation = serviceType;
                var @interface = serviceType.GetInterfaces()
                .First(serviceInterface => serviceInterface != typeof(IService));
                return new InterfaceToImplementation
                {
                    Implementation = implementation,
                    Interface = @interface
                };
            });


            foreach (var serviceToInterface in interfaceToImplementationMap)
            {
                services.AddScoped(serviceToInterface.Interface, serviceToInterface.Implementation);
            }
        }
    }
}
