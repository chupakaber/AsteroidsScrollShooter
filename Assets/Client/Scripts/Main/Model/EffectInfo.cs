using UnityEngine;

namespace Client.Main
{
    public class EffectInfo : ISceneObjectInfo
    {
        public float EndTime = 0f;

        public override void ApplyMotion(float deltaTime, Vector3 additionalLinearVelocity)
        {
            Position.Value = Position.Value + additionalLinearVelocity * deltaTime;
        }
    }
}