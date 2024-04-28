using System;
using Better.Commons.Runtime.Utility;
using UnityEngine;

namespace Better.UISystem.Runtime
{
    public partial class UISystem
    {
        protected bool ValidateMutable(bool logException = true)
        {
            if (!Mutable && logException)
            {
                var message = "Is immutable";
                DebugUtility.LogException<AccessViolationException>(message);
            }

            return Mutable;
        }
    }
}