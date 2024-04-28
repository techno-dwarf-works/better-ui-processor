using System.Threading.Tasks;
using Better.UISystem.Runtime.TransitionInfos;

namespace Better.UISystem.Runtime.Interfaces
{
    public interface ITransitionRunner
    {
        public Task<ISystemElement> RunAsync(TransitionInfo info);
    }
}