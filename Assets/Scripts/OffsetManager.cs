using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OffsetManager : MonoBehaviour
{

    // float lastNoteOffset = 0.0f;
    // Start is called before the first frame update
    public float offset;
    private List<float> noteTimings;
    
    private List<float> offsets = new List<float>();
    private AudioSource musicSource;
    private int index;

    private float prevOffset = -999;

    public GameObject wtfObject;
    private bool corWorking = false;

    IEnumerator wtf()
    {
        if (corWorking)
        {
            yield break;
        }
        corWorking = true;
        wtfObject.GetComponent<TextMeshProUGUI>().text = "???";
        wtfObject.GetComponent<DOTweenAnimation>().DORestartById("scale");
        wtfObject.GetComponent<DOTweenAnimation>().DORestartById("rotate");
        yield return new WaitForSeconds(0.5f);
        wtfObject.GetComponent<DOTweenAnimation>().DOPlayBackwardsById("scale");
        wtfObject.GetComponent<DOTweenAnimation>().DOPlayBackwardsById("rotate");
        yield return new WaitForSeconds(0.5f);
        wtfObject.GetComponent<TextMeshProUGUI>().text = "...";
        corWorking = false;
    }
    
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        noteTimings = GenerateNoteTimingsByBPM(100, 100, offset);
    }
    
    public List<float> GenerateNoteTimingsByBPM(int noteCount, float bpm, float offset)
    {
        // bpm *= 2;
        // noteCount *= 2;
        
        List<float> timings = new List<float>();
        float secondsPerBeat = 60f / bpm;
    
        for (int i = 0; i < noteCount; i++)
        {
            timings.Add(i * secondsPerBeat + offset);
        }
    
        return timings;
    }

    IEnumerator WaitAndStart()
    {
        yield return new WaitForSeconds(3.0f);
    }

    GameObject castObject()
    {
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("MenuScene");
        }
        
        if (musicSource.time >= noteTimings[index]+(0.6f)/2)
        {
            index++;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            // 클릭한 시간과 노트 타이밍 사이의 차이 계산
                
            var curObject = castObject();
            if (curObject != null)
            {
                float oft = musicSource.time - noteTimings[index]; //얼마나 느리게 쳤는가
                Debug.Log(oft);
                offsets.Add(oft);

                if (prevOffset > -800 && Mathf.Abs(prevOffset - oft) > 0.1f)
                {
                    StartCoroutine(wtf());
                }

                prevOffset = oft;


                if (offsets.Count >= 20)
                {
                    float sum = 0;
                    foreach (var o in offsets)
                    {
                        sum += o;
                    }
                    float average = sum / offsets.Count;

                    // 편차 계산을 위해 분산을 구함
                    float variance = 0;
                    foreach (var o in offsets)
                    {
                        variance += Mathf.Pow(o - average, 2);
                    }
                    variance /= offsets.Count;
                    float standardDeviation = Mathf.Sqrt(variance);

                    // 3 시그마 규칙을 적용하여 이상치를 제외한 값만 필터링
                    List<float> filteredOffsets = offsets.Where(o => Mathf.Abs(o - average) <= 1.5 * standardDeviation).ToList();

                    if (filteredOffsets.Count > 0)
                    {
                        float filteredSum = 0;
                        foreach (var o in filteredOffsets)
                        {
                            filteredSum += o;
                        }
                        float filteredAverage = filteredSum / filteredOffsets.Count;
                        Debug.Log("Filtered Average: " + filteredAverage);
                        filteredAverage = filteredAverage < 0 ? 0 : filteredAverage;

                        GameObject.Find("DataPasser").GetComponent<DataPasser>().deviceOffset = filteredAverage;
                    }
                    else
                    {
                        Debug.LogWarning("All values were considered outliers and filtered out.");
                    }

                    SceneManager.LoadScene("MenuScene");
                }
            }


        }
    }
}
