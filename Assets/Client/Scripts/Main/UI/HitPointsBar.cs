using UnityEngine;
using UniRx;

namespace Client.Main
{
    public class HitPointsBar : MonoBehaviour
    {
        readonly int LostAnimatorHash = Animator.StringToHash("Lost");

        [SerializeField] GameObject _heartPrefab = default;
        [SerializeField] RectTransform _containerTransform = default;
        
        Animator[] _hearts = null;
        int _lastValue = 0;
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.INIT_PLAYER)
            .Subscribe(msg => {
                var playerInfo = (PlayerInfo) msg.Context;
                Init(playerInfo);
            }).AddTo(_disposables);
        }

        void OnDisable()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
        }

        void Init(PlayerInfo playerInfo)
        {
            var heartsCount = playerInfo.HitPoints.Value;
            _hearts = new Animator[heartsCount];
            _lastValue = heartsCount;
            for (var i = 0; i < heartsCount; i++)
            {
                var heart = GameObject.Instantiate<GameObject>(_heartPrefab, _containerTransform).GetComponent<Animator>();
                _hearts[i] = heart;
            }

            playerInfo.HitPoints
            .ObserveEveryValueChanged(value => value.Value)
            .Subscribe(value => {
                SetValue(value);
            })
            .AddTo(_disposables);
        }

        void SetValue(int value)
        {
            for (var i = value; i < _lastValue; i++)
            {
                var index = _hearts.Length - i - 1;
                if (index >= 0 && index < _hearts.Length)
                {
                    _hearts[index].SetTrigger(LostAnimatorHash);
                }
            }
            _lastValue = value;
        }
    }
}