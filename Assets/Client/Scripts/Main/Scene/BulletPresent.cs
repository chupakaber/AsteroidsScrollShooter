using UnityEngine;
using Client.Common;
using UniRx;

namespace Client.Main
{
    public sealed class BulletPresent : MonoBehaviour
    {
        [SerializeField] GameObject _bulletPrefab = default;

        GameObjectPool<Transform> _bulletPool = null;
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
            _bulletPool = new GameObjectPool<Transform>(_bulletPrefab.transform);

            MessageBroker.Default
            .Receive<SharedMessage>()
            .Where(msg => msg.Type == SharedMessage.MessageType.SHOW_BULLET)
            .Subscribe(msg => {
                var bulletInfo = (BulletInfo) msg.Context;
                ShowBullet(bulletInfo);
            }).AddTo(_disposables);
        }

        void OnDisable()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
        }

        void ShowBullet(ISceneObjectInfo bullet)
        {
            var bulletObject = _bulletPool.Get(null);
            bulletObject.gameObject.SetActive(true);
            bullet.Position.ObserveEveryValueChanged(value => value.Value)
            .Subscribe(value => {
                bulletObject.position = value;
            }).AddTo(_disposables);
            Observable.Timer (System.TimeSpan.FromMilliseconds(3000))
            .Subscribe (_ => {
                bulletObject.gameObject.SetActive(false);
                _bulletPool.Put(bulletObject);
            }).AddTo(_disposables);
        }
    }
}
