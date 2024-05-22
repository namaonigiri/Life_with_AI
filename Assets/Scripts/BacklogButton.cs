using UnityEngine;
using UnityEngine.UI; // UIコンポーネントの使用
using UnityEngine.EventSystems;

public class BacklogButton : MonoBehaviour
{
    // Canvasや他のUI要素がクリックされたときに呼ばれる
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Backlog Button was clicked.");

    }
}