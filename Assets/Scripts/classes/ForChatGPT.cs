namespace ForChatGPT
{
    //クラス定義
    //ChatGPTに送るメッセージ形式
    [System.Serializable]
    public class ChatGPTMessageModel
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class CharacterReactionModel
    {
        public string quote;
        public Emotion emotion;

        [System.Serializable]
        public class Emotion
        {
            public int happy;
            public int anger;
            public int sad;
            public int joy;
        }
    }

    public class JudgeModel
    {
        public string judge;
    }

    public class BackgroundPromptModel
    {
        public string prompt;
    }

    public class ScenarioProgressModel
    {
        public string summary;
        public string nextscene;
    }
    
}