namespace GameResources.Scripts.SaveLoadSystem
{
    using System.IO;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using UnityEngine;
    using UnityEngine.Networking;

    public sealed class SaveLoadSystem : ISaveLoadSystem
    {
        private const string SAVE_CATALOG = "Saves/{0}/{1}";
        private const string STREAMING_CONFIGS = "Configs";

        private string Path { get; set; }

        public bool TryLoadData<T>(out T data)
        {
            data = default;
            return TryLoadData(string.Empty, out data);
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

        public bool TryLoadData<T>(out T data, string fileName = null)
        {
            data = default;
            Debug.LogError("Synchronous TryLoadData is not supported for StreamingAssets. Use async TryLoadDataAsync instead.");
            return false;
        }

        public async UniTask<(bool success, T data)> TryLoadDataAsync<T>(string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = typeof(T).Name + ".json";

            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, STREAMING_CONFIGS, fileName);
            
#if UNITY_WEBGL && !UNITY_EDITOR
            string url = filePath;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"Config file not found: {filePath}");
                    return (false, default);
                }

                try
                {
                    string json = request.downloadHandler.text;
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        Converters = new JsonConverter[] { new StringEnumConverter() }
                    };
                    T data = JsonConvert.DeserializeObject<T>(json, settings);
                    return (data != null, data);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load config {fileName}: {e.Message}");
                    return (false, default);
                }
            }
#else
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"Config file not found: {filePath}");
                return (false, default);
            }

            try
            {
                string json = File.ReadAllText(filePath);
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Converters = new JsonConverter[] { new StringEnumConverter() }
                };
                T data = JsonConvert.DeserializeObject<T>(json, settings);
                return (data != null, data);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load config {fileName}: {e.Message}");
                return (false, default);
            }
#endif
        }

        public void SaveData<T>(string data)
        {
            Path = string.Format(SAVE_CATALOG, typeof(T).Name, string.Empty);
            Save(data);
#if UNITY_EDITOR
            Debug.Log($"Data: {data}");
#endif
        }

        public void SaveData<T>(T data) => SaveData(string.Empty, data);

        public void SaveData<T>(string id, T data)
        {
            Path = string.Format(SAVE_CATALOG, typeof(T).Name, id);
            string json = JsonUtility.ToJson(data);
            Save(json);
#if UNITY_EDITOR
            Debug.Log($"Data: {json}");
#endif
        }

        public void SaveData<T>(T data, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = typeof(T).Name + ".json";

            string folderPath = System.IO.Path.Combine(Application.streamingAssetsPath, STREAMING_CONFIGS);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = System.IO.Path.Combine(folderPath, fileName);
            
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Converters = new JsonConverter[] { new StringEnumConverter() },
                Formatting = Formatting.Indented
            };
            
            string json = JsonConvert.SerializeObject(data, settings);
            File.WriteAllText(filePath, json);
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
        public bool TryLoadData<T>(out T data, string fileName = null);
        public UniTask<(bool success, T data)> TryLoadDataAsync<T>(string fileName = null);
        
        public void SaveData<T>(string data);
        public void SaveData<T>(T data);
        public void SaveData<T>(string id, T data);
        public void SaveData<T>(T data, string fileName = null);
    }
}