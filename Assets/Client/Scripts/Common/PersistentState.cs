using UnityEngine;

namespace Client.Common
{
    public class PersistentState : MonoBehaviour
    {
        public LevelInfo CurrentLevelInfo = default;

        static PersistentState _instance = null;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public static PersistentState Get()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PersistentState>();
            }
            return _instance;
        }
    }
}