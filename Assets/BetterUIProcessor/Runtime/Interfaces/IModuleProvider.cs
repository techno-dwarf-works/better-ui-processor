using Better.UIProcessor.Runtime.Modules;

namespace Better.UIProcessor.Runtime.Interfaces
{
    public interface IModuleProvider<out TModule>
        where TModule : Module
    {
        public TModule GetModule();
    }

    public interface IModuleProvider : IModuleProvider<Module>
    {
    }
}