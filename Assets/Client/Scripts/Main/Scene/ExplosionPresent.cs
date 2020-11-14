using UnityEngine;
using Client.Common;
using UniRx;

namespace Client.Main
{
    public sealed class ExplosionPresent : MonoBehaviour
    {
        [SerializeField] GameObject _explosionPrefab = default;

        GameObjectPool<Transform> _explosionPool = null;
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
            _explosionPool = new GameObjectPool<Transform>(_explosionPrefab);

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.SHOW_EXPLOSION)
            .Subscribe(msg => {
                var effectInfo = (EffectInfo) msg.Context;
                ShowExplosion(effectInfo);
            }).AddTo(_disposables);
        }

        void OnDisable()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
        }

        void ShowExplosion(ISceneObjectInfo explosion)
        {
            var explosionObject = _explosionPool.Get(null);
            explosionObject.gameObject.SetActive(true);
            explosion.Position.ObserveEveryValueChanged(value => value.Value)
            .Subscribe(value => {
                explosionObject.position = value + Vector3.up;
            }).AddTo(_disposables);
            Observable.Timer (System.TimeSpan.FromMilliseconds(500))
            .Subscribe (_ => {
                explosionObject.gameObject.SetActive(false);
                _explosionPool.Put(explosionObject);
            }).AddTo(_disposables);
        }
    }
}
