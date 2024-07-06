using Better.UIComposite.Runtime.TransitionInfos;

namespace Better.UIComposite.Runtime.Modules.Historical
{
    public class HistoryInfo
    {
        public IStackable Stackable { get; }
        public TransitionInfo Info { get; }

        public HistoryInfo(IStackable stackable, TransitionInfo info)
        {
            Stackable = stackable;
            Info = info;
        }

        public void Deconstruct(out IStackable screen, out TransitionInfo info)
        {
            screen = Stackable;
            info = Info;
        }
    }
}