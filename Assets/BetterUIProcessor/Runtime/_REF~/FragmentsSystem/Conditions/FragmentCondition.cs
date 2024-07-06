using System.Collections.Generic;

namespace Better.UIComposite.Runtime
{
    public abstract class FragmentCondition
    {
        //TODO: Replace string to Addressable Reference
        public abstract List<Fragment> VerifyRequest(List<Fragment> ownFragments, List<Fragment> fragments);
        public abstract List<string> VerifyCreate(List<Fragment> ownFragments);
    }
}