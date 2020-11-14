using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;

namespace Client.Main
{
    public class ShipControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public const float ShipPositionOffset = 1.5f;

        ShipInfo _shipInfo = null;
        Camera _camera = null;
        bool _drag = false;
        Vector3 _shipTargetPosition = new Vector3();
        CompositeDisposable _disposables = new CompositeDisposable();

        void OnEnable()
        {
            _camera = Camera.main;

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
            if (_drag)
            {
                SmoothPosition();
                Shoot();
            }
        }

        void SmoothPosition()
        {
            var position = Vector3.Lerp(_shipInfo.Position.Value, _shipTargetPosition, Time.deltaTime * 10f);
            MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.MOVE_SHIP, position));
        }

        void Shoot()
        {
            if (_shipInfo.Active && (_shipInfo.WeaponLastShotTime.Value + 1f / _shipInfo.WeaponShootingFrequency < Time.time))
            {
                _shipInfo.WeaponLastShotPlace++;
                if (_shipInfo.WeaponLastShotPlace >= _shipInfo.WeaponPlaces.Length)
                {
                    _shipInfo.WeaponLastShotPlace = 0;
                }
                _shipInfo.WeaponLastShotTime.Value = Time.time;

                MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.SHOOT, null));
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            var ray = _camera.ScreenPointToRay(eventData.position);
            if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out var distance))
            {
                var newPosition = ray.GetPoint(distance);
                newPosition.z += ShipPositionOffset;
                _shipTargetPosition = newPosition;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
            _drag = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _drag = false;
        }
    }
}