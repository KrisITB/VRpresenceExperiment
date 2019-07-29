using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FOV_CTRL : MonoBehaviour
{
    public ExperimentMODEL ExperimentModel;
    private float startScale; // to do: measure startScale and endScale in HMD
    private float endScale;

    private float targetPercentage; 
    private float hmdMAX;
    private float hmdLow;

    private void Start()
    {
        hmdMAX = ExperimentModel.GetHMDfov();
        targetPercentage = ExperimentModel.GetHMDfovLowPerc();
        startScale = this.gameObject.transform.localScale.x;
        endScale = startScale * targetPercentage;
        hmdLow = hmdMAX * targetPercentage;
    }
    
    public float ModulateFOV(float progression)
    {
        float currentScale = Mathf.Lerp(startScale, endScale, 1-progression);
        this.gameObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        float fov = ((int)(Mathf.Lerp(hmdMAX, hmdLow, 1 - progression)* 100)) / 100f;
        return fov;
    }

}
