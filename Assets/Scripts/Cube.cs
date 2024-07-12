using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private Material material;
    // Start is called before the first frame update

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    public void clickObject(bool click)
    {
        Debug.Log("김진모");
        GetComponent<DOTweenAnimation>().DORestartById("start");
        if (click)
        {
            Debug.Log("김진모 노랑");
            // GetComponent<DOTweenAnimation>().DORestartById("clickColor");
            material.color = Color.yellow;
            StopAllCoroutines();
            StartCoroutine(unColor());
        }
        else
        {
            Debug.Log("김진모 초록");

            // GetComponent<DOTweenAnimation>().DORestartById("color");
            material.color = Color.green;
            StopAllCoroutines();
            StartCoroutine("unColor");
        }
    }

    IEnumerator unColor()
    {
        while (material.color != Color.white)
        {
            material.color = Color.Lerp(material.color, Color.white, 0.04f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
