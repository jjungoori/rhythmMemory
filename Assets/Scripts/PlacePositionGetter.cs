using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

public class PlacePositionGetter : MonoBehaviour
{
    public List<Vector3> placeablePositions = new List<Vector3>();
    
    public List<Vector3> getPlacePositions(List<Transform> cubes)
    {
        placeablePositions.Clear();
        foreach (Transform cube in cubes)
        {
            float distance = 1f;
            Vector3 origin = cube.position;
            List<Vector3> emptyPositions = new List<Vector3>();

            // 오른쪽 방향 레이케스트
            if (!Physics.Raycast(origin, Vector3.right, distance))
            {
                emptyPositions.Add(origin + Vector3.right * distance);
            }

            // 왼쪽 방향 레이케스트
            if (!Physics.Raycast(origin, Vector3.left, distance))
            {
                emptyPositions.Add(origin + Vector3.left * distance);
            }

            // 위쪽 방향 레이케스트
            if (!Physics.Raycast(origin, Vector3.forward, distance))
            {
                emptyPositions.Add(origin + Vector3.forward * distance);
            }

            // 아래쪽 방향 레이케스트
            if (!Physics.Raycast(origin, Vector3.back, distance))
            {
                emptyPositions.Add(origin + Vector3.back * distance);
            }

            placeablePositions.AddRange(emptyPositions);
        }

        if (placeablePositions.Count == 0)
        {
            return new List<Vector3>(){Vector3.zero};
        }

        return placeablePositions;

    }
}