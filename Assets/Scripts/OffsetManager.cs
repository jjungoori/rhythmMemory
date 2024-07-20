using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OffsetManager : MonoBehaviour
{
    public float offset = 0.01f;
    private List<float> noteTimings;
    private List<float> offsets = new List<float>();
    private AudioSource musicSource;
    private int index;

    private float prevOffset = -999;

    public GameObject wtfObject;
    private bool corWorking = false;
    public double startTime;

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
        index = 0; // Index 초기화
        
        startTime = AudioSettings.dspTime;
        musicSource.PlayScheduled(startTime);
    }

    public List<float> GenerateNoteTimingsByBPM(int noteCount, float bpm, float offset)
    {
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
            hitObject.GetComponent<Cube>().clickObject(true);
            return hitObject;
        }

        return null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("MenuScene");
        }

        if (index < noteTimings.Count && GetCurrentTime() >= noteTimings[index] + (0.6f) / 2)
        {
            index++;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // 클릭한 시간과 노트 타이밍 사이의 차이 계산
            var curObject = castObject();
            if (curObject != null)
            {
                if (index >= noteTimings.Count)
                {
                    Debug.LogWarning("Index out of range");
                    return;
                }

                float oft = GetCurrentTime() - noteTimings[index]; // 얼마나 느리게 쳤는가
                Debug.Log(oft);
                offsets.Add(oft);

                if (prevOffset > -800 && Mathf.Abs(prevOffset - oft) > 0.1f)
                {
                    StartCoroutine(wtf());
                }

                prevOffset = oft;

                if (offsets.Count >= 20)
                {
                    float sum = offsets.Sum();
                    float average = sum / offsets.Count;

                    // 편차 계산을 위해 분산을 구함
                    float variance = offsets.Select(o => Mathf.Pow(o - average, 2)).Sum() / offsets.Count;
                    float standardDeviation = Mathf.Sqrt(variance);

                    // 3 시그마 규칙을 적용하여 이상치를 제외한 값만 필터링
                    List<float> filteredOffsets = offsets.Where(o => Mathf.Abs(o - average) <= 1.5 * standardDeviation).ToList();

                    if (filteredOffsets.Count > 0)
                    {
                        float filteredSum = filteredOffsets.Sum();
                        float filteredAverage = filteredSum / filteredOffsets.Count;
                        Debug.Log("Filtered Average: " + filteredAverage);
                        filteredAverage = Mathf.Max(filteredAverage, 0);

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

    private float GetCurrentTime()
    {
        // Use AudioSettings.dspTime for more accurate timing
        return (float)(AudioSettings.dspTime - startTime);
    }
}
