using System;
using System.Collections.Generic;

namespace Better.UISystem.Runtime
{
    [Serializable]
    public class NoneCondition : FragmentCondition
    {
        public override List<Fragment> VerifyRequest(List<Fragment> ownFragments, List<Fragment> fragments)
        {
            return new List<Fragment>();
        }

        public override List<string> VerifyCreate(List<Fragment> ownFragments)
        {
            return new List<string>();
        }
    }
}