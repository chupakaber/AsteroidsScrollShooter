using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Client.Main
{
    public class ExplosionProcessing : MonoBehaviour
    {
        List<EffectInfo> _explosions = null;
        ShipInfo _shipInfo = null;
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
            _explosions = new List<EffectInfo>();

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.INIT_SHIP)
            .Subscribe(msg => {
                _shipInfo = (ShipInfo) msg.Context;
            }).AddTo(_disposables);

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.CREATE_EXPLOSION)
            .Subscribe(msg => {
                var position = (Vector3) msg.Context;
                CreateExplosion(position);
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

            ApplyExplosionMotion(deltaTime);
        }

        void ApplyExplosionMotion(float deltaTime)
        {
            for (var i = 0; i < _explosions.Count; i++)
            {
                var explosion = _explosions[i];
                explosion.ApplyMotion(deltaTime, new Vector3(0f, 0f, -_shipInfo.Speed));
                if (explosion.EndTime < Time.time)
                {
                    _explosions.Remove(explosion);
                }
            }
        }

        void CreateExplosion(Vector3 position)
        {
            var explosion = new EffectInfo();
            explosion.EndTime = Time.time + 0.5f;
            explosion.Position.Value = position;
            _explosions.Add(explosion);
            
            MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.SHOW_EXPLOSION, explosion));
        }
    }
}