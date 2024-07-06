using System;
using System.Threading.Tasks;
using Better.UIComposite.Runtime.Common;
using Better.UIComposite.Runtime.Elements;
using Better.UIComposite.Runtime.Interfaces;
using Better.UIComposite.Runtime.Modules.OpenElements;
using Better.UIComposite.Runtime.TransitionInfos;

namespace Better.UIComposite.Runtime.Modules.Reusing
{
    [Serializable]
    public class ReusableModule : SystemModule<OpenTransitionInfo>
    {
        protected override Task<Result<ISystemElement>> TryHandleOpen(OpenTransitionInfo info)
        {
            var element = System.OpenedElement;
            if (element.GetType() == info.PresenterType)
            {
                element.SetModel(info.DerivedModel);
                return Task.FromResult(new Result<ISystemElement>(element));
            }

            return Task.FromResult(Result<ISystemElement>.GetUnsuccessful());
        }

        protected override void OnInitialize()
        {
        }

        protected override void OnDeconstruct()
        {
        }
    }
}