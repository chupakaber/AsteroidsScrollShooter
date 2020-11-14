using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Client.Common;

namespace Client.Map
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] LevelInfo[] Levels = new LevelInfo[3] {
            new LevelInfo() { LevelID = 0, Seed = 12345, TotalAsteroids = 30 },
            new LevelInfo() { LevelID = 1, Seed = 54321, TotalAsteroids = 60 },
            new LevelInfo() { LevelID = 2, Seed = 12321, TotalAsteroids = 90 }
        };

        [SerializeField] LevelButtonsPresent _levelButtonsPresent = default;

        MapState _mapState = null;
        GameProgress _gameProgress = null;
        CompositeDisposable _disposables = new CompositeDisposable();

        void Start()
        {
            _gameProgress = new GameProgress();
            DataStorage.Load<GameProgress>(ref _gameProgress);

            SetupPersistentState();
            SetupButtons();
        }

        void OnDisable()
        {
            if (_disposables != null)
            {
                _disposables.Dispose ();
            }
        }

        void SetupPersistentState()
        {
            _mapState = new MapState();
            _mapState.SelectedLevel.Value = -1;
            _mapState.SelectedLevel.ObserveEveryValueChanged(value => value.Value)
            .Subscribe(value => {
                if (value > -1)
                {
                    var levelInfo = Levels[value];
                    PersistentState.Get().CurrentLevelInfo = levelInfo;
                    SceneManager.LoadScene(1);
                }
            }).AddTo(_disposables);
        }

        void SetupButtons()
        {
            _levelButtonsPresent.Init(_gameProgress.CurrentLevel, _mapState, Levels);
        }
    }
}