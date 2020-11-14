using System.Collections.Generic;
using UnityEngine;

namespace Client.Common
{
    public class GameObjectPool<T>
    {
        GameObject _prefab = null;
        Queue<T> _storedObjects = null;

        public GameObjectPool(GameObject prefab)
        {
            _prefab = prefab;
            _storedObjects = new Queue<T>();
        }

        public T Get(Transform container)
        {
            if (_storedObjects.Count > 0)
                return _storedObjects.Dequeue();
            
            var obj = GameObject.Instantiate<GameObject>(_prefab, container).GetComponent<T>();
            return obj;
        }

        public void Put(T obj)
        {
            _storedObjects.Enqueue(obj);
        }
    }
}