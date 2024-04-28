using UnityEngine;

namespace Better.UISystem.Runtime
{
    public interface IFragmentContainer
    {
        RectTransform RectTransform { get; }
        public int Priority { get; }
        public FragmentCondition[] GetFragmentConditions();
        public FragmentHolder[] GetHolders();
    }
}