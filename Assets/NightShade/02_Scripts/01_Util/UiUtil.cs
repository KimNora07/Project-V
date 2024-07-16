//System
using System.Collections;
using System.Collections.Generic;

//UnityEngine
using UnityEngine;
using UnityEngine.UI;

public static class UiUtil
{
    /// <summary>
    /// �Ѱ��� ��ư�� �Ѱ��� �̺�Ʈ �߰�
    /// </summary>
    /// <param name="button">��ư</param>
    /// <param name="action">�̺�Ʈ</param>
    public static void SetButtonClick(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
            return;

        button.onClick.AddListener(action);
    }

    /// <summary>
    /// �������� ��ư�� �Ѱ��� �̺�Ʈ �߰�
    /// </summary>
    /// <param name="buttons">��ư</param>
    /// <param name="action">�̺�Ʈ</param>
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
