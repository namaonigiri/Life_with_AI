using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InputHandler : MonoBehaviour, ISubmitHandler
{
    [SerializeField] public TMP_InputField inputWindow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //入力ターン
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                ExecuteEvents.Execute<ISubmitHandler>(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        // 入力完了時にFromKeyboard関数を呼び出す
        SystemController.instance.FromKeyboard(inputWindow.text);

    }
}
