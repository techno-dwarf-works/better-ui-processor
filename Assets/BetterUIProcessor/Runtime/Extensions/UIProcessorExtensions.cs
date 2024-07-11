using System;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Modules;

namespace Better.UIProcessor.Runtime.Extensions
{
    public static class UIProcessorExtensions
    {
        public static bool TryInitialize(this UIProcessor self)
        {
            if (self.Initialized)
            {
                return false;
            }

            self.Initialize();
            return self.Initialized;
        }

        public static bool HasOpenedElement(this UIProcessor self)
        {
            return self.OpenedElement != null;
        }

        public static bool IsOpenedElement<TElement>(this UIProcessor self)
            where TElement : IElement
        {
            return self.OpenedElement is TElement;
        }

        public static bool TryGetOpenedElement<TElement>(this UIProcessor self, out TElement element)
            where TElement : IElement
        {
            if (self.OpenedElement is TElement castedElement)
            {
                element = castedElement;
                return true;
            }

            element = default;
            return false;
        }

        public static TElement GetOpenedElement<TElement>(this UIProcessor self)
            where TElement : IElement
        {
            if (self.TryGetOpenedElement<TElement>(out var element))
            {
                return element;
            }

            var message = $"{nameof(UIProcessor.OpenedElement)} is not {typeof(TElement)}";
            throw new InvalidOperationException(message);
        }

        public static bool AddModule<TModule>(this UIProcessor self)
            where TModule : Module, new()
        {
            var module = new TModule();
            return self.AddModule(module);
        }

        public static bool TryGetModule<TModule>(this UIProcessor self, out TModule module)
            where TModule : Module
        {
            var type = typeof(TModule);
            if (self.TryGetModule(type, out var baseModule)
                && baseModule is TModule castedModule)
            {
                module = castedModule;
                return true;
            }

            module = null;
            return false;
        }

        public static Module GetModule(this UIProcessor self, Type moduleType)
        {
            if (!self.TryGetModule(moduleType, out var module))
            {
                var message = $"{nameof(Module)}({moduleType}) not found";
                throw new InvalidOperationException(message);
            }

            return module;
        }

        public static TModule GetModule<TModule>(this UIProcessor self)
            where TModule : Module
        {
            if (!self.TryGetModule<TModule>(out var module))
            {
                var message = $"{nameof(Module)}({typeof(TModule)}) not found";
                throw new InvalidOperationException(message);
            }

            return module;
        }

        public static Task ReleaseOpenedElementAsync(this UIProcessor self)
        {
            return self.ReleaseElementAsync(self.OpenedElement);
        }
    }
}