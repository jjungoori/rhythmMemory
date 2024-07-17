using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    GameObject lastSelectedObject = null;
    
    public GameObject settingMenu;
    bool isSettingMenuOn = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void triggerSettingMenu()
    {
        isSettingMenuOn = !isSettingMenuOn;
        settingMenu.SetActive(isSettingMenuOn);
    }
    
    public void syncPage()
    {
        SceneManager.LoadScene("OffsetScene");
    }

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            SceneManager.LoadScene("OffsetScene");
            Debug.Log("offset scene");
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 레이가 어떤 오브젝트에 부딪혔는지 확인
        if (!isSettingMenuOn && Physics.Raycast(ray, out hit))
        {
            // 부딪힌 오브젝트의 정보를 얻고, 원하는 작업을 수행
            if (Input.GetMouseButtonDown(0))
            {
                lastSelectedObject.GetComponent<Item>().Selected();
            }
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log(hitObject);
            if ( lastSelectedObject != null && lastSelectedObject != hitObject)
            {
                lastSelectedObject.GetComponent<Item>().MouseOut();
            }
            hitObject.GetComponent<Item>().MouseOn();
            lastSelectedObject = hitObject;
        }
        else
        {
            if (lastSelectedObject != null)
            {
                lastSelectedObject.GetComponent<Item>().MouseOut();
                lastSelectedObject = null;
            }
        }
        
    }
}
