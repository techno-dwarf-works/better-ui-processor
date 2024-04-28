using System.Threading.Tasks;
using Better.UISystem.Runtime.Elements;
using Better.UISystem.Runtime.Interfaces;

namespace Better.UISystem.Runtime.Modules.Historical
{
    public interface IStackable : ISystemElement
    {
        public Task PopStackAsync();
        public Task PushStackAsync();
        public Task ReleasedFormStackAsync();
    }
}