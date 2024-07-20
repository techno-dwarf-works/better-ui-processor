using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Extensions
{
    public static class UIProcessorExtensions
    {
        public static UIProcessor Initialize(this UIProcessor self)
        {
            return self.Initialize(self.Container);
        }

        public static bool TryInitialize(this UIProcessor self, RectTransform container)
        {
            if (self.Initialized)
            {
                return false;
            }

            self.Initialize(container);
            return self.Initialized;
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

        public static UIProcessor ReleaseElement(this UIProcessor self, IElement element)
        {
            self.ReleaseElementAsync(element).Forget();
            return self;
        }

        public static Task ReleaseElementsAsync(this UIProcessor self, IEnumerable<IElement> elements)
        {
            if (elements == null)
            {
                return Task.CompletedTask;
            }

            return elements.Select(self.ReleaseElementAsync).WhenAll();
        }

        public static UIProcessor ReleaseElements(this UIProcessor self, IEnumerable<IElement> elements)
        {
            self.ReleaseElementsAsync(elements).Forget();
            return self;
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