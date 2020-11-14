using UnityEngine;
using UniRx;

namespace Client.Main
{
    public class ShipProcessing : MonoBehaviour
    {
        ShipInfo _shipInfo = null;

        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.INIT_SHIP)
            .Subscribe(msg => {
                _shipInfo = (ShipInfo) msg.Context;
            }).AddTo(_disposables);

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.MOVE_SHIP)
            .Subscribe(msg => {
                var position = (Vector3) msg.Context;
                Move(position);
            }).AddTo(_disposables);

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.SHOOT)
            .Subscribe(msg => {
                Shoot();
            }).AddTo(_disposables);

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.GAME_OVER)
            .Subscribe(msg => {
                var success = (bool) msg.Context;
                if (!success)
                {
                    _shipInfo.Active = false;
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

        public void Update()
        {
            var deltaTime = Time.deltaTime;

            MovingForward(deltaTime);
        }

        void MovingForward(float deltaTime)
        {
            if (_shipInfo == null)
            {
                return;
            }

            _shipInfo.Distance += _shipInfo.Speed * deltaTime;
        }

        void Move(Vector3 position)
        {
            _shipInfo.Position.Value = position;
        }

        void Shoot()
        {
            var position = _shipInfo.Position.Value + _shipInfo.WeaponPlaces[_shipInfo.WeaponLastShotPlace];
            MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.CREATE_BULLET, position));
        }
    }
}