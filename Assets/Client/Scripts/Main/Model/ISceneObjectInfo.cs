using UniRx;
using UnityEngine;

namespace Client.Main
{
    public abstract class ISceneObjectInfo
    {
        public ReactiveProperty<Vector3> Position { get; protected set; }
        public ReactiveProperty<Quaternion> Rotation { get; protected set; }
        public ReactiveProperty<float> Scale { get; protected set; }
        public ReactiveProperty<ISceneObjectInfo> CollidedWith { get; protected set; }

        public ISceneObjectInfo()
        {
            Position = new ReactiveProperty<Vector3>();
            Rotation = new ReactiveProperty<Quaternion>();
            Scale = new ReactiveProperty<float>();
            CollidedWith = new ReactiveProperty<ISceneObjectInfo>();
        }

        public static bool CheckCollision(ISceneObjectInfo obj1, ISceneObjectInfo obj2, out Vector3 deltaPositionNormalized)
        {
            var deltaPosition = obj1.Position.Value - obj2.Position.Value;
            var deltaMagnitude = deltaPosition.magnitude;
            var intersection = obj1.Scale.Value + obj2.Scale.Value - deltaMagnitude;
            deltaPositionNormalized = deltaPosition.normalized;
            return intersection > 0f;
        }

        public virtual void ApplyMotion(float deltaTime, Vector3 additional)
        {
        }
    }
}