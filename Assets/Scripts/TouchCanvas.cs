using UnityEngine;
using UnityEngine.EventSystems; // EventSystemを使用するため

public class TouchCanvas : MonoBehaviour, IPointerClickHandler
{
    // Canvasや他のUI要素がクリックされたときに呼ばれる
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Canvas was clicked.");
        // ここで任意の関数を呼び出す
        SystemController.instance.SystemToUser();
    }
}