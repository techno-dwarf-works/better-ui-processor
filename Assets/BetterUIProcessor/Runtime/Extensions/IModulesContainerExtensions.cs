using System;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Modules;

namespace Better.UIProcessor.Runtime.Extensions
{
    public static class IModulesContainerExtensions
    {
        public static bool TryAddModule<TModule>(this IModulesContainer self)
            where TModule : Module, new()
        {
            var module = new TModule();
            return self.TryAddModule(module);
        }

        public static IModulesContainer AddModule(this IModulesContainer self, Module module)
        {
            if (!self.TryAddModule(module))
            {
                var message = $"{nameof(Module)}({module}) not added";
                throw new InvalidOperationException(message);
            }

            return self;
        }

        public static IModulesContainer AddModule<TModule>(this IModulesContainer self)
            where TModule : Module, new()
        {
            if (!self.TryAddModule<TModule>())
            {
                var message = $"{nameof(Module)}({typeof(TModule)}) not added";
                throw new InvalidOperationException(message);
            }

            return self;
        }

        public static bool TryAddModule<TModule>(this IModulesContainer self, IModuleProvider<TModule> provider)
            where TModule : Module
        {
            var module = provider.GetModule();
            return self.TryAddModule(module);
        }

        public static IModulesContainer AddModule<TModule>(this IModulesContainer self, IModuleProvider<TModule> provider)
            where TModule : Module
        {
            var module = provider.GetModule();
            return self.AddModule(module);
        }
    }
}