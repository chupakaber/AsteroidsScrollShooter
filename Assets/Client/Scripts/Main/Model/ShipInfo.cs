using UniRx;
using UnityEngine;

namespace Client.Main
{
    public class ShipInfo : ISceneObjectInfo
    {
        public ReactiveProperty<float> WeaponLastShotTime { get; protected set; }

        public float WeaponDamage = 0f;
        public float WeaponShootingFrequency = 0f;
        public int WeaponLastShotPlace = 0;
        public Vector3[] WeaponPlaces = null;
        public float Speed = 0f;
        public float Distance = 0f;
        public bool Active = false;

        public ShipInfo()
        {
            WeaponLastShotTime = new ReactiveProperty<float>();
        }

        public override void ApplyMotion(float deltaTime, Vector3 additionalLinearVelocity)
        {
            Position.Value = Position.Value + additionalLinearVelocity * deltaTime;
        }
    }
}