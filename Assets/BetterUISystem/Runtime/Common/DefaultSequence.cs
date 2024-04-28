using System.Threading.Tasks;
using Better.UISystem.Runtime.Elements;
using Better.UISystem.Runtime.Interfaces;

namespace Better.UISystem.Runtime.Common
{
    public class DefaultSequence : Sequence
    {
        public override async Task DoPlay(ISystemElement to)
        {
            await to.PrepareShowAsync();
            await to.ShowAsync();
        }

        public override async Task DoPlay(ISystemElement from, ISystemElement to)
        {
            await Task.WhenAll(
                from.PrepareHideAsync(),
                to.PrepareShowAsync()
            );

            await from.HideAsync();
            await to.ShowAsync();
        }

        public override Sequence GetInverseSequence()
        {
            return this;
        }
    }
}