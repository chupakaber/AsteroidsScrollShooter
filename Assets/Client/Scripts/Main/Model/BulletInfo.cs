using UniRx;
using UnityEngine;

namespace Client.Main
{
    public class BulletInfo : ISceneObjectInfo
    {
        public float EndTime = 0f;
        public Vector3 LinearVelocity = default;
        
        public override void ApplyMotion(float deltaTime, Vector3 additionalLinearVelocity)
        {
            Position.Value = Position.Value + (LinearVelocity + additionalLinearVelocity) * deltaTime;
        }
    }
}