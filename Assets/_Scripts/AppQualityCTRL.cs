using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppQualityCTRL : MonoBehaviour
{
    // Start is called before the first frame update
    private string[] allQualityNames;
    private string[] myNames;

    int newQuality = 999;
    int currentQuality = 999;
    int nOfQualities = 999;

    void Start()
    {
        qualityInit();
    }

    private void qualityInit()
    {
        allQualityNames = QualitySettings.names;
        myNames = new string[] { "Q1", "Q2", "Q3", "Q4", "Q5" };
        nOfQualities = myNames.Length;
        //Debug.Log(nOfQualities);
        newQuality = nOfQualities;
        StartCoroutine ("setAppQuality");
    }

    public string modulateAppQuality(float progression)
    {
        int setQualit = QualitySettings.GetQualityLevel();
        if (progression == 1f)
        {
            if(setQualit != currentQuality - 1)
            {
                newQuality = nOfQualities;
            }
        }
        else if (progression == 0f)
        {
            if (setQualit != currentQuality - 1)
            {
                newQuality = 1;
            }
        }
        else
        {
            newQuality = Mathf.CeilToInt(Mathf.Lerp(1f, nOfQualities - 1, progression));
        }

        if (newQuality != currentQuality)
        {
            currentQuality = newQuality;
            StartCoroutine("setAppQuality");
        }
        //Debug.Log("progression : " + progression + " | newQuality : " + newQuality + " | setQuality : " + QualitySettings.GetQualityLevel());
        
        return allQualityNames[QualitySettings.GetQualityLevel()]; 
        //return allQualityNames[currentQuality-1]; //cheaper than above
    }
    
    IEnumerator setAppQuality()
    {
        string qualityName = myNames[newQuality - 1];
        int pos = Array.IndexOf(allQualityNames, qualityName);
        yield return null;
        QualitySettings.SetQualityLevel(pos);
        //Debug.Log("pos : " + pos + " | newQuality : " + newQuality + " | qualityName : " + qualityName + " | setQuality : " + QualitySettings.GetQualityLevel());
        
    }
}
