using UnityEngine;
using UniRx;

namespace Client.Main
{
    public sealed class ShipPresent : MonoBehaviour
    {
        [SerializeField] GameObject ShipPrefab;

        ShipInfo _shipInfo = null;
        Transform _shipTransform = null;
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.INIT_SHIP)
            .Subscribe(msg => {
                var shipInfo = (ShipInfo) msg.Context;
                Init(shipInfo);
            }).AddTo(_disposables);

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.GAME_OVER)
            .Subscribe(msg => {
                var success = (bool) msg.Context;
                if (!success)
                {
                    Destroy();
                }
            }).AddTo(_disposables);
        }

        void OnDisable()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
        }

        void Init(ShipInfo _shipInfo)
        {
            if (_shipTransform == null)
            {
                _shipTransform = GameObject.Instantiate<GameObject>(ShipPrefab).transform;
            }
            _shipInfo.Position.ObserveEveryValueChanged(value => value.Value)
            .Subscribe(value => {
                _shipTransform.transform.position = value;
            }).AddTo(_disposables);
        }

        void Destroy()
        {
            _shipTransform.gameObject.SetActive(false);
        }
    }
}