using UnityEngine;

namespace Client.Main
{
    public class SharedMessage
    {
        public enum MessageType
        {
            INIT_PLAYER,
            INIT_LEVEL,
            INIT_ASTEROIDS,
            INIT_SHIP,
            MOVE_SHIP,
            SHOOT,
            CREATE_EXPLOSION,
            SHOW_EXPLOSION,
            CREATE_BULLET,
            SHOW_BULLET,
            SHIP_COLLIDED,
            GAME_OVER,
            APPLY_ASTEROID_DATA
        }

        public MonoBehaviour Sender { get; private set; }
        public MessageType Type { get; private set; }
        public System.Object Context { get; private set; }

        public SharedMessage(MonoBehaviour sender, MessageType type, System.Object context)
        {
            this.Sender = sender;
            this.Type = type;
            this.Context = context;
        }
        
        public static SharedMessage Create(MonoBehaviour sender, MessageType type, System.Object context) 
        {
            return new SharedMessage(sender, type, context);
        }
    }
}