using System.Collections.Generic;
using UnityEngine;
using Client.Common;
using UniRx;

namespace Client.Main
{
    public sealed class AsteroidsPresent : MonoBehaviour
    {
        [SerializeField] GameObject _asteroidPrefab = default;

        GameObjectPool<Transform> _asteroidsPool = null;
        List<Transform> _activeAsteroids = null;
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
            _asteroidsPool = new GameObjectPool<Transform>(_asteroidPrefab.transform);
            _activeAsteroids = new List<Transform>();

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.APPLY_ASTEROID_DATA)
            .Subscribe(msg => {
                var asteroids = (AsteroidInfo[]) msg.Context;
                ApplyAsteroidData(asteroids);
            }).AddTo(_disposables);
        }

        void OnDisable()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
        }

        void ApplyAsteroidData(AsteroidInfo[] asteroids)
        {
            var i = 0;
            for (var k = 0; k < asteroids.Length; k++)
            {
                var asteroidInfo = asteroids[k];
                if (!asteroidInfo.Frozen)
                {
                    if (_activeAsteroids.Count <= i)
                    {
                        var newAsteroid = _asteroidsPool.Get(null);
                        newAsteroid.gameObject.SetActive(true);
                        _activeAsteroids.Add(newAsteroid);
                    }
                    var asteroid = _activeAsteroids[i];
                    asteroid.transform.position = asteroidInfo.Position.Value;
                    asteroid.transform.rotation = asteroidInfo.Rotation.Value;
                    asteroid.transform.localScale = Vector3.one * asteroidInfo.Scale.Value;
                    i++;
                }
            }
            for (var j = i; j < _activeAsteroids.Count; j++)
            {
                _activeAsteroids[j].gameObject.SetActive(false);
                _asteroidsPool.Put(_activeAsteroids[j]);
            }
            _activeAsteroids.RemoveRange(i, _activeAsteroids.Count - i);
        }
    }
}
