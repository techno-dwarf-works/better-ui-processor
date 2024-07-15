using System;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
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

        public static bool TryAddModule<TModule>(this UIProcessor self)
            where TModule : Module, new()
        {
            var module = new TModule();
            return self.TryAddModule(module);
        }

        public static UIProcessor AddModule(this UIProcessor self, Module module)
        {
            if (!self.TryAddModule(module))
            {
                var message = $"{nameof(Module)}({module}) not added";
                throw new InvalidOperationException(message);
            }

            return self;
        }

        public static UIProcessor AddModule<TModule>(this UIProcessor self)
            where TModule : Module, new()
        {
            if (!self.TryAddModule<TModule>())
            {
                var message = $"{nameof(Module)}({typeof(TModule)}) not added";
                throw new InvalidOperationException(message);
            }

            return self;
        }

        public static bool TryAddModule<TModule>(this UIProcessor self, IModuleProvider<TModule> provider)
            where TModule : Module
        {
            var module = provider.GetModule();
            return self.TryAddModule(module);
        }

        public static UIProcessor AddModule<TModule>(this UIProcessor self, IModuleProvider<TModule> provider)
            where TModule : Module
        {
            var module = provider.GetModule();
            return self.AddModule(module);
        }

        public static Task ReleaseOpenedElementAsync(this UIProcessor self)
        {
            return self.ReleaseElementAsync(self.OpenedElement);
        }

        public static void ReleaseOpenedElement(this UIProcessor self)
        {
            self.ReleaseOpenedElementAsync().Forget();
        }
    }
}