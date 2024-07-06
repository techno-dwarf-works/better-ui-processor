using System.Threading.Tasks;
using Better.UIComposite.Runtime.Elements;
using Better.UIComposite.Runtime.Interfaces;

namespace Better.UIComposite.Runtime.Modules.Historical
{
    public interface IStackable : ISystemElement
    {
        public Task PopStackAsync();
        public Task PushStackAsync();
        public Task ReleasedFormStackAsync();
    }
}