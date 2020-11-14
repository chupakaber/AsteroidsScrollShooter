using System;
using UniRx;
using UnityEngine;

namespace Client.Common
{
    [Serializable]
    public class LevelInfo
    {
        [SerializeField]
        public int LevelID = 0;
        [SerializeField]
        public int Seed = 0;
        [SerializeField]
        public int TotalAsteroids = 0;
    }
}