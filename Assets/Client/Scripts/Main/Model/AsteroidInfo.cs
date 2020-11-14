using UniRx;
using UnityEngine;

namespace Client.Main
{
    public class AsteroidInfo : ISceneObjectInfo
    {
        public Vector3 LinearVelocity = default;
        public Vector3 AngularVelocity = default;
        public float Durability = 0f;
        public bool Frozen = false;
        CompositeDisposable _disposables = new CompositeDisposable();

        ~AsteroidInfo()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
        }

        public override void ApplyMotion(float deltaTime, Vector3 additionalLinearVelocity)
        {
            Position.Value = Position.Value + (LinearVelocity + additionalLinearVelocity) * deltaTime;
            Rotation.Value = Quaternion.Euler(Rotation.Value.eulerAngles + AngularVelocity * deltaTime);
            CollidedWith.ObserveEveryValueChanged(value => value.Value).Subscribe(OnCollidedWith).AddTo(_disposables);
        }

        public void OnCollidedWith(ISceneObjectInfo collider)
        {
            if (collider != null)
            {
                var deltaPositionNormalized = (collider.Position.Value - Position.Value).normalized;
                var newLinearVelocity = LinearVelocity - deltaPositionNormalized * collider.Scale.Value * 0.01f;
                newLinearVelocity.z = Mathf.Min(0f, newLinearVelocity.z);
                LinearVelocity = newLinearVelocity;
            }
        }
    }
}