using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Better.Extensions.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Better.UIComposite.Runtime
{
    public class FragmentService : MonoService
    {
        [SerializeField] private RectTransform _fragmentContainer;

        [SerializeField] private FragmentServiceLibrary _library;

        private Dictionary<IFragmentContainer, List<Fragment>> _fragmentContainerMap;
        //TODO: Use SerializedType instead Identifier
        private Dictionary<Identifier, Fragment> _prefabsMap;

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            _prefabsMap = _library.FragmentPrefabs.ToDictionary(key => key.Identifier);
            _fragmentContainerMap = new Dictionary<IFragmentContainer, List<Fragment>>();
            return Task.CompletedTask;
        }

        public void RegisterContainer(IFragmentContainer container)
        {
            if (!_fragmentContainerMap.ContainsKey(container))
            {
                _fragmentContainerMap.Add(container, new List<Fragment>());
            }
        }

        public async Task UnregisterContainer(IFragmentContainer container)
        {
            if (_fragmentContainerMap.TryGetValue(container, out var list))
            {
                _fragmentContainerMap.Remove(container);
                await list.Select(x => x.OnUnlink()).WhenAll();
            }

            await Task.Yield();
        }

        public async Task Link(IFragmentContainer container)
        {
            if (_fragmentContainerMap.TryGetValue(container, out var ownFragments))
            {
                var list = new List<Task>();
                foreach (var fragment in ownFragments)
                {
                    var rectTransform = SelectHolder(container, fragment.DesiredHolderId);
                    LinkFragment(rectTransform, fragment);
                    var task = fragment.OnLink();
                    list.Add(task);
                }

                await Task.WhenAll(list);
            }

            await Task.Yield();
        }

        //TODO: Clean up with Request Fragments
        public Task ReleaseFragments(IFragmentContainer container)
        {
            if (!_fragmentContainerMap.TryGetValue(container, out var ownFragments))
            {
                return Task.CompletedTask;
            }

            if (ownFragments.Count < 0)
            {
                return Task.CompletedTask;
            }

            var others = _fragmentContainerMap.Where(x => x.Key != container);
            var descending = others.OrderByDescending(x => x.Key.Priority);
            foreach (var (fragmentContainer, fragments) in descending)
            {
                var fragmentsToRequest = new List<Fragment>();

                var fragmentConditions = fragmentContainer.GetFragmentConditions();

                foreach (var condition in fragmentConditions)
                {
                    var bufferOwnFragments = fragments;
                    var conditionFragments = condition.VerifyRequest(bufferOwnFragments, ownFragments);

                    fragmentsToRequest.AddRangeDistinct(conditionFragments);
                    ownFragments.RemoveRange(conditionFragments);
                }

                foreach (var fragment in fragmentsToRequest)
                {
                    SnapFragment(fragmentContainer, fragment);

                    LinkFragment(_fragmentContainer, fragment);
                }
            }


            return Task.CompletedTask;
        }

        //TODO: Clean up with Request Fragments
        public async Task RequestFragments(IFragmentContainer container)
        {
            if (!_fragmentContainerMap.TryGetValue(container, out var ownFragments))
            {
                ownFragments = new List<Fragment>();
            }

            var otherFragments = _fragmentContainerMap.Where(x => container.Priority > x.Key.Priority).SelectMany(x => x.Value).ToList();
            var fragmentsToRequest = new List<Fragment>();
            var bufferOwnFragments = new List<Fragment>(ownFragments);

            var fragmentConditions = container.GetFragmentConditions();

            foreach (var condition in fragmentConditions)
            {
                var conditionFragments = condition.VerifyRequest(bufferOwnFragments, otherFragments);

                fragmentsToRequest.AddRangeDistinct(conditionFragments);
                otherFragments.RemoveRange(conditionFragments);
                bufferOwnFragments.AddRangeDistinct(conditionFragments);
            }

            foreach (var fragment in fragmentsToRequest)
            {
                SnapFragment(container, fragment);
                LinkFragment(_fragmentContainer, fragment);
            }

            var fragmentsToSpawn = new List<string>();
            foreach (var condition in fragmentConditions)
            {
                var spawnTypes = condition.VerifyCreate(bufferOwnFragments);
                fragmentsToSpawn.AddRangeDistinct(spawnTypes);
            }

            foreach (var identifier in fragmentsToSpawn)
            {
                var prefab = await GetFragmentPrefab(identifier);
                var instance = Instantiate(prefab, _fragmentContainer);
                SnapFragment(container, instance);
                var rectTransform = SelectHolder(container, instance.DesiredHolderId);
                LinkFragment(rectTransform, instance);
            }
        }

        private RectTransform SelectHolder(IFragmentContainer container, string id)
        {
            var holders = container.GetHolders();
            foreach (var fragmentHolder in holders)
            {
                if (fragmentHolder.Id.Equals(id))
                {
                    return fragmentHolder.RectTransform;
                }
            }

            return container.RectTransform;
        }

        private void LinkFragment(RectTransform container, Fragment fragment)
        {
            var fragmentTransform = fragment.RectTransform;
            var anchoredPosition = fragmentTransform.anchoredPosition;
            fragmentTransform.SetParent(container, false);
            fragmentTransform.anchoredPosition = anchoredPosition;
        }

        private void SnapFragment(IFragmentContainer container, Fragment fragment)
        {
            var containing = _fragmentContainerMap.Where(x => x.Value.Contains(fragment));
            foreach (var valuePair in containing)
            {
                valuePair.Value.Remove(fragment);
            }

            if (_fragmentContainerMap.TryGetValue(container, out var list))
            {
                list.Add(fragment);
            }
        }

        private async Task<Fragment> GetFragmentPrefab(string id)
        {
            // TODO: Upgrade to Addressable
            foreach (var (identifier, fragmentPrefab) in _prefabsMap)
            {
                if (identifier.CompareId(id))
                {
#if UNITY_EDITOR
                    //TODO: Controllable unexpected operation
                    await TaskUtility.WaitFrame(Random.Range(0, 20));
#endif
                    return fragmentPrefab;
                }
            }

            throw new InvalidOperationException($"[{nameof(FragmentService)}] {nameof(GetFragmentPrefab)}: not found prefab with identifier: {id}");
        }
    }
}