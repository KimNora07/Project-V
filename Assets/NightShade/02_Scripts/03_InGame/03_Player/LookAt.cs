//System
using System.Collections;
using System.Collections.Generic;

//UnityEngine
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;


    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        // ���콺 ��ġ�� 2D ��ǥ��� ��ȯ
        mousePos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.nearClipPlane));
        mousePos.z = 0; // Z ��ǥ�� 0���� �����Ͽ� 2D�� ����

        // ȸ���� ���� ���
        Vector3 rotation = mousePos - transform.position;

        // ȸ������ ���
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        rotZ -= 90;

        // ������Ʈ ȸ�� ����
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
