using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Client.Main
{
    public class AsteroidProcessing : MonoBehaviour
    {
        readonly float SpawnDistanceOffset = 14f;

        AsteroidInfo[] _asteroids = null;
        ShipInfo _shipInfo = null;
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
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
            if (_shipInfo == null)
            {
                return;
            }

            var deltaTime = Time.deltaTime;

            UpdateAsteroidsActivity(_shipInfo.Distance);
            ApplyAsteroidsMotion(deltaTime, _shipInfo.Speed);
            CheckAsteroidsCollision();
            CheckEndGame();
            ApplyAsteroidData();
        }

        void CheckAsteroidsCollision()
        {
            for (var i = 0; i < _asteroids.Length; i++)
            {
                var asteroid1 = _asteroids[i];
                if (!asteroid1.Frozen)
                {
                    for (var j = i + 1; j < _asteroids.Length; j++)
                    {
                        var asteroid2 = _asteroids[j];
                        if (!asteroid2.Frozen)
                        {
                            if (ISceneObjectInfo.CheckCollision(asteroid1, asteroid2, out var deltaPositionNormalized))
                            {
                                asteroid1.CollidedWith.Value = asteroid2;
                                asteroid2.CollidedWith.Value = asteroid1;
                                asteroid2.LinearVelocity -= deltaPositionNormalized * asteroid1.Scale.Value * 0.01f;
                                asteroid1.LinearVelocity += deltaPositionNormalized * asteroid2.Scale.Value * 0.01f;
                            }
                        }
                    }
                    if (ISceneObjectInfo.CheckCollision(asteroid1, _shipInfo, out var deltaPositionNormalized2))
                    {
                        MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.SHIP_COLLIDED, asteroid1));
                    }
                }
            }
        }

        void UpdateAsteroidsActivity(float distance)
        {
            foreach (var asteroid in _asteroids)
            {
                if (asteroid.Position.Value.z < -1f)
                {
                    if (!asteroid.Frozen)
                    {
                        asteroid.Frozen = true;
                    }
                }
                else if (asteroid.Position.Value.z < distance && asteroid.Frozen)
                {
                    asteroid.Frozen = false;
                    asteroid.Position.Value += new Vector3(0f, 0f, SpawnDistanceOffset - distance);
                }
            }
        }

        void ApplyAsteroidsMotion(float deltaTime, float shipSpeed)
        {
            foreach (var asteroid in _asteroids)
            {
                if (!asteroid.Frozen)
                {
                    asteroid.ApplyMotion(deltaTime, new Vector3(0f, 0f, -shipSpeed));
                }
            }
        }

        void CheckEndGame()
        {
            if (_asteroids[_asteroids.Length - 1].Position.Value.z < 0f)
            {
                MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.GAME_OVER, true));
            }
        }

        void ApplyAsteroidData()
        {
            MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.APPLY_ASTEROID_DATA, _asteroids));
        }
    }
}