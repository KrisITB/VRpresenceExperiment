using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class ExperimentMODEL : MonoBehaviour
{
    private float adaptation_PE_PhaseLenght = 90f;   //90
    private float adaptation_VE_PhaseLenght = 90f;   //90
    private float interventionPhaseLenght = 192f;     //12 * 16 = 192
    private float controlPhaseLenght = 192f;
    private float periodLenght = 0f;
    private float trialLenght = 0f;
    private int noOfTrials = 16;
    private float samplingFrequency=0.01f;
    private float hMD_FOV = 110f;
    private float hMD_FOV_lowPerc = 0.33f;
    private float currentSaturation = 100;
    
    public float GetCurrentSaturation()
    {
        return currentSaturation;
    }

    public float GetSamplingFrequency()
    {
        return samplingFrequency;
    }

    public float GetAdaptation_PE_PhaseLenght()
    {
        return adaptation_PE_PhaseLenght;
    }
    public float GetAdaptation_VE_PhaseLenght()
    {
        return adaptation_VE_PhaseLenght;
    }
    public float GetControlPhaseLenght()
    {
        return controlPhaseLenght;
    }
    public float GetInterventionPhaseLenght()
    {
        return interventionPhaseLenght;
    }
    public float GetPeriodLenght()
    {
        return periodLenght;
    }
    public float GetTrialLenght()
    {
        return trialLenght;
    }
    public int GetNoOfTrials()
    {
        return noOfTrials;
    }
    public float GetHMDfov()
    {
        return hMD_FOV;
    }
    public float GetHMDfovLowPerc()
    {
        return hMD_FOV_lowPerc;
    }

    private List<string> data = new List<string>();


    public float GetTotalExperimentTimeLength()
    {
        float totalLengt = adaptation_PE_PhaseLenght + adaptation_VE_PhaseLenght + interventionPhaseLenght + controlPhaseLenght;
        return totalLengt;
    }

    private void Start()
    {
        trialLenght = interventionPhaseLenght / noOfTrials;
        periodLenght = trialLenght / 4f;
    }

    public void updateDataSet(string newData)
    {
        data.Add(newData);
    }

    public void SaveCsv(int sampleID, int groupID)
    {
#if UNITY_EDITOR
        string filePath = Application.dataPath + "/Data/" + "Sample_Number-" + sampleID + "_Group_Number-" + groupID + ".csv";
#else
        string filePath = Application.persistentDataPath + "/" + "Sample_Number-" + sampleID + "_Group_Number-" + groupID + ".csv";
#endif

        string delimiter = ",";
        StringBuilder sb = new StringBuilder();

        foreach (string dataString in data)
        {
            string toAppend = string.Concat(dataString, delimiter);
            //string toAppend = string.Join(delimiter, dataString);
            sb.AppendLine(toAppend);
        }

        File.WriteAllText(filePath, sb.ToString());

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
