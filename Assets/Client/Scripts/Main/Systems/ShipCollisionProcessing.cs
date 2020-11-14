using UnityEngine;
using UniRx;

namespace Client.Main
{
    public sealed class ShipCollisionProcessing : MonoBehaviour
    {
        PlayerInfo _playerInfo = null;
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.INIT_PLAYER)
            .Subscribe(msg => {
                _playerInfo = (PlayerInfo) msg.Context;
            }).AddTo(_disposables);

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.SHIP_COLLIDED)
            .Subscribe(msg => {
                var collider = (ISceneObjectInfo) msg.Context;
                ShipCollided(collider);
            }).AddTo(_disposables);
        }

        void OnDisable()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
        }

        void ShipCollided(ISceneObjectInfo collider)
        {
            
            MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.CREATE_EXPLOSION, collider.Position.Value));
            collider.Position.Value = new Vector3(0f, 0f, -2f);

            _playerInfo.HitPoints.Value--;
            if (_playerInfo.HitPoints.Value == 0)
            {
                MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.GAME_OVER, false));
            }
        }
    }
}
