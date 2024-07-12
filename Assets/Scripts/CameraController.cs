using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera; // Unity Inspector에서 메인 카메라를 할당하세요.
    public Vector3[] targetPositions; // 카메라가 바라볼 좌표 배열
    private Vector3 targetCameraPosition;
    public float extraSpaceFactor = 1.2f; // 바운딩 박스에 추가할 공간 비율
    public float minSize = 5f; // 최소 크기 설정

    private void Update()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, 0.01f);
    }

    void Start()
    {
    }

    public void AdjustCameraPosition()
    {
        if (targetPositions.Length == 0)
        {
            Debug.LogWarning("No target positions provided.");
            return;
        }

        // 모든 좌표를 포함하는 최소 박스를 구함
        Bounds bounds = CalculateBounds(targetPositions);

        // 바운딩 박스에 여유 공간 추가
        bounds.Expand(bounds.size * (extraSpaceFactor - 1.0f));

        // 최소 크기 적용
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        if (maxSize < minSize)
        {
            bounds.Expand(minSize - maxSize);
        }

        // 카메라 위치를 박스의 중심으로 이동
        targetCameraPosition = bounds.center - mainCamera.transform.forward * Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        // 카메라 위치 변경
        // mainCamera.transform.position = targetCameraPosition;
    }

    Bounds CalculateBounds(Vector3[] positions)
    {
        if (positions.Length == 0)
        {
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        Vector3 min = positions[0];
        Vector3 max = positions[0];

        foreach (Vector3 pos in positions)
        {
            min = Vector3.Min(min, pos);
            max = Vector3.Max(max, pos);
        }

        Vector3 size = max - min;
        Vector3 center = min + size * 0.5f;

        return new Bounds(center, size);
    }
}