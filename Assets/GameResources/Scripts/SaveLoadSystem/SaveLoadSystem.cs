namespace GameResources.Scripts.SaveLoadSystem
{
    using UnityEngine;

    public sealed class SaveLoadSystem : ISaveLoadSystem
    {
        private const string SAVE_CATALOG = "Saves/{0}/{1}";

        private string Path { get; set; }

        public void SaveData<T>(string data)
        {
            Path = SAVE_CATALOG + typeof(T).Name;
            Save(data);
#if UNITY_EDITOR
            Debug.Log($"Data: {data}");
#endif
        }

        public void SaveData<T>(T data) => SaveData(string.Empty, data);

        public void SaveData<T>(string id, T data)
        {
            Path = string.Format(SAVE_CATALOG, typeof(T).Name, id);
            Save(JsonUtility.ToJson(data));
#if UNITY_EDITOR
            string json = JsonUtility.ToJson(data);
            Debug.Log($"Data: {json}");
#endif
        }

        public bool TryLoadData<T>(out T data)
        {
            data = default;
            if (TryLoadData(string.Empty, out T loaddata))
            {
                data = loaddata;
                return true;
            }

            return false;
        }

        public bool TryLoadData<T>(string id, out T data)
        {
            data = default;
            Path = string.Format(SAVE_CATALOG, typeof(T).Name, id);
#if UNITY_EDITOR
            Debug.Log($"Data: {Load()}");
#endif
            data = JsonUtility.FromJson<T>(Load());
            if (data == null)
            {
                Debug.Log("Data not loaded");
            }

            return data != null;
        }

        private void Save(string value)
        {
            PlayerPrefs.SetString(Path, value);
            PlayerPrefs.Save();
        }

        private string Load() => PlayerPrefs.GetString(Path, string.Empty);
    }

    public interface ISaveLoadSystem
    {
        public bool TryLoadData<T>(out T data);
        public bool TryLoadData<T>(string id, out T data);
        public void SaveData<T>(string data);
        public void SaveData<T>(T data);
        public void SaveData<T>(string id, T data);
    }
}