using UniRx;

namespace Client.Main
{
    public class PlayerInfo
    {
        public ReactiveProperty<int> HitPoints { get; private set; }

        public PlayerInfo(int startHitPoints)
        {
            HitPoints = new ReactiveProperty<int>(startHitPoints);
        }
    }
}