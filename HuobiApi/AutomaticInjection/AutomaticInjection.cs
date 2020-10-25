using System;
using System.Reflection;
using HuobiApi.AutomaticInjection.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace HuobiApi.AutomaticInjection {
    public static class AutomaticInjectionExpand {
        public static IServiceCollection AutomaticInjection(this IServiceCollection services) {
            Type[] types = Assembly.Load("HuobiApi").GetTypes();
            foreach (var type in types) {
                Console.WriteLine(type);
                Component component = type.GetCustomAttribute<Component>();
                if (component != null) {
                    Scope scope = component.Scope;
                    if (scope == Scope.SINGLETON) {
                        services.AddSingleton(type, type);
                    }
                    else if (scope == Scope.SCOPE) {
                        services.AddScoped(type, type);
                    }
                    else if (scope == Scope.TRANSIENT) {
                        services.AddTransient(type, type);
                    }
                }
            }

            return services;
        }
    }
}