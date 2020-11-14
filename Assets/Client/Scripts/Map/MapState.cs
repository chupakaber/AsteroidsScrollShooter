using UniRx;

namespace Client.Map
{
    public class MapState
    {
        public ReactiveProperty<int> SelectedLevel { get; private set; }

        public MapState()
        {
            SelectedLevel = new ReactiveProperty<int>();
        }
    }
}