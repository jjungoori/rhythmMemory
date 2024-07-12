// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
//
// public class BPMCalculator : MonoBehaviour
// {
//     public AudioSource audioSource;
//     private float[] samples = new float[1024];
//     private float[] spectrum = new float[1024];
//     private List<float> bpmReadings = new List<float>();
//     private float[] energyHistory = new float[43];
//     private int energyIndex = 0;
//
//     private const float BEAT_ENERGY_THRESHOLD = 1.5f;
//     private const float MIN_BPM = 60f;
//     private const float MAX_BPM = 200f;
//
//     public float CalculateBPM()
//     {
//         if (audioSource == null || audioSource.clip == null)
//         {
//             Debug.LogError("AudioSource or AudioClip is missing!");
//             return 0f;
//         }
//
//         StartCoroutine(AnalyzeAudio());
//         return 0f; // This will be updated later in the coroutine
//     }
//
//     
//     private IEnumerator AnalyzeAudio()
//     {
//         audioSource.Play();
//         float startTime = Time.time;
//         List<float> beatTimes = new List<float>();
//
//         while (audioSource.isPlaying)
//         {
//             audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
//             
//             float lowFreqEnergy = 0f;
//             for (int i = 1; i < 87; i++) // Focus on bass frequencies (0-86 Hz)
//             {
//                 lowFreqEnergy += spectrum[i];
//             }
//
//             energyHistory[energyIndex] = lowFreqEnergy;
//             energyIndex = (energyIndex + 1) % energyHistory.Length;
//
//             float averageEnergy = energyHistory.Average();
//             if (lowFreqEnergy > averageEnergy * BEAT_ENERGY_THRESHOLD && lowFreqEnergy > 0.01f)
//             {
//                 float currentTime = Time.time - startTime;
//                 if (beatTimes.Count == 0 || currentTime - beatTimes[beatTimes.Count - 1] > 0.2f)
//                 {
//                     beatTimes.Add(currentTime);
//                     Debug.Log($"Beat detected at time: {currentTime}");
//                 }
//             }
//
//             yield return null;
//         }
//
//         Debug.Log($"Total beats detected: {beatTimes.Count}");
//
//         if (beatTimes.Count > 2)
//         {
//             List<float> intervals = new List<float>();
//             for (int i = 1; i < beatTimes.Count; i++)
//             {
//                 intervals.Add(beatTimes[i] - beatTimes[i - 1]);
//             }
//
//             float averageInterval = intervals.Average();
//             float bpm = 60f / averageInterval;
//
//             // Constrain BPM to a reasonable range
//             bpm = Mathf.Clamp(bpm, MIN_BPM, MAX_BPM);
//
//             Debug.Log($"Calculated BPM: {bpm}");
//             bpmReadings.Add(bpm);
//
//             if (bpmReadings.Count > 5)
//                 bpmReadings.RemoveAt(0);
//
//             float averageBPM = bpmReadings.Average();
//             Debug.Log($"Smoothed BPM: {averageBPM}");
//         }
//         else
//         {
//             Debug.LogWarning("Not enough beats detected to calculate BPM");
//         }
//     }
// }