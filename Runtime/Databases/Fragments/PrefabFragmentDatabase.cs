using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.Locators.Runtime;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Better.UIProcessor.Runtime.Databases
{
    [Serializable]
    public class PrefabFragmentDatabase : IFragmentDatabase
    {
        [SerializeField] private Locator<IFragment> _prefabs;

        public PrefabFragmentDatabase()
        {
            _prefabs = new();
        }

        public PrefabFragmentDatabase(IEnumerable<IFragment> prefabs) : this()
        {
            foreach (var prefab in prefabs)
            {
                _prefabs.Add(prefab);
            }
        }

        public PrefabFragmentDatabase(IEnumerable<GameObject> rawPrefabs) : this()
        {
            foreach (var rawPrefab in rawPrefabs)
            {
                if (rawPrefab.TryGetComponent(out IFragment prefab))
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

        async Task<ProcessResult<IFragment>> IFragmentDatabase.TryCreateFragmentAsync(RectTransform container, Type fragmentType, CancellationToken cancellationToken)
        {
            if (TryCreateFragment(container, fragmentType, out var fragment))
            {
                await fragment.InitializeAsync(cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    return new ProcessResult<IFragment>(fragment);
                }

                DestroyFragment(fragment);
            }

            return ProcessResult<IFragment>.Unsuccessful;
        }

        private bool TryCreateFragment(RectTransform container, Type fragmentType, out IFragment fragment)
        {
            if (_prefabs.TryGet(fragmentType, out var prefab))
            {
                var rectTransform = UObject.Instantiate(prefab.RectTransform, container);
                if (rectTransform.TryGetComponent(out fragment))
                {
                    return true;
                }

                rectTransform.DestroyGameObject();
                var message = $"{nameof(rectTransform)}({rectTransform.name}) haven`t {nameof(fragmentType)}({fragmentType}), was destroyed";
                Debug.LogWarning(message);
            }

            fragment = default;
            return false;
        }

        private void DestroyFragment(IFragment fragment)
        {
            fragment.RectTransform.DestroyGameObject();
        }
    }
}