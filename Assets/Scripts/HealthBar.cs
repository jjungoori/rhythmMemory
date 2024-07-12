using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private GameObject indicator;
    [SerializeField]
    private GameObject lateIndicator;
    
    private RectTransform indicatorRectTransform;
    private RectTransform lateIndicatorRectTransform;
    private float maxWidth;
    private float targetWidth;
    public float healthPercent;
    
    // Start is called before the first frame update
    void Start()
    {
        indicatorRectTransform = indicator.GetComponent<RectTransform>();
        lateIndicatorRectTransform = lateIndicator.GetComponent<RectTransform>();
        maxWidth = indicatorRectTransform.rect.width;
    }
    
    float getTargetWidth()
    {
        return maxWidth * (healthPercent+1);
    }

    // Update is called once per frame
    void Update()
    {
        targetWidth = getTargetWidth();
        indicatorRectTransform.offsetMax = Vector2.Lerp(indicatorRectTransform.offsetMax, new
            Vector2(-maxWidth + targetWidth, indicatorRectTransform.offsetMax.y), 0.5f);
        lateIndicatorRectTransform.offsetMax = 
            Vector2.Lerp(lateIndicatorRectTransform.offsetMax, new Vector2(-maxWidth + targetWidth, lateIndicatorRectTransform.offsetMax.y), 0.04f);
    }
}