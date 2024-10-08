using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.Locators.Runtime;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class ElementPrefabsModule<TElement> : Module<DirectedTransitionInfo>
        where TElement : IElement
    {
        [SerializeField] private Locator<TElement> _prefabs;

        public override int Priority => ModulePriority.DefaultWithOffset(-1);

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

        public ElementPrefabsModule(IEnumerable<GameObject> rawPrefabs) : this()
        {
            foreach (var rawPrefab in rawPrefabs)
            {
                if (rawPrefab.TryGetComponent(out TElement prefab))
                {
                    _prefabs.Add(prefab);
                }
                else
                {
                    var message = $"Getting {nameof(Component)} from {nameof(rawPrefab)}({rawPrefab}), be skipped";
                    Debug.LogWarning(message, rawPrefab);
                }
            }
        }

        protected override async Task<ProcessResult<IElement>> TryGetTransitionElement(UIProcessor processor, DirectedTransitionInfo transitionInfo)
        {
            var result = await base.TryGetTransitionElement(processor, transitionInfo);
            if (result.IsSuccessful)
            {
                return result;
            }

            if (TryCreateElement(processor.Container, transitionInfo.ElementType, out var element))
            {
                await element.InitializeAsync(transitionInfo.CancellationToken);

                if (transitionInfo.IsCanceled && await TryReleaseElement(processor, element))
                {
                    return ProcessResult<IElement>.Unsuccessful;
                }

                return new ProcessResult<IElement>(element);
            }

            return ProcessResult<IElement>.Unsuccessful;
        }

        private bool TryCreateElement(RectTransform container, Type elementType, out TElement element)
        {
            if (_prefabs.TryGet(elementType, out var prefab))
            {
                var rectTransform = Object.Instantiate(prefab.RectTransform, container);
                if (rectTransform.TryGetComponent(out element))
                {
                    return true;
                }

                rectTransform.DestroyGameObject();
                var message = $"{nameof(rectTransform)}({rectTransform.name}) haven`t {nameof(elementType)}({elementType}), was destroyed";
                Debug.LogWarning(message);
            }

            element = default;
            return false;
        }

        protected internal override async Task<bool> TryReleaseElement(UIProcessor processor, IElement element)
        {
            var result = await base.TryReleaseElement(processor, element);
            if (!result)
            {
                element.RectTransform.DestroyGameObject();
            }

            return true;
        }
    }

    [Serializable]
    public class ElementPrefabsModule : ElementPrefabsModule<IElement>
    {
        public ElementPrefabsModule()
        {
        }

        public ElementPrefabsModule(IEnumerable<IElement> prefabs) : base(prefabs)
        {
        }

        public ElementPrefabsModule(IEnumerable<GameObject> rawPrefabs) : base(rawPrefabs)
        {
        }
    }
}