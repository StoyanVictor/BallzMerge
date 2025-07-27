using System.IO;
using UnityEngine;
namespace CodeBase
{
    public class SaveLoadDataService<T> where T : IData
    {
        public T LoadData()
        {
            string path = Application.persistentDataPath + $"/{typeof(T).Name}.json";
            if (!File.Exists(path))
                return default;

            var json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        public void SaveData(T data)
        {
            var json = JsonUtility.ToJson(data,true);
            File.WriteAllText(Application.persistentDataPath + $"/{typeof(T).Name}.json",json);
        }
    }
}
[System.Serializable]
public class BestResult : IData
{
    
    public int Result;
    
    public BestResult(int result)
    {
        Result = result;
    }
}
public interface IData
{
}