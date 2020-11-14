using UnityEngine;
using Client.Common;

namespace Client.Main
{
    public class LevelGenerator
    {
        public static float AsteroidDensityFactor = 2f;
        public static int AsteroidMaxRotationSpeed = 30;
        public static int AsteroidMinForwardLinearVelocity = 0;
        public static int AsteroidMaxForwardLinearVelocity = 95;
        public static int AsteroidMaxSideLinearVelocity = 5;
        public static int AsteroidMinScale = 35;
        public static int AsteroidMaxScale = 60;
        public static int AsteroidHorizontalGenrationRange = 500;

        public static void GenerateAsteroids(LevelInfo levelInfo, out AsteroidInfo[] array)
        {
            System.Random rnd = new System.Random(levelInfo.Seed);
            array = new AsteroidInfo[levelInfo.TotalAsteroids];
            for (var i = 0; i < levelInfo.TotalAsteroids; i++)
            {
                var asteroid = CreateAsteroid(rnd, i);
                array[i] = asteroid;
            }
        }

        static AsteroidInfo CreateAsteroid(System.Random rnd, int index)
        {
            var asteroid = new AsteroidInfo();
            asteroid.Position.Value = new Vector3(rnd.Next(-AsteroidHorizontalGenrationRange, AsteroidHorizontalGenrationRange) * 0.01f, 0f, rnd.Next(0, 100) * 0.1f + index / AsteroidDensityFactor);
            asteroid.Scale.Value = rnd.Next(AsteroidMinScale, AsteroidMaxScale) * 0.01f;
            asteroid.LinearVelocity = new Vector3(rnd.Next(-AsteroidMaxSideLinearVelocity, AsteroidMaxSideLinearVelocity) * 0.01f, 0.0f, rnd.Next(AsteroidMinForwardLinearVelocity, AsteroidMaxForwardLinearVelocity) * -0.01f);
            asteroid.AngularVelocity = new Vector3(rnd.Next(-AsteroidMaxRotationSpeed, AsteroidMaxRotationSpeed), rnd.Next(-AsteroidMaxRotationSpeed, AsteroidMaxRotationSpeed), rnd.Next(-AsteroidMaxRotationSpeed, AsteroidMaxRotationSpeed));
            asteroid.Durability = 5f;
            asteroid.Frozen = true;
            return asteroid;
        }
    }
}