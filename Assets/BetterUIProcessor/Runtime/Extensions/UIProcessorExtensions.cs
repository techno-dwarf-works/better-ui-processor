using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Extensions
{
    public static class UIProcessorExtensions
    {
        // TODO
        // Module operations with Generic
        //         public bool RemoveModule(Type type) +TryAdd +GetOrAdd

        public static bool IsOpened<TElement>(this UIProcessor self)
            where TElement : IElement
        {
            // TODO: can be exc when null?
            return self.OpenedElement is TElement;
        }

        public static bool TryGetOpened<TElement>(this UIProcessor self, out TElement element)
            where TElement : IElement
        {
            // TODO: can be exc when null?
            if (self.OpenedElement is TElement castedElement)
            {
                element = castedElement;
                return true;
            }

            element = default;
            return false;
        }

        public static TElement GetOpened<TElement>(this UIProcessor self)
            where TElement : IElement
        {
            if (self.TryGetOpened<TElement>(out var element))
            {
                return element;
            }

            // TODO: throw exc
            return default;
        }

        public static bool TryInitialize(this UIProcessor self)
        {
            if (self.Initialized)
            {
                return false;
            }

            self.Initialize();
            return self.Initialized;
        }
    }
}