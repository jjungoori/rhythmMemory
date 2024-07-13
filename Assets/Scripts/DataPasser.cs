using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPasser : MonoBehaviour
{
    private GameData gameData;
    
    public void SetGameData(GameData data)
    {
        gameData = data;
    }
    // Start is called before the first frame update
    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            GameObject gameManagerObject = GameObject.Find("GameManager");
            if (gameManagerObject != null)
            {
                GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
                if (gameManager != null)
                {
                    gameManager.SetData(gameData);
                }
                else
                {
                    Debug.LogError("GameManager component not found on GameManager object.");
                }
            }
            else
            {
                Debug.LogError("GameManager object not found in the scene.");
            }
        }
    }

}
