using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Components.UI;
using UnityEngine;

namespace Better.UIProcessor.Runtime
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ElementView : UIMonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        protected CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }

                return _canvasGroup;
            }
        }

        public bool Interactable
        {
            get => CanvasGroup.interactable;
            set => CanvasGroup.interactable = value;
        }

        public bool Displayed
        {
            get => CanvasGroup.alpha > 0f;
            set => CanvasGroup.alpha = value ? 1f : 0f;
        }

        public virtual Task ShowAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task HideAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}