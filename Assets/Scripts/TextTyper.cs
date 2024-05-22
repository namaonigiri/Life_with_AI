using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class TextTyper : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI uiText; // UIテキストコンポーネントへの参照
    public float typingSpeed = 0.1f; // 文字が表示される速度（秒）
    public static TextTyper instance;

    void Awake()
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

        Debug.Log("ITextTyper initialized.");
    }

    // StartTypingメソッドをasyncメソッドとして定義
    public async Task StartTyping(string textToType)
    {
        await TypeText(textToType);
    }

    // TypeTextメソッドをasync Taskメソッドとして定義
    private async Task TypeText(string textToType)
    {
        uiText.text = ""; // テキストをクリア
        foreach (char letter in textToType.ToCharArray())
        {
            uiText.text += letter; // 一文字ずつテキストに追加
            await Task.Delay((int)(typingSpeed * 1000)); // 次の文字を表示する前に少し待機
        }
    }
}