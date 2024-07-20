using Better.UIProcessor.Runtime.Modules;

namespace Better.UIProcessor.Runtime.Interfaces
{
    public interface IModulesContainer
    {
        public bool TryAddModule(Module module);
        public bool ContainsModule(Module module);
        public bool RemoveModule(Module module);
    }
}