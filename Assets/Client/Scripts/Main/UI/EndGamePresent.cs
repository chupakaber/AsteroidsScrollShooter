using UnityEngine;
using UniRx;

namespace Client.Main
{
    public class EndGamePresent : MonoBehaviour
    {
        readonly int WinAnimatorHash = Animator.StringToHash("Win");
        readonly int LooseAnimatorHash = Animator.StringToHash("Loose");

        [SerializeField] Animator _animator = default;
        
        bool _gameIsEnd = false;
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
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

        public void GameOver(bool success)
        {
            if (_gameIsEnd)
            {
                return;
            }
            
            _gameIsEnd = true;
            
            if (success)
            {
                _animator.SetTrigger(WinAnimatorHash);
            }
            else
            {
                _animator.SetTrigger(LooseAnimatorHash);
            }
        }
    }
}