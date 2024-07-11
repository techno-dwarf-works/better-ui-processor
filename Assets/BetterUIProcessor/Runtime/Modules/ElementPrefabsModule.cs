using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Better.Locators.Runtime;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class ElementPrefabsModule<TElement> : Module<DirectedTransitionInfo<TElement>>
        where TElement : Component, IElement
    {
        [SerializeField] private Locator<TElement> _prefabs;

        public ElementPrefabsModule()
        {
            _prefabs = new();
        }

        public ElementPrefabsModule(IEnumerable<TElement> prefabs) : this()
        {
            foreach (var prefab in prefabs)
            {
                _prefabs.Add(prefab);
            }
        }

        protected override async Task<ProcessResult<IElement>> TryGetTransitionElement(UIProcessor processor, DirectedTransitionInfo<TElement> transitionInfo)
        {
            var result = await base.TryGetTransitionElement(processor, transitionInfo);
            if (result.IsSuccessful)
            {
                return result;
            }

            if (TryCreateElement(processor.Container, transitionInfo.ElementType, out var element))
            {
                await element.InitializeAsync(transitionInfo.CancellationToken);
                if (!transitionInfo.IsCanceled)
                {
                    return new ProcessResult<IElement>(element);
                }
            }

            return ProcessResult<IElement>.Unsuccessful;
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