using System;
using System.Collections.Generic;
using System.Linq;
using Better.Attributes.Runtime.Select;
using UnityEngine;

namespace Better.UIComposite.Runtime
{
    [Serializable]
    public class GuaranteeCondition : FragmentCondition
    {
        [Dropdown(FragmentServiceLibrary.EditorIdsGetter, ShowUniqueKey = true)] [SerializeField]
        private string[] _ids;

        public override List<Fragment> VerifyRequest(List<Fragment> ownFragments, List<Fragment> fragments)
        {
            var neededFragments = new List<Fragment>();
            foreach (var id in _ids)
            {
                neededFragments.AddRange(ownFragments.Where(x => x.Identifier.CompareId(id)));
                neededFragments.AddRange(fragments.Where(x => x.Identifier.CompareId(id)));
            }

            return neededFragments;
        }

        public override List<string> VerifyCreate(List<Fragment> ownFragments)
        {
            var buffer = new List<string>(_ids);
            foreach (var id in _ids)
            {
                if (ownFragments.Any(x => x.Identifier.CompareId(id)))
                {
                    buffer.Remove(id);
                }
            }

            return buffer;
        }
    }
}