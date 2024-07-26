using Better.Singletons.Runtime;
using Better.UIProcessor.Runtime.Databases;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Modules;
using UnityEngine;

namespace TEST
{
    public class FragmentsManager : MonoSingleton<FragmentsManager>, IModuleProvider<FragmentsModule>
    {
        [SerializeField] private GameObject[] _fragmentsPrefabs;

        private FragmentsModule _fragmentsModule;

        public FragmentsModule GetModule()
        {
            if (_fragmentsModule == null)
            {
                var fragmentDatabase = new PrefabFragmentDatabase(_fragmentsPrefabs);
                _fragmentsModule = new FragmentsModule(fragmentDatabase);
            }

            return _fragmentsModule;
        }
    }
}