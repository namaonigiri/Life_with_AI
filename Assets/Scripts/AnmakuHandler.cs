using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AnmakuHandler : MonoBehaviour
{
    public static AnmakuHandler instance;
    [SerializeField] public CanvasGroup canvasGroup;
    public float fadeDuration = 2.0f; // フェードインにかかる時間（秒）

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

    public async Task FadeInAsync()
    {
        float endTime = Time.time + fadeDuration;

        while (Time.time < endTime)
        {
            float elapsed = Time.time + fadeDuration - endTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            await Task.Yield(); // 次のフレームまで待機
        }

        canvasGroup.alpha = 1; // 念のため、完全に不透明に設定
    }

    // フェードアウト実行
    public async Task FadeOutAsync()
    {
        float startTime = Time.time;
        float endTime = startTime + fadeDuration;

        while (Time.time <= endTime)
        {
            // 経過時間に基づいてalpha値を計算
            float elapsed = Time.time - startTime;
            canvasGroup.alpha = 1 - Mathf.Clamp01(elapsed / fadeDuration);
            await Task.Yield(); // 次のフレームまで待機
        }

        canvasGroup.alpha = 0; // 完全に透明に設定
    }

}
