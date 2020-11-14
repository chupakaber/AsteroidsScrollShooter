using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UniRx;

namespace Client.Map
{
    public class LevelButton : MonoBehaviour
    {
        readonly string LabelTemplate = "Level {0}\n<size=72>Asteroids: {1}";

        [SerializeField] Button _button;
        [SerializeField] TextMeshProUGUI _label;

        public void Init(int levelID, int asteroids, MapState mapState, bool available)
        {
            _label.text = string.Format(LabelTemplate, levelID, asteroids);
            _button.OnClickAsObservable().Subscribe(_ => {
                mapState.SelectedLevel.Value = levelID;
            }).AddTo(this);
            _button.interactable = available;
        }
    }
}