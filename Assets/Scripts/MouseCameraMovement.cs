using UnityEngine;

public class MouseCameraMovement : MonoBehaviour
{
    public float sensitivity = 10.0f; // 마우스 이동에 따른 카메라 이동 민감도
    public float maxXAngle = 45.0f;   // 카메라가 움직일 수 있는 최대 X축 각도
    public float maxYAngle = 45.0f;   // 카메라가 움직일 수 있는 최대 Y축 각도

    private Vector3 initialPosition;  // 초기 위치 저장
    private Vector3 currentRotation;  // 현재 회전 저장

    private Vector3 offset;

    void Start()
    {

        initialPosition = transform.localPosition; // 초기 위치 설정
        currentRotation = transform.localEulerAngles; // 초기 회전 각도 설정
        
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X"); // 마우스 X축 움직임
        float mouseY = Input.GetAxis("Mouse Y"); // 마우스 Y축 움직임
        
        Debug.Log(mouseX);

        currentRotation.x -= mouseY * sensitivity; // 마우스 Y축 움직임에 따른 회전 변경
        currentRotation.y += mouseX * sensitivity; // 마우스 X축 움직임에 따른 회전 변경

        // 각도를 제한하여 카메라의 회전 범위를 설정
        currentRotation.x = Mathf.Clamp(currentRotation.x, -maxXAngle, maxXAngle);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

        transform.localEulerAngles = currentRotation + offset; // 회전 적용
    }
}