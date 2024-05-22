using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using TMPro;

using ForChatGPT;

public class SystemController : MonoBehaviour
{
    //変数定義
    //メッセージ管理
    [System.Serializable]
    public class Savedata
    {
        public string name;
        public string history; //追加
        public string conversation;
        public string backgroundPrompt;
        public bool isClickable;//クリックしてユーザのターンに変更できるか
        //削除
        //public List<ChatGPTMessageModel> rootMessageList;
        public List<ChatGPTMessageModel> messageList;
        public int emotion;
        public string worldConfig;

    }
    public List<ChatGPTMessageModel> forTransition;
    public List<ChatGPTMessageModel> forSummary;
    public List<ChatGPTMessageModel> forBackground = new List<ChatGPTMessageModel>
    {
        new ChatGPTMessageModel
        {
            role = "system",
            content = "You are a graphic designer. Output a prompt to create an background image suitable for given scene in visual novel. Be animeish. Please Use JSON style. ex) {\"prompt\": \"some words\"}"
        }
    };
    
    public Savedata data;
    [SerializeField] public GameObject userPanel;
    [SerializeField] public GameObject systemPanel;
    [SerializeField] public GameObject Anmaku;
    [SerializeField] public TMP_InputField inputWindow;
    [SerializeField] public TextMeshProUGUI uiText; // UIテキストコンポーネントへの参照
    public static SystemController instance;

    //シングルトン(このclassのinstanceを単一にすることで呼び出しを省略する)
    async void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        //会話の初期化
        data.name = "ほのか";
        //ペルソナデータの読み込み
        string personaPath = "./Assets/PersonaTemplate/Honoka.txt";
        ReadPersona(personaPath);
        //クリックフラグのセット
        SetUnclickable();
        //削除
        //data.rootMessageList.Add(new ChatGPTMessageModel(){role = "system", content = "Early morning, " + name + " and User start their communall living from now."});
        //conversation→history
        data.history = "Early morning, " + name + " and User start their communall living from now.\n";
        data.messageList.Add(new ChatGPTMessageModel(){role = "assistant", content = "Here is our history.\n" + data.history});
        //data.messageList = data.rootMessageList;
        forTransition.Add(new ChatGPTMessageModel(){role = "system", content = "You are a video game scenario writer. Consider whether to continue the conversation in the current scene or move on to the next scene. The transition of scenes is defined as the change in the location where the conversation takes place. The current scene: '"+data.backgroundPrompt+"' Also, user must speak at least one time. Additionally, the transition of scenes can also be achieved by detecting stagnation in the conversation. Please respond in JSON format, considering this definition. ex) {\"judge\": \"false\"}"});
        forSummary.Add(new ChatGPTMessageModel(){role = "system", content = "You are a popular light novel writer. Please summarize the given scene concisely. Also, think of an introduction to the next scene that follows. The two is always together, not apart. This conversation transitions in the order of waking up, breakfast, morning, lunch, afternoon, dinner, night. One happening that reveals "+data.name+"'s lovely aspect is needed. Out put with JSON style in Japanese. Represent 'User' by the word 'あなた'. \nWorld config: " + data.worldConfig + "\nex){\"summary\": \"説明\", \"nextscene\": \"説明\"}"});
        UnityEngine.Debug.Log("ここまで進んだよ");
        UnityEngine.Debug.Log("SystemController initialized.");
        data.worldConfig = "2000s in japan, where extraordinary power does not exist. A clone of the real world.";
    }
    async void Start()
    {
        //画像プロンプトの初期化
        forBackground.Add(new ChatGPTMessageModel(){role = "user", content = "World config: " + data.worldConfig + "\n next scene: Early morning, " + name + " and User start their communall living from now. They are at home, living room."});
        string resPrompt = await ChatHandler.instance.RequestChatGPT(forBackground);
        forBackground.RemoveAt(1);
        BackgroundPromptModel bgPrompt = JsonUtility.FromJson<BackgroundPromptModel>(resPrompt);
        data.backgroundPrompt = bgPrompt.prompt;
        await BackgroundGenerator.instance.RequestDALLE("japanese anime, " + data.backgroundPrompt);
        ImageHandler.instance.ChangeBackground();
        await AnmakuHandler.instance.FadeOutAsync();
        Anmaku.SetActive(false);
    }
    


    //変数管理のメソッド群
    //ペルソナを読み込む
    public void ReadPersona(string personaPath)
    {
        try
        {
            string persona = File.ReadAllText(personaPath);
            string gameInstruction = File.ReadAllText("./Assets/Prompt/forgame.txt");
            //rootMessageListから変更
            data.messageList = new List<ChatGPTMessageModel>
            {
                new ChatGPTMessageModel
                {
                    role = "system",
                    content = persona + "\n" + gameInstruction
                }
            };
        }
        catch(Exception ex)
        {
            UnityEngine.Debug.Log("Failed to read persona data.");
        }
    }



    public void SetClickable()
    {
        data.isClickable = true;
    }

    public void SetUnclickable()
    {
        data.isClickable = false;
    }


    //ユーザ，システムの切り替えに関するメソッド群
    public void SystemToUser()
    {
        SetUnclickable();
        systemPanel.SetActive(false);
        userPanel.SetActive(true);
        uiText.text = "";
        UnityEngine.Debug.Log("User turn.");
    }

    public void UserToSystem()
    {
        userPanel.SetActive(false);
        systemPanel.SetActive(true);

        UnityEngine.Debug.Log("System turn.");
    }

    //キャラクターの出力が完了し，クリック可能なタイミングのみユーザのターンに変更する
    public void ClickAndChange()
    {
        Debug.Log("現在の背景操作: " + data.isClickable);
        if(data.isClickable)
        {
            SystemToUser();
            SetUnclickable();
        }
    }
    public int Representation(CharacterReactionModel.Emotion res)
    {
        int max = 5;
        //0: normal, 1: happy, 2: anger, 3:sad, 4: joy
        int ans = 0;
        if(res.happy > max)
        {
            max = res.happy;
            ans = 1;
        }
        if(res.anger > max)
        {
            max = res.anger;
            ans = 2;
        }
        if(res.sad > max)
        {
            max = res.sad;
            ans = 3;
        }
        if(res.joy > max)
        {
            max = res.joy;
            ans = 4;
        }
        return ans;
    }


    //シーンの遷移に関わるメソッド群
    public async UniTask SceneTransition()
    {
        Anmaku.SetActive(true);
        AnmakuHandler.instance.FadeInAsync();
        forSummary.Add(new ChatGPTMessageModel(){role = "user", content = data.conversation});
        string resSummary = await ChatHandler.instance.RequestChatGPT(forSummary);
        forSummary.RemoveAt(1);
        ScenarioProgressModel summary = JsonUtility.FromJson<ScenarioProgressModel>(resSummary);
        UnityEngine.Debug.Log("summary: " + summary.summary);
        UnityEngine.Debug.Log("nextscene: " + summary.nextscene);
        /*
        data.rootMessageList.Add(new ChatGPTMessageModel(){role = "assistant", content = summary.summary});
        data.rootMessageList.Add(new ChatGPTMessageModel(){role = "assistant", content = summary.nextscene});
        */
        data.history += summary.summary + "\n";
        data.conversation = summary.nextscene + "\n";

        forBackground.Add(new ChatGPTMessageModel(){role = "user", content = "World config: " + data.worldConfig + "\n next scene: " + summary.nextscene});
        string resPrompt = await ChatHandler.instance.RequestChatGPT(forBackground);
        forBackground.RemoveAt(1);
        BackgroundPromptModel bgPrompt = JsonUtility.FromJson<BackgroundPromptModel>(resPrompt);
        data.backgroundPrompt = bgPrompt.prompt;

        await BackgroundGenerator.instance.RequestDALLE("DO NOT PAINT HUMANS. ONLY LANDSCAPE. japanese anime, " + data.backgroundPrompt);
        ImageHandler.instance.ChangeBackground();
        uiText.text = "";

        string personaPath = "./Assets/PersonaTemplate/Honoka.txt";
        ReadPersona(personaPath);
        data.messageList.Add(new ChatGPTMessageModel(){role = "assistant", content = "Here is our history.\n" + data.history});

        AnmakuHandler.instance.FadeOutAsync();
        Anmaku.SetActive(false);
        await TextTyper.instance.StartTyping(summary.nextscene);
        SetClickable();

    }
    

    public async UniTask CharacterAction(string response)
    {
        CharacterReactionModel reaction = JsonUtility.FromJson<CharacterReactionModel>(response);
        UnityEngine.Debug.Log("character: " + reaction.quote);
        //軽微な変更
        data.conversation += reaction.quote + "\n";
        data.emotion = Representation(reaction.emotion);
        UserToSystem();
        // 入力フィールドをリセットする場合
        inputWindow.text = "";
        //ここらで表情変化
        ImageHandler.instance.ChangeAvatar(data.emotion);
        //後で消す
        forTransition.Add(new ChatGPTMessageModel(){role = "user", content = data.conversation});
        string resTransition = await ChatHandler.instance.RequestChatGPT(forTransition);
        forTransition.RemoveAt(1);
        await TextTyper.instance.StartTyping(reaction.quote);
        SetClickable();
        UnityEngine.Debug.Log("タッチできます");
        JudgeModel judge = JsonUtility.FromJson<JudgeModel>(resTransition);
        UnityEngine.Debug.Log("judge: " + judge.judge);
        if(judge.judge == "true")
        {
            await SceneTransition();
        }
    }




    //ユーザの入力を受け取ってセリフを出力する
    public async UniTask FromKeyboard(string userInput)
    {
        UnityEngine.Debug.Log("user_input: " + userInput);
        //軽微な変更
        data.conversation += userInput + "\n";
        //新たな入力を会話履歴に追加
        data.messageList.Add(new ChatGPTMessageModel(){role = "user", content = data.conversation});

        string response = await ChatHandler.instance.RequestChatGPT(data.messageList);
        data.messageList.RemoveAt(2);

        /*
        UserToSystem();
        CharacterReactionModel reaction = JsonUtility.FromJson<CharacterReactionModel>(response);
        await TextTyper.instance.StartTyping(reaction.quote);
        SetClickable();
        */

        await CharacterAction(response);
    }
}