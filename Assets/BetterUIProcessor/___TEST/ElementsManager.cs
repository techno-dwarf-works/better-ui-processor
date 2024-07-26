using System.Threading;
using Better.Attributes.Runtime.Select;
using Better.Singletons.Runtime;
using Better.UIProcessor.Runtime;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Modules;
using Better.UIProcessor.Runtime.Sequences;
using UnityEngine;
using UnityEngine.Serialization;

namespace TEST
{
    public class ElementsManager<TSelf> : MonoSingleton<TSelf> where TSelf : MonoSingleton<TSelf>
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private GameObject[] _screenPrefabs;

        [Select]
        [SerializeReference] private Sequence[] _sequences;

        [SerializeField] private GameObject _rawPrewarmElement;

        private UIProcessor _processor;

        private void Awake()
        {
            var prefabsModule = new ElementPrefabsModule<IElement>(_screenPrefabs);
            var sequencesModule = new SequencesModule(_sequences);
            var historicalModule = new HistoricalModule();
            historicalModule.SetAutoClear();

            _processor = new();

            if (_rawPrewarmElement != null && _rawPrewarmElement.TryGetComponent(out IElement prewarmElement))
            {
                _processor.Initialize(_container, prewarmElement);
            }
            else
            {
                _processor.Initialize(_container);
            }

            _processor.AddModule<ModelDispatcherModule>()
                .AddModule<CloseElementModule>()
                .AddModule(historicalModule)
                .AddModule(prefabsModule)
                .AddModule(sequencesModule)
                .AddModule(FragmentsManager.Instance);
        }

        public DirectedTransitionInfo<TScreen, TModel> CreateDirectedTransition<TScreen, TView, TModel>(TModel model, CancellationToken cancellationToken = default)
            where TScreen : Screen<TView, TModel>
            where TView : ScreenView
            where TModel : ScreenModel
        {
            return new DirectedTransitionInfo<TScreen, TModel>(_processor, model, cancellationToken);
        }

        public CloseTransitionInfo CreateCloseTransition(CancellationToken cancellationToken = default)
        {
            return new CloseTransitionInfo(_processor, cancellationToken);
        }

        public HistoricalTransitionInfo CreateHistoricalTransition(int depth, CancellationToken cancellationToken = default)
        {
            return new HistoricalTransitionInfo(_processor, depth, cancellationToken);
        }
    }
}