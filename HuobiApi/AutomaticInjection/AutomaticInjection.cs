using System;
using System.Reflection;
using HuobiApi.AutomaticInjection.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace HuobiApi.AutomaticInjection {
    public static class AutomaticInjectionExpand {
        public static void AutomaticInjection(this IServiceCollection services) {
            Type[] types = Assembly.Load("HuobiApi").GetTypes();
            foreach (var type in types) {
                if (type.IsInterface) {
                    continue;
                }

                var component = type.GetCustomAttribute<Component>();
                if (component == null) continue;
                var scope = component.Scope;
                var interfaces = type.GetInterfaces();
                Action<Type, Type> action = scope switch {
                    Scope.SINGLETON => (a, b) => services.AddSingleton(a, b),
                    Scope.SCOPE => (a, b) => services.AddScoped(a, b),
                    Scope.TRANSIENT => (a, b) => services.AddTransient(a, b),
                    _ => null
                };
                if (action == null) continue;
                action(type, type);
                foreach (var i in interfaces) {
                    action(i, type);
                }
            }

        }
    }
}