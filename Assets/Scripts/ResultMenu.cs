using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultMenu : MonoBehaviour
{
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI processText;
    
    public TextMeshProUGUI rankText;
    
    // Start is called before the first frame update
    string GetRank(int score)
    {
        string rank = "F"; // Default to F if none of the conditions match

        if (score >= 95 && score <= 100)
        {
            rank = "S";
        }
        else if (score >= 90 && score <= 94)
        {
            rank = "A";
        }
        else if (score >= 85 && score <= 89)
        {
            rank = "B";
        }
        else if (score >= 80 && score <= 84)
        {
            rank = "C";
        }
        else if (score >= 75 && score <= 79)
        {
            rank = "D";
        }

        return rank;
    }

    public void SetData(List<float> recordedDifferences, AudioSource musicSource)
    {
        float averageDifference;
        if (recordedDifferences.Count == 0)
        {
            averageDifference = 0;
        }
        else
        {
            float sumOfDifferences = 0;
            foreach (float difference in recordedDifferences)
            {
                sumOfDifferences += difference;
            }

            averageDifference = sumOfDifferences / recordedDifferences.Count;
        }
        
        float process = (float)AudioSettings.dspTime / musicSource.clip.length;
        process = process == 0 ? 100 : process;
        
        process = Mathf.Round(process * 100);
        averageDifference = Mathf.Round(averageDifference * 100);
        
        float averageAccuracy = 100 - averageDifference;

        accuracyText.text = "Your Accuracy: " + averageAccuracy.ToString() + "%";
        processText.text = "Music process: " + process.ToString() + "%";

        rankText.text = GetRank((int)averageAccuracy+(int)process-100);
    }
}
