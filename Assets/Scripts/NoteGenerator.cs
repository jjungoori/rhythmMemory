using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class NoteGenerator : MonoBehaviour
{
    
    public float AdjustBPM(float bpm, float maxBPM = 300)
    {
        while (bpm > maxBPM)
        {
            bpm = Mathf.RoundToInt(bpm * 0.5f);
        }

        return bpm;
    }
    
    public float GetBPM(AudioSource musicSource)
    {
        float bpm = UniBpmAnalyzer.AnalyzeBpm(musicSource.clip);
        bpm = AdjustBPM(bpm);

        return bpm;
    }
    
    public List<float> GenerateNote(AudioSource musicSource, float offset)
    {
        float bpm = GetBPM(musicSource);
        int noteCount = GetNoteNumber(musicSource, bpm);
        
        return GenerateNoteTimingsByBPM(noteCount, bpm, offset);
    }
    
    public int GetNoteNumber(AudioSource musicSource, float calculatedBPM)
    {
        return Mathf.CeilToInt(musicSource.clip.length / 60 * calculatedBPM); // Adjust this as needed
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
}