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
        // 마우스 위치를 2D 좌표계로 변환
        mousePos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.nearClipPlane));
        mousePos.z = 0; // Z 좌표를 0으로 설정하여 2D로 만듦

        // 회전할 방향 계산
        Vector3 rotation = mousePos - transform.position;

        // 회전각도 계산
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        rotZ -= 90;

        // 오브젝트 회전 설정
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
