using System.Threading.Tasks;
using Better.Attributes.Runtime;
using Better.Commons.Runtime.Extensions;
using UnityEngine;

namespace TEST
{
    public class UseTestPopup : MonoBehaviour
    {
        [EditorButton]
        public void OpenHUD()
        {
            Open<HUDScreen, HUDView, HUDModel>("HUD").Forget();
        }

        [EditorButton]
        public void OpenMicro()
        {
            Open<MicroScreen, MicroView, MicroModel>("Micro").Forget();
        }

        [EditorButton]
        public void OpenArab()
        {
            Open<ArabScreen, ArabView, ArabModel>("Arab").Forget();
        }

        [EditorButton]
        public void OpenAss()
        {
            Open<AssScreen, AssView, AssModel>("Ass").Forget();
        }

        [EditorButton]
        public void OpenGameOver()
        {
            Open<GameOverScreen, GameOverView, GameOverModel>("GameOver").Forget();
        }

        [EditorButton]
        public void OpenSettings()
        {
            Open<SettingsScreen, SettingsView, SettingsModel>("Settings").Forget();
        }

        private static async Task Open<TScreen, TView, TModel>(string prefix)
            where TView : ScreenView
            where TScreen : Screen<TView, TModel>
            where TModel : ScreenModel, new()
        {
            Debug.Log($"{prefix}: started");
            var model = new TModel();
            var result = await PopupsManager.Instance.CreateDirectedTransition<TScreen, TView, TModel>(model).Run().Await();
            Debug.Log($"{prefix}: end with {nameof(result)} = {result.IsSuccessful}", result.Result);
        }

        [EditorButton]
        public async Task Close()
        {
            Debug.Log("Close: started");
            var result = await PopupsManager.Instance.CreateCloseTransition().Run().Await();
            Debug.Log($"Close: end with {nameof(result)} = {result}");
        }

        [Min(0)]
        public int HistoryDepth = 1;
        public bool SafeDepth;

        [EditorButton]
        public async Task BackHistory()
        {
            Debug.Log($"History: started, depth = {HistoryDepth}");
            
            var result = await PopupsManager.Instance
                .CreateHistoricalTransition(HistoryDepth)
                .SetSafeDepth(SafeDepth)
                .Run()
                .Await();
            
            Debug.Log($"History: end with {nameof(result)} = {result}");
        }
    }
}