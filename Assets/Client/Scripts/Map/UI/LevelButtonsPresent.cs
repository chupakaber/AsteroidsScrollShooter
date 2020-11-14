using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Client.Common;

namespace Client.Map
{
    public class LevelButtonsPresent : MonoBehaviour
    {
        [SerializeField] GameObject _levelButtonPrefab = default;
        [SerializeField] RectTransform _levelButtonContainer = default;

        LevelButton[] _levelButtons = null;

        public void Init(int currentLevel, MapState mapState, LevelInfo[] levels)
        {
            for (var i = 0; i < levels.Length; i++)
            {
                var level = levels[i];
                var levelButton = GameObject.Instantiate<GameObject>(_levelButtonPrefab, _levelButtonContainer).GetComponent<LevelButton>();
                levelButton.Init(i, level.TotalAsteroids, mapState, currentLevel >= i);
            }
        }
    }
}