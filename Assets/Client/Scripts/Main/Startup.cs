using UnityEngine;
using UniRx;
using Client.Common;

namespace Client.Main
{
    public class Startup : MonoBehaviour
    {
        const int MaxHitPoints = 3;
        const float Speed = 1f;
        const float WeaponDamage = 1f;
        const float WeaponShootingFrequency = 4f;

        PlayerInfo _playerInfo = null;
        LevelInfo _levelInfo = null;
        AsteroidInfo[] _asteroids = null;
        ShipInfo _shipInfo = null;

        async void Start()
        {
            _playerInfo = new PlayerInfo(MaxHitPoints);

            _shipInfo = new ShipInfo();
            _shipInfo.Speed = Speed;
            _shipInfo.WeaponDamage = WeaponDamage;
            _shipInfo.WeaponPlaces = new Vector3[4] {
                new Vector3(-0.6f, 0f, 0.5f),
                new Vector3(0.6f, 0f, 0.5f),
                new Vector3(-0.35f, 0f, 0.3f),
                new Vector3(0.35f, 0f, 0.3f)
            };
            _shipInfo.WeaponShootingFrequency = WeaponShootingFrequency;
            _shipInfo.Position.Value = new Vector3(0f, 0f, 0f);

            _levelInfo = PersistentState.Get().CurrentLevelInfo;

            LevelGenerator.GenerateAsteroids(_levelInfo, out _asteroids);

            await System.Threading.Tasks.Task.Yield();
            Debug.Log("StartInitializing");
            MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.INIT_PLAYER, _playerInfo));
            Debug.Log("InitPlayer");
            MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.INIT_LEVEL, _levelInfo));
            Debug.Log("InitLevel");
            MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.INIT_ASTEROIDS, _asteroids));
            Debug.Log("InitAsteroids");
            MessageBroker.Default.Publish(SharedMessage.Create(this, SharedMessage.MessageType.INIT_SHIP, _shipInfo));
            Debug.Log("InitShip");
        }
    }
}