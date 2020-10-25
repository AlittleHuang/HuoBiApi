using System;

namespace HuobiApi.AutomaticInjection.Attributes {
    public class Component : Attribute {
        public Component(Scope scope) {
            Scope = scope;
        }

        public Component() {
        }

        public Scope Scope { get; private set; } = Scope.SINGLETON;
    }

    public enum Scope {
        SINGLETON,
        SCOPE,
        TRANSIENT
    }
}