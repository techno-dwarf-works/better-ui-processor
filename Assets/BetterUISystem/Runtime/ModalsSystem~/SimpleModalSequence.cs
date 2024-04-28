using System.Collections.Generic;
using System.Threading.Tasks;
using Better.UISystem.Runtime.PopupsSystem.Interfaces;
using UnityEngine;

namespace Better.UISystem.Runtime
{
    public class SimpleModalSequence : ModalSequence
    {
        public override async Task DoPlay(IPopup from, IPopup to)
        {
            if (from == null && to == null)
            {
                Debug.LogWarning($"[{GetType().Name}] {nameof(DoPlay)}: possible unexpected case");
                return;
            }

            var tasks = new List<Task>();
            if (from != null) tasks.Add(from.PrepareHideAsync());
            if (to != null) tasks.Add(to.PrepareShowAsync());
            await Task.WhenAll(tasks);

            if (from != null) await from.HideAsync();
            if (to != null) await to.ShowAsync();
        }
    }
}