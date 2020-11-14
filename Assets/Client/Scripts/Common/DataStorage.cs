using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Client.Common
{
    public class DataStorage
    {
        public static bool Save<T>(T data)
        {
            var path = GetPath<T>(data);
            var bf = new BinaryFormatter();
            var fs = File.OpenWrite(path);
            if (fs != null && fs.CanWrite)
            {
                bf.Serialize(fs, data);
                fs.Close();
                return true;
            }
            return false;
        }

        public static bool Load<T>(ref T data)
        {
            var path = GetPath<T>(data);
            var bf = new BinaryFormatter();
            if (File.Exists(path))
            {
                var fs = File.OpenRead(path);
                if (fs != null && fs.CanRead)
                {
                    data = (T) bf.Deserialize(fs);
                    fs.Close();
                    return true;
                }
            }
            return false;
        }

        static string GetPath<T>(T data)
        {
            return $"{UnityEngine.Application.temporaryCachePath}/storage.{data.GetType().Name}.dat";
        }
    }
}