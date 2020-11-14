using UnityEngine;
using UniRx;

namespace Client.Main
{
    public sealed class ShipPresent : MonoBehaviour
    {
        [SerializeField] Transform Ship;

        ShipInfo _shipInfo = null;
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
            _shipInfo.Position.ObserveEveryValueChanged(value => value.Value)
            .Subscribe(value => {
                Ship.transform.position = value;
            }).AddTo(_disposables);
        }
    }
}