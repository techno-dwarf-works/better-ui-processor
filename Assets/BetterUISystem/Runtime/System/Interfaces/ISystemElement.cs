using System.Threading.Tasks;
using Better.UISystem.Runtime.Elements;
using UnityEngine;

namespace Better.UISystem.Runtime.Interfaces
{
    public interface ISystemElement
    {
        public RectTransform RectTransform { get; }
        public Task InitializeAsync();
        public Task PrepareShowAsync();
        public Task ShowAsync();
        public Task PrepareHideAsync();
        public Task HideAsync();
        
        public void SetModel(ElementModel model);
    }
}