using System;
using System.Collections.Generic;
using Better.Attributes.Runtime;
using Better.Attributes.Runtime.Manipulation;
using Better.Extensions.Runtime;
using Better.Singletons.Runtime;
using UnityEngine;

namespace Better.UIComposite.Runtime
{
    [CreateAssetMenu(menuName = "Settings/Fragments/Library")]
    public class FragmentServiceLibrary : ScriptableSingletonAsset<FragmentServiceLibrary>
    {
        public const string EditorHolderIdsGetter = "r:FragmentServiceLibrary.EditorInstance.GetHolderIds()";

        public const string EditorIdsGetter = "r:FragmentServiceLibrary.EditorInstance.GetIds()";

        [Header("FRAGMENTS")] [SerializeField, ReadOnly]
        private Fragment[] _fragmentPrefabs;

        [SerializeField] private Identifier[] _holderIdentifiers;

        public Fragment[] FragmentPrefabs => _fragmentPrefabs;

        private List<Tuple<string, string>> GetIds()
        {
            var items = _fragmentPrefabs.Select(x => new Tuple<string, string>(x.Identifier.Name, x.Identifier.Id).ToList();

            return items;
        }

        private List<Tuple<string, string>> GetHolderIds()
        {
            var items = _holderIdentifiers.Select(x => new Tuple<string, string>(x.Name, x.Id).ToList();

            return items;
        }


#if UNITY_EDITOR

        [EditorButton("CACHE FRAGMENT PREFABS")]
        private void CacheFragmentPrefabsEditor()
        {
            var prefabs = AssetDatabaseUtility.FindPrefabsOfType<Fragment>();
            _fragmentPrefabs = prefabs;
        }
#endif
    }
}