using UnityEngine;
using TMPro;

public class FaceCamera : MonoBehaviour
{
    public Camera mainCamera; // 바라볼 카메라를 설정

    void Start()
    {
        // mainCamera가 설정되지 않았다면 기본 카메라로 설정
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void LateUpdate()
    {
        // 카메라를 바라보도록 회전
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
}