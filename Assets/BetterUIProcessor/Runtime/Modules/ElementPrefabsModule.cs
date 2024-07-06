using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        // TODO: Add Serialization, handle corner use-cases

        [SerializeField] private List<TElement> _prefabs;

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
            if (TryGetPrefab(elementType, out TElement prefab))
            {
                element = Object.Instantiate(prefab, container);
                return true;
            }

            element = default;
            return false;
        }

        private bool TryGetPrefab(Type elementType, out TElement elementPrefab)
        {
            foreach (var prefab in _prefabs)
            {
                if (prefab.GetType() == elementType)
                {
                    elementPrefab = prefab;
                    return true;
                }
            }

            elementPrefab = default;
            return false;
        }
    }
}