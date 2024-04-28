using System;

namespace Better.UISystem.Runtime.Common
{
    [Serializable]
    public abstract class Condition
    {
        public abstract bool Verify();
    }
}