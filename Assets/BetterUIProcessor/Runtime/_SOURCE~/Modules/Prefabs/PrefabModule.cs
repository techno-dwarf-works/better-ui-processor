using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Better.Commons.Runtime.Utility;
using Better.UIComposite.Runtime.Common;
using Better.UIComposite.Runtime.Elements;
using Better.UIComposite.Runtime.Interfaces;
using Better.UIComposite.Runtime.Modules.OpenElements;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Better.UIComposite.Runtime.Modules.Prefabs
{
    [Serializable]
    public abstract class PrefabModule : SystemModule<OpenTransitionInfo>
    {
        [SerializeField] private SystemElement[] _prefabs;

        private Dictionary<Type, SystemElement> _elementsMap;
        
        protected override void OnInitialize()
        {
            _elementsMap = _prefabs.ToDictionary(key => key.GetType());
        }

        protected override async Task<Result<ISystemElement>> TryHandleOpen(OpenTransitionInfo info)
        {
            var openedPresenter = await InstantiateAsync(info.PresenterType);
            await openedPresenter.InitializeAsync();
            openedPresenter.SetModel(info.DerivedModel);
            return new Result<ISystemElement>(openedPresenter);
        }

        protected override Task<Result<Sequence>> TryGetTransitionSequence(OpenTransitionInfo info)
        {
            if (!info.OverridenSequence || !System.TryGetSequence(info.SequenceType, out var sequence))
            {
                sequence = System.GetDefaultSequence();
                
                return Task.FromResult(new Result<Sequence>(sequence));
            }

            return Task.FromResult(Result<Sequence>.GetUnsuccessful());
        }

        private async Task<ISystemElement> InstantiateAsync(Type presenterType)
        {
            if (!_elementsMap.TryGetValue(presenterType, out var prefab))
            {
                var message = $"Unexpected {nameof(presenterType)}({presenterType})";
                throw new InvalidOperationException(message);
            }

#if UNITY_EDITOR
            //TODO: Controllable unexpected operation
            var framesDelay = Random.Range(1, 20);
            await TaskUtility.WaitFrame(framesDelay);
#endif

            var popup = Object.Instantiate(prefab, System.Container);
            return popup;
        }
    }
}