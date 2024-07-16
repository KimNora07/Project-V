//System
using System.Collections;
using System.Collections.Generic;

//UnityEngine
using UnityEngine;
using UnityEngine.UI;

public static class UiUtil
{
    /// <summary>
    /// 한개의 버튼에 한개의 이벤트 추가
    /// </summary>
    /// <param name="button">버튼</param>
    /// <param name="action">이벤트</param>
    public static void SetButtonClick(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
            return;

        button.onClick.AddListener(action);
    }

    /// <summary>
    /// 여러개의 버튼에 한개의 이벤트 추가
    /// </summary>
    /// <param name="buttons">버튼</param>
    /// <param name="action">이벤트</param>
    public static void SetButtonClick(Button[] buttons,  UnityEngine.Events.UnityAction action)
    {
        for(int index = 0; index < buttons.Length; index++)
        {
            if (buttons[index] == null)
                return;

            buttons[index].onClick.AddListener(action);
        }
    }
}
