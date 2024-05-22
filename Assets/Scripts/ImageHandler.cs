using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageHandler : MonoBehaviour
{
    [SerializeField] public Sprite normal;
    [SerializeField] public Sprite happy;
    [SerializeField] public Sprite anger;
    [SerializeField] public Sprite sad;
    [SerializeField] public Sprite joy;
    [SerializeField] public Image imageComponent; // エディタからアサインするための公開変数
    [SerializeField] public Image bg;
    public static ImageHandler instance;

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
    }

    public void ChangeBackground()
    {
        string path = "./Assets/Resources/Save/background.png";
        // ResourcesフォルダからTexture2Dとして画像をロード
        byte[] imageData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(1728, 1024);

        if (texture.LoadImage(imageData)) // LoadImageは画像データをテクスチャに変換します
        {
            bg.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    // 画像を変更するための公開メソッド
    public void ChangeAvatar(int emotion)
    {
        if(emotion == 1){
            imageComponent.sprite = happy;
        }
        else if(emotion == 2){
            imageComponent.sprite = anger;
        }
        else if(emotion == 3){
            imageComponent.sprite = sad;
        }
        else if(emotion == 4){
            imageComponent.sprite = joy;
        }
        else if(emotion == 0){
            imageComponent.sprite = normal;
        }
    }
}
