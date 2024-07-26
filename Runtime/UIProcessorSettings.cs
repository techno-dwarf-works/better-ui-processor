using Better.Internal.Core.Runtime;
using Better.ProjectSettings.Runtime;
using Better.Singletons.Runtime.Attributes;
using Better.UIProcessor.Runtime.Settings;

namespace Better.UIProcessor.Runtime
{
    [ScriptableCreate(Path)]
    public class UIProcessorSettings : ScriptableSettings<UIProcessorSettings, SettingsData>
    {
        public const string Path = PrefixConstants.BetterPrefix + "/" + nameof(Better.UIProcessor);
    }
}