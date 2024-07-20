using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //GameData
    private float offset;
    private float clickAccuracy = 0.3f;
    private float clickDetection = 0.5f;
    private int cubeCount = 4;
    private float health;
    
    private AudioSource musicSource;
    private CameraController camCon;
    
    [SerializeField] private GameObject _cubesParent;
    [SerializeField] private Camera _mainCamera;
    public List<float> noteTimings;
    private int noteIndex = 0;
    
    private double startTime;
    private List<GameObject> gameObjects;
    
    [SerializeField] private TextMeshProUGUI testText;
    [SerializeField] private GameObject cubePrefab;

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject main;

    private List<float> recordedDifferences = new List<float>();

    private bool isProcessingNotesByCom = true;
    private int comIndex = 0;
    private int clickIndex = 0;
    

    void reduceHealth()
    {
        health -= 0.1f;
        GameObject.Find("HPBar").GetComponent<HealthBar>().healthPercent = health;
        if(health <= 0)
        {
            endGame();
        }
    }

    private float GetDeviceAudioLatency()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    try
    {
        using (AndroidJavaObject audioManager = new AndroidJavaObject("android.media.AudioManager"))
        {
            string latencyProperty = audioManager.Call<string>("getProperty", "android.media.property.OUTPUT_LATENCY");
            if (latencyProperty != null)
            {
                return float.Parse(latencyProperty) / 1000f; // milliseconds to seconds
            }
        }
    }
    catch (Exception e)
    {
        Debug.LogWarning("Failed to get audio latency: " + e.Message);
    }
#endif
        return 0f; // Return 0 if not on Android or in case of an error
    }

    
    private void GameStart()
    {
        noteTimings = gameObject.AddComponent<NoteGenerator>().GenerateNote(musicSource, offset);
        musicSource = GetComponent<AudioSource>();
        startTime = AudioSettings.dspTime;
        musicSource.PlayScheduled(startTime);
        
        isProcessingNotesByCom = true;
        comIndex = 0;
        clickIndex = 0;
        gameObjects = GetRandomChildren(cubeCount, _cubesParent.transform);
    }
    

    List<Transform> getCubes()
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < _cubesParent.transform.childCount; i++)
        {
            children.Add(_cubesParent.transform.GetChild(i));
        }

        return children;
    }

    void increaseNoteCount()
    {
        noteIndex+=1; // +=2
        Debug.Log(noteIndex + " and " + noteTimings.Count.ToString());
        if (noteIndex >= noteTimings.Count-1)
        {
            StopAllCoroutines();
            endGame();
            Debug.Log("win!!!");
        }
    }

    void spawnCube()
    {
        var placeablePositions = GetComponent<PlacePositionGetter>().getPlacePositions(getCubes());
        GameObject newCube = Instantiate(cubePrefab, _cubesParent.transform);
        newCube.transform.position = placeablePositions[Random.Range(0, placeablePositions.Count)];
        Debug.Log(newCube.transform.position);
        
        GetComponent<CameraController>().targetPositions = placeablePositions;
        GetComponent<CameraController>().AdjustCameraPosition();
    }

    // float getMusicTime()
    // {
    //     // return musicSource.time;
    //     return (float)AudioSettings.dspTime;
    // }
    //
    private float GetCurrentTime()
    {
        // Use AudioSettings.dspTime for more accurate timing
        float deviceLatency = GetDeviceAudioLatency();
        return (float)(AudioSettings.dspTime - startTime - deviceLatency);
    }
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void SetData(GameData gameData, float deviceOffset)
    {
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }
        
        offset = gameData.offset + deviceOffset;
        musicSource.clip = gameData.musicClip;
        clickAccuracy = gameData.clickAccuracy;
        clickDetection = gameData.clickDetection;
        cubeCount = gameData.cubeCount;
        health = gameData.health;
        noteTimings = new List<float>(gameData.noteTimings);
        
        StartCoroutine(GameStartInTime());
    }
    
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        camCon = GetComponent<CameraController>();
        gameObject.AddComponent<PlacePositionGetter>();
        gameObject.AddComponent<CameraController>();
        GetComponent<CameraController>().mainCamera = _mainCamera;
        var white = GameObject.Find("White").GetComponent<Image>();
        white.color = new Color(1, 1, 1, 1);
        StartCoroutine(fadeIn());
    }
    
    IEnumerator GameStartInTime()
    {
        yield return new WaitForSeconds(2f);
        GameStart();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isProcessingNotesByCom)
        {
            HandleClick();
        }
    }

    GameObject castObject()
    {
        // 카메라에서 마우스 위치로 레이 발사
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 레이가 어떤 오브젝트에 부딪혔는지 확인
        if (Physics.Raycast(ray, out hit))
        {
            // 부딪힌 오브젝트의 정보를 얻고, 원하는 작업을 수행
            GameObject hitObject = hit.collider.gameObject;
            // Debug.Log("Hit Object: " + hitObject.name);
            hitObject.GetComponent<Cube>().clickObject(true);
            // 여기서 추가 작업을 수행할 수 있습니다. 예를 들어:
            // hitObject.GetComponent<Renderer>().material.color = Color.red;
            return hitObject;
        }
        
        return null;
    }

    public List<GameObject> GetRandomChildren(int repeatCount, Transform _cubesParent)
    {
        List<GameObject> selectedChildren = new List<GameObject>();

        // 자식 오브젝트의 수를 얻음
        int childCount = _cubesParent.childCount;

        // 랜덤으로 자식 선택
        for (int i = 0; i < repeatCount; i++)
        {
            // 랜덤 인덱스 생성
            int randomIndex = Random.Range(0, childCount);

            // 랜덤으로 선택된 자식을 리스트에 추가
            Transform selectedChild = _cubesParent.GetChild(randomIndex);
            selectedChildren.Add(selectedChild.gameObject);
        }

        return selectedChildren;
    }
    private void ProcessNotesByCom()
    {
        float musicTime = GetCurrentTime();
        
        if (comIndex < gameObjects.Count && musicTime >= noteTimings[noteIndex])
        {
            ProcessGameObject(gameObjects[comIndex]);
            comIndex++;
            increaseNoteCount();

            if (comIndex >= gameObjects.Count)
            {
                isProcessingNotesByCom = false;
                spawnCube();
                clickIndex = 0;
            }
        }
    }

    private void ProcessNotesByClick()
    {
        float musicTime = GetCurrentTime();

        if (clickIndex >= gameObjects.Count)
        {
            isProcessingNotesByCom = true;
            comIndex = 0;
            gameObjects = GetRandomChildren(cubeCount, _cubesParent.transform);
            return;
        }

        if (musicTime >= noteTimings[noteIndex] + clickAccuracy)
        {
            fail();
            increaseNoteCount();
            clickIndex++;
        }
    }

    private void HandleClick()
    {
        float musicTime = GetCurrentTime();
        float noteTime = noteTimings[noteIndex];
        float difference = Mathf.Abs(musicTime - noteTime);

        var curObject = castObject();
        recordedDifferences.Add(difference / clickDetection);

        if (difference <= clickAccuracy && gameObjects[clickIndex].transform.GetChild(0).gameObject == curObject)
        {
            increaseNoteCount();
            clickIndex++;
        }
        else
        {
            fail();
            increaseNoteCount();
            clickIndex++;
        }
    }

    private void FixedUpdate()
    {
        if (musicSource.isPlaying)
        {
            if (isProcessingNotesByCom)
            {
                ProcessNotesByCom();
            }
            else
            {
                ProcessNotesByClick();
            }
        }
    }


    /// <summary>
    /// 게임 오브젝트를 처리하는 함수
    /// </summary>
    /// <param name="obj">처리할 게임 오브젝트</param>
    private void ProcessGameObject(GameObject obj)
    {
        obj.transform.GetChild(0).gameObject.GetComponent<Cube>().clickObject(false);
    }

    void fail()
    {
        reduceHealth();
    }
    void endGame()
    {
        menu.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(PichToZero());
        menu.GetComponent<ResultMenu>().SetData(recordedDifferences, musicSource);
        main.GetComponent<DOTweenAnimation>().DORestart();
    }
    
    IEnumerator PichToZero()
    {
        while (musicSource.pitch > 0)
        {
            musicSource.pitch -= 0.05f;
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    IEnumerator fadeIn()
    {
        // PostProcessVolume volume = GameObject.Find("Post-process Volume").GetComponent<PostProcessVolume>();
        // Debug.Log("lensD"+volume);
        // volume.profile.TryGetSettings(out LensDistortion lensDistortion);
        // volume.profile.TryGetSettings(out Bloom bloom);
        //
        //
        // while(bloom.intensity.value > 0)
        // {
        //     // Debug.Log("lensD"+lensDistortion.intensity.value);
        //     // lensDistortion.intensity.value -= 1f;
        //     // lensDistortion.intensity.value *= 1.2f;
        //     //
        //     // lensDistortion.scale.value *= 0.8f;
        //
        //     bloom.intensity.value -= 1f;
        //     bloom.intensity.value *= 0.8f;
        //     bloom.diffusion.value *= 0.8f;
        //     yield return new WaitForSeconds(0.01f);
        // }
        var white = GameObject.Find("White").GetComponent<Image>();
        while(white.color.a > 0)
        {
            white.color = new Color(1, 1, 1, white.color.a - 0.01f);
            yield return new WaitForSeconds(0.005f);
        }
        white.gameObject.SetActive(false);
    }
}
