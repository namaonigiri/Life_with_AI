using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class BackgroundGenerator : MonoBehaviour
{
    private readonly string apiKey = File.ReadAllText("./Assets/Key/APIKey.txt");
    private readonly string imageApiUrl = "https://api.openai.com/v1/images/generations";
    public static BackgroundGenerator instance;

    /*
    [System.Serializable]
    public class DALLEResponseModel
    {
        public int created;
        public List<Dictionary<string, string>> data;
    }
    */
    [System.Serializable]
    public class DALLEResponseModel
    {
        public int created;
        public List<DataItem> data;
    }

    [System.Serializable]
    public class DataItem
    {
        public string revised_prompt;
        public string url;
    }

    [System.Serializable]
    public class DALLEPayload
    {
        public string model;
        public string prompt;
        public int n;
        public string size;
    }


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            UnityEngine.Debug.Log("BackgroundGenerator Initialized");
        }
        else 
        {
            Destroy(this.gameObject);
        }
    }

    public async UniTask FetchImage(string url)
    {
        UnityEngine.Debug.Log("Image downloading...");
        try
        {
            // HttpClientインスタンスの作成
            using (HttpClient client = new HttpClient())
            {
                // URLから画像をバイト配列として非同期でダウンロード
                byte[] imageBytes = await client.GetByteArrayAsync(url);

                // ここでバイト配列を使用して、画像ファイルを保存するなどの処理を行う
                File.WriteAllBytes("./Assets/Resources/Save/background.png", imageBytes);
                UnityEngine.Debug.Log("Background downloaded");
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error downloading image: " + ex.Message);
        }
    }

    public async UniTask RequestDALLE(string prompt)
    {
        var payload = new DALLEPayload();
        payload.model = "dall-e-3";
        payload.prompt = prompt;
        payload.n = 1;
        payload.size = "1792x1024";
        string jsonPayload = JsonUtility.ToJson(payload);

        var request = new UnityWebRequest(imageApiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        UnityEngine.Debug.Log("Now communicating");
        await request.SendWebRequest().ToUniTask();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image generator: " + request.downloadHandler.text);
            // ここでレスポンスを解析して使用する
            DALLEResponseModel res = JsonUtility.FromJson<DALLEResponseModel>(request.downloadHandler.text);
            if(res.data == null)
            {
                Debug.Log("終わり");
            }
            Debug.Log("url: " + string.Join(", ", res.data));
            string url = res.data[0].url;
            await FetchImage(url);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}
