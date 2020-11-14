using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Client.Main
{
    public class BulletProcessing : MonoBehaviour
    {
        AsteroidInfo[] _asteroids = null;
        List<BulletInfo> _bullets = null;
        ShipInfo _shipInfo = null;
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
            _bullets = new List<BulletInfo>();

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.INIT_ASTEROIDS)
            .Subscribe(msg => {
                _asteroids = (AsteroidInfo[]) msg.Context;
            }).AddTo(_disposables);

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.INIT_SHIP)
            .Subscribe(msg => {
                _shipInfo = (ShipInfo) msg.Context;
            }).AddTo(_disposables);

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.CREATE_BULLET)
            .Subscribe(msg => {
                var position = (Vector3) msg.Context;
                CreateBullet(position);
            }).AddTo(_disposables);
        }

        void OnDisable()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
        }

        void Update()
        {
            if (_asteroids == null)
            {
                return;
            }

            var deltaTime = Time.deltaTime;

            ApplyBulletMotion(deltaTime);
            CheckBulletsCollision();
        }

        void ApplyBulletMotion(float deltaTime)
        {
            for (var i = 0; i < _bullets.Count; i++)
            {
                var bullet = _bullets[i];
                bullet.ApplyMotion(deltaTime, new Vector3(0f, 0f, -_shipInfo.Speed));
                if (bullet.EndTime < Time.time)
                {
                    _bullets.Remove(bullet);
                    i--;
                }
            }
        }

        void CheckBulletsCollision()
        {
            for (var i = 0; i < _asteroids.Length; i++)
            {
                var asteroid1 = _asteroids[i];
                if (!asteroid1.Frozen)
                {
                    for (var j = 0; j < _bullets.Count; j++)
                    {
                        var bullet = _bullets[j];
                        if (ISceneObjectInfo.CheckCollision(asteroid1, bullet, out var deltaPositionNormalized))
                        {
                            asteroid1.Durability -= _shipInfo.WeaponDamage;
                            if (asteroid1.Durability <= 0f)
                            {
                                MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.CREATE_EXPLOSION, asteroid1.Position.Value));
                                asteroid1.Position.Value = new Vector3(0f, 0f, -2f);
                            }
                            bullet.Position.Value += new Vector3(0f, 0f, 20f);
                            _bullets.Remove(bullet);
                            j--;
                        }
                    }
                }
            }
        }

        void CreateBullet(Vector3 position)
        {
            var bullet = new BulletInfo();
            bullet.EndTime = Time.time + 3f;
            bullet.LinearVelocity = new Vector3(0f, 0f, 10f);
            bullet.Position.Value = position;
            _bullets.Add(bullet);
            
            MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.SHOW_BULLET, bullet));
        }
    }
}