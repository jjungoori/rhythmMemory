using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float offset;
    [SerializeField]
    private GameObject _cubesParent;
    [SerializeField]
    private Camera _mainCamera;
    public List<float> noteTimings;
    private int noteIndex = 0;
    
    public AudioSource musicSource;
    private float startTime;
    private List<GameObject> gameObjects;
    
    [SerializeField]
    private TextMeshProUGUI testText;
    [SerializeField]
    private GameObject cubePrefab;
    
    private float clickAccuracy = 0.3f;
    private float clickDetection = 0.5f;
    private int cubeCount = 4;

    private float health = 1;

    [SerializeField] private GameObject menu;

    [SerializeField] private GameObject main;
    // Start is called before the first frame update
    
    bool IsCoroutineRunning(IEnumerator coroutine)
    {
        return coroutine != null && coroutine.MoveNext();
    }

    void reduceHealth()
    {
        health -= 0.1f;
        GameObject.Find("HPBar").GetComponent<HealthBar>().healthPercent = health;
        if(health <= 0)
        {
            endGame();
        }
    }
    
    


    private void GameStart()
    {
        
        // Debug.Log("Calculated BPM: " + calculatedBPM);
        // Debug.Log("length: " + musicSource.clip.length);

        // Generate note timings
        noteTimings = gameObject.AddComponent<NoteGenerator>().GenerateNote(musicSource, offset);



        // 필요한 데이터 초기화 및 준비 작업
        // gameObjects = GetRandomChildren(cubeCount, _cubesParent.transform);
        // noteTimings = GenerateNoteTimings(10, beatInterval); // 주석 처리된 코드는 필요에 따라 사용할 수 있습니다.
        musicSource = GetComponent<AudioSource>(); // AudioSource 가져오기
        musicSource.Play(); // 음악 재생

        // 시작 시간 기록
        startTime = Time.time;

        // 코루틴 시작
        StartCoroutine(StartProcessNotes());
    }
    
    IEnumerator StartProcessNotes()
    {
        while (musicSource.isPlaying)
        {
            yield return StartCoroutine(ProcessNotesByCom());
        }
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
        noteIndex+=2;
        if (noteIndex >= noteTimings.Count)
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
        
        GetComponent<CameraController>().targetPositions = placeablePositions.ToArray();
        GetComponent<CameraController>().AdjustCameraPosition();
    }
    
    
    

    

    float getMusicTime()
    {
        return Time.time - startTime;
    }

    private void Awake()
    {
        Application.targetFrameRate = 60; // 60 FPS 고정
    }

    void Start()
    {
        gameObject.AddComponent<PlacePositionGetter>();
        gameObject.AddComponent<CameraController>();
        GetComponent<CameraController>().mainCamera = _mainCamera;
        GameStart();
        var white = GameObject.Find("White").GetComponent<Image>();
        white.color = new Color(1, 1, 1, 1);
        StartCoroutine(fadeIn());
        
    }
    
    /// <summary>
    /// 주어진 개수와 간격으로 가상의 노트 타이밍을 생성하는 함수
    /// </summary>
    /// <param name="noteCount">생성할 노트의 개수</param>
    /// <param name="interval">노트 간의 시간 간격 (초)</param>
    /// <returns>생성된 노트 타이밍 리스트</returns>
    public List<float> GenerateNoteTimings(int noteCount, float interval)
    {
        List<float> timings = new List<float>();

        for (int i = 0; i < noteCount; i++)
        {
            timings.Add(i * interval);
        }

        return timings;
    }
    
    /// <summary>
    /// 주어진 반복 횟수만큼 _cubesParent의 자식 중 하나를 랜덤으로 선택하여 리스트에 저장하고 반환하는 함수
    /// </summary>
    /// <param name="repeatCount">반복 횟수</param>
    /// <param name="_cubesParent">자식들을 포함하는 부모 Transform</param>
    /// <returns>랜덤으로 선택된 자식들의 리스트</returns>
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

    void Update()
    {
        // 마우스 왼쪽 버튼이 클릭되었을 때
        if (Input.GetMouseButtonDown(0))
        {
            
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



    private IEnumerator ProcessNotesByCom()
    {
        spawnCube();
        gameObjects = GetRandomChildren(cubeCount, _cubesParent.transform);
        
        int index = 0;

        if (noteIndex >= noteTimings.Count)
        {
            // Debug.Log("error by noteIndex");
            yield break;
        }

        while (index < gameObjects.Count)
        {
            // 현재 음악 재생 시간 계산
            // float musicTime = musicSource.time;
            var musicTime = getMusicTime();
            
            // 현재 시간이 다음 노트 타이밍을 지났는지 확인
            if (musicTime >= noteTimings[noteIndex])
            {
                // 리스트의 현재 인덱스에 해당하는 게임 오브젝트 처리
                ProcessGameObject(gameObjects[index]);

                // 인덱스 증가
                index++;
                increaseNoteCount();
            }

            // 다음 프레임까지 대기
            yield return null;
        }

        yield return StartCoroutine(ProcessNotesByClick());
    }
    
    private IEnumerator ProcessNotesByClick()
    {
        int index = 0;
        while (index < gameObjects.Count)
        {
            // 현재 음악 재생 시간 계산
            var musicTime = getMusicTime();

            // 현재 시간이 노트 타이밍보다 앞서면 다음 프레임까지 대기
            if (musicTime < noteTimings[noteIndex]-clickDetection)
            {
                yield return null;
                continue;
            }
            float clickTime = musicTime;
            float noteTime = noteTimings[noteIndex];
            float difference = Mathf.Abs(clickTime - noteTime);

            testText.text = difference.ToString();
            // 클릭 감지
            if (Input.GetMouseButtonDown(0))
            {
                // 클릭한 시간과 노트 타이밍 사이의 차이 계산
                
                var curObject = castObject();
                Debug.Log(gameObjects[index]);

                // 오차 범위 내에 클릭이 있으면 처리
                if (difference <= clickAccuracy && gameObjects[index].transform.GetChild(0).gameObject ==  curObject)
                {
                    // Debug.Log("Clicked! Clicked at " + clickTime + ", expected " + noteTime);
                    // ProcessGameObject(gameObjects[index]);
                    increaseNoteCount();
                    index++;
                }
                else
                {
                    fail();
                    // Debug.Log("Missed! Clicked at " + clickTime + ", expected " + noteTime);
                    // 클릭 실패 시 다음 노트로 넘어감
                    increaseNoteCount();                    
                    index++;
                }
            }
            else
            {
                if (musicTime - noteTimings[noteIndex] > clickAccuracy)
                {
                    fail();
                    // Debug.Log("Missed! Didn't Click at " + musicTime + ", expected " + noteTimings[noteIndex]);
                    increaseNoteCount();                    
                    index++;
                }
            }

            yield return null;
        }
    }

    
    /// <summary>
    /// 게임 오브젝트를 처리하는 함수
    /// </summary>
    /// <param name="obj">처리할 게임 오브젝트</param>
    private void ProcessGameObject(GameObject obj)
    {
        // 여기서 원하는 처리를 수행합니다.
        // 예를 들어, 오브젝트를 활성화하거나 특정 동작을 수행할 수 있습니다.
        // obj.SetActive(true);
        // Debug.Log("Processed GameObject: " + obj.name);
        obj.transform.GetChild(0).gameObject.GetComponent<Cube>().clickObject(false);
    }

    void fail()
    {
        // _mainCamera.GetComponent<DOTweenAnimation>().DORestart();
        reduceHealth();
    }

    void endGame()
    {
        menu.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(PichToZero());
        main.GetComponent<DOTweenAnimation>().DORestart();
    }
    
    IEnumerator PichToZero()
    {
        while (GetComponent<AudioSource>().pitch > 0)
        {
            GetComponent<AudioSource>().pitch -= 0.05f;
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
