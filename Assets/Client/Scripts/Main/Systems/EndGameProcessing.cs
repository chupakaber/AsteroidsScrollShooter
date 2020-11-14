using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Client.Common;
using UnityEngine.SceneManagement;

namespace Client.Main
{
    public class EndGameProcessing : MonoBehaviour
    {
        LevelInfo _levelInfo = null;
        bool _gameIsEnd = false;
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.INIT_LEVEL)
            .Subscribe(msg => {
                _levelInfo = (LevelInfo) msg.Context;
            }).AddTo(_disposables);

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.GAME_OVER)
            .Subscribe(msg => {
                var success = (bool) msg.Context;
                GameOver(success);
            }).AddTo(_disposables);
        }

        void OnDisable()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
        }

        void GameOver(bool success)
        {
            if (_gameIsEnd)
            {
                return;
            }
            
            _gameIsEnd = true;

            if (success)
            {
                var gameProgress = new GameProgress();
                DataStorage.Load<GameProgress>(ref gameProgress);
                if (_levelInfo.LevelID >= gameProgress.CurrentLevel)
                {
                    gameProgress.CurrentLevel++;
                    DataStorage.Save<GameProgress>(gameProgress);
                }
            }

            Observable.Timer (System.TimeSpan.FromMilliseconds(3000))
            .Subscribe (_ => {
                SceneManager.LoadScene(0);
            }).AddTo(_disposables);
        }
    }
}