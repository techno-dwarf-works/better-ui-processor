using System;
using System.Threading.Tasks;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Elements;
using Better.UISystem.Runtime.Interfaces;
using Better.UISystem.Runtime.Modules.OpenElements;
using Better.UISystem.Runtime.TransitionInfos;

namespace Better.UISystem.Runtime.Modules.Reusing
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