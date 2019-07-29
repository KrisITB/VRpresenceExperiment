using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingCTRL : MonoBehaviour
{
    private float saturation;

    public PostProcessVolume SaturationVolume;
    private ColorGrading colorGrading;
    private PostProcessVolume volume;

    void Start()
    {
        volume = GetComponent<PostProcessVolume>();

        colorGrading = ScriptableObject.CreateInstance<ColorGrading>();
        colorGrading.enabled.Override(true);
        colorGrading.saturation.Override(0f);
        volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, colorGrading);
        saturation = 0f;
    }

    public float ModulateSaturation(float progression)
    {
        float targetSaturation = 0f;

        if (progression == 1f && saturation != 0f)
        {
            targetSaturation = 0f;
            colorGrading.saturation.Override(targetSaturation);
            saturation = targetSaturation;
        }
        else if(progression == 0f && saturation != -100f)
        {
            targetSaturation = -100f;
            colorGrading.saturation.Override(targetSaturation);
            saturation = targetSaturation;
        }
        else
        {
            targetSaturation = Mathf.Lerp(-100, 0, progression);
            colorGrading.saturation.Override(targetSaturation);
            saturation = targetSaturation;
        }

        return targetSaturation + 100f;
    }


    
    void Destroy()
    {
        RuntimeUtilities.DestroyVolume(volume, true);
    }
}
