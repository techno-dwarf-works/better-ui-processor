using Better.Internal.Core.Runtime;
using Better.ProjectSettings.EditorAddons;
using Better.UIProcessor.Runtime;
using UnityEditor;

namespace Better.UIProcessor.EditorAddons.Settings
{
    public class UIProcessorSettingsProvider : DefaultProjectSettingsProvider<UIProcessorSettings>
    {
        public UIProcessorSettingsProvider() : base(UIProcessorSettings.Path)
        {
            keywords = new[] { "processor", "ui", "screen", "popup" };
        }

        [MenuItem(UIProcessorSettings.Path + "/" + PrefixConstants.HighlightPrefix, false, 999)]
        private static void Highlight()
        {
            SettingsService.OpenProjectSettings(ProjectPath + UIProcessorSettings.Path);
        }
    }
}