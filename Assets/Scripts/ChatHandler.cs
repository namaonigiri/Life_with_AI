//Manage log.
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using ForChatGPT;

public class ChatHandler : MonoBehaviour
{
    //変数定義
    //APIKeyの取得
    //static string apiKeyPath = "./Assets/Key/APIKey.txt";
    private readonly string apiKey = File.ReadAllText("./Assets/Key/APIKey.txt");
    //リクエストの送信先
    private readonly string apiUrl = "https://api.openai.com/v1/chat/completions";
    //
    public static ChatHandler instance;


    [System.Serializable]
    //ChatGPTに送るpayload形式
    public class ChatGPTPayload
    {
        public string model;
        public List<ChatGPTMessageModel> messages;
        public Dictionary<string, string> response_format;
    }

    [System.Serializable]
    //ChatGPTから受信するデータ形式
    public class ChatGPTResponseModel
    {
        public string id;
        public string @object;
        public int created;
        public Choice[] choices;
        public Usage usage;

        [System.Serializable]
        public class Choice
        {
            public int index;
            public ChatGPTMessageModel message;
            public string finish_reason;
        }

        [System.Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }
    }


    //Headerを返す
    private Dictionary<string, string> GetHeaders()
    {
        var headers = new Dictionary<string, string>
        {
            {"Authorization", "Bearer " + apiKey},
            {"Content-type", "application/json"},
            {"X-Slack-No-Retry", "1"}
        };
        return headers;
    }

   
    //payloadを返す
    private ChatGPTPayload GetPayload(List<ChatGPTMessageModel> messageList)
    {
        var payload = new ChatGPTPayload();
        payload.model = "gpt-4-1106-preview";
        payload.messages = messageList;
        payload.response_format = new Dictionary<string, string> {{"type", "json_object"}};
        return payload;
    }


    private UnityWebRequest GetRequest(string jsonPayload)
    {
        var request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonPayload));
        request.downloadHandler = new DownloadHandlerBuffer();
        return request;
    }



    //動作を示すメソッドの定義
    //シーン開始時に実行
    void Awake()
    {
        //シングルトン(このclassのinstanceを単一にすることで呼び出しを省略する)
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public string ExtractJson(string input)
    {
        int braceDepth = 0;
        int firstBraceIndex = -1;

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '{')
            {
                if (braceDepth == 0)
                {
                    firstBraceIndex = i;
                }
                braceDepth++;
            }
            else if (input[i] == '}')
            {
                braceDepth--;
                if (braceDepth == 0 && firstBraceIndex != -1)
                {
                    return input.Substring(firstBraceIndex, i - firstBraceIndex + 1);
                }
            }
        }

        return null; // 有効なJSONオブジェクトが見つからない場合
    }

    //ChatGPTにリクエスト
    public async UniTask<string> RequestChatGPT(List<ChatGPTMessageModel> messageList)
    {

        try{
            //ヘッダなどの作成
            Dictionary<string, string> headers = GetHeaders();
            ChatGPTPayload payload = GetPayload(messageList);
            string jsonPayload = JsonUtility.ToJson(payload);
            UnityWebRequest request = GetRequest(jsonPayload);

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            UnityEngine.Debug.Log("Now communicating");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                // より詳細なエラーログを出力
                UnityEngine.Debug.LogError($"Error: {request.error}, Response: {request.downloadHandler.text}");
                request.Dispose();
                throw new Exception($"WebRequest Error: {request.error}");
            }
            else
            {
                //正常終了
                UnityEngine.Debug.Log("Received");
                //ChatGPTからのレスポンスを取り出す
                var responseString = request.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<ChatGPTResponseModel>(responseString);
                messageList.Add(responseObject.choices[0].message);
                string result = responseObject.choices[0].message.content;
                UnityEngine.Debug.Log("RawResponse: " + result);
                // リクエストを破棄してリソースを開放
                request.Dispose();
                return ExtractJson(result);
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Exception caught: {ex.Message}");
            return "error"; // または適切なエラーハンドリング
        }
    }
    
}
