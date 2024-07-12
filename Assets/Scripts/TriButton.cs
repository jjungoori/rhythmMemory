using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TriButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<DOTweenAnimation>().DOPlayBackwardsById("mouseIn");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<DOTweenAnimation>().DORestartById("mouseIn");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("MenuScene");
    }
}
