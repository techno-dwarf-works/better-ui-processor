using System;
using System.Threading.Tasks;
using Better.Locators.Runtime;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class ElementPrefabsModule<TElement> : Module<TElement, DirectedTransitionInfo<TElement>>
        where TElement : Component, IElement
    {
        [SerializeField] private Locator<TElement> _prefabs;

        public ElementPrefabsModule()
        {
            _prefabs = new();
        }

        public ElementPrefabsModule(TElement[] prefabs) : this()
        {
            foreach (var prefab in prefabs)
            {
                _prefabs.Add(prefab);
            }
        }

        protected override async Task<ProcessResult<TElement>> TryGetTransitionElement(UIProcessor<TElement> processor, DirectedTransitionInfo<TElement> transitionInfo)
        {
            var result = await base.TryGetTransitionElement(processor, transitionInfo);
            if (result.IsSuccessful)
            {
                return result;
            }

            if (TryCreateElement(processor.Container, transitionInfo.ElementType, out var element))
            {
                return new ProcessResult<TElement>(element);
            }

            return ProcessResult<TElement>.Unsuccessful;
        }

        private bool TryCreateElement(RectTransform container, Type elementType, out TElement element)
        {
            if (_prefabs.TryGet(elementType, out var prefab))
            {
                element = Object.Instantiate(prefab, container);
                return true;
            }

            element = default;
            return false;
        }
    }
}