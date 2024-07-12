using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;


public class Item : MonoBehaviour
{
    public bool isMouseOn = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MouseOn()
    {
        if (!isMouseOn)
        {
            // GetComponent<>()
            GetComponent<DOTweenAnimation>().DORestartById("mouseOn");
            foreach (DOTweenAnimation i in transform.GetComponentsInChildren<DOTweenAnimation>())
            {
                i.DORestart();
            }
            isMouseOn = true;
        }
    }
    
    public void MouseOut()
    {
        Debug.Log("unselected");
        if (isMouseOn)
        {
            // GetComponent<DOTweenAnimation>().DORestartById("mouseOut");
            GetComponent<DOTweenAnimation>().DOPlayBackwardsById("mouseOn");
            foreach (DOTweenAnimation i in transform.GetComponentsInChildren<DOTweenAnimation>())
            {
                i.DOPlayBackwards();
            }

            isMouseOn = false;
        }
    }

    public void Selected()
    {
        GetComponent<DOTweenAnimation>().DORestartById("selected");


        StartCoroutine(fadeOut());
        StartCoroutine(LoadSampleSceneAfterDelay(0.5f));
    }

    IEnumerator fadeOut()
    {
        PostProcessVolume volume = GameObject.Find("Post-process Volume").GetComponent<PostProcessVolume>();
        Debug.Log("lensD"+volume);
        volume.profile.TryGetSettings(out LensDistortion lensDistortion);
        volume.profile.TryGetSettings(out Bloom bloom);
        
        
        while(lensDistortion.intensity.value > -100)
        {
            // Debug.Log("lensD"+lensDistortion.intensity.value);
            // lensDistortion.intensity.value -= 1f;
            // lensDistortion.intensity.value *= 1.2f;
            //
            // lensDistortion.scale.value *= 0.8f;

            bloom.intensity.value += 1f;
            bloom.intensity.value *= 1.2f;
            bloom.diffusion.value *= 1.1f;
            yield return new WaitForSeconds(0.01f);
        }

    }
    
    IEnumerator LoadSampleSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("SampleScene");
    }
}
