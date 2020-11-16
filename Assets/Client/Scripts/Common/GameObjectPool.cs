using System.Collections.Generic;
using UnityEngine;

namespace Client.Common
{
    public class GameObjectPool<T> where T : Component
    {
        T _prefab;
        Queue<T> _storedObjects = null;

        public GameObjectPool(T prefab)
        {
            _prefab = prefab;
            _storedObjects = new Queue<T>();
        }

        public T Get(Transform container)
        {
            if (_storedObjects.Count > 0)
            {
                var obj = _storedObjects.Dequeue();
                obj.transform.SetParent(container);
                return obj;
            }
            
            return GameObject.Instantiate<T>(_prefab, container);
        }

        public void Put(T obj)
        {
            _storedObjects.Enqueue(obj);
        }
    }
}