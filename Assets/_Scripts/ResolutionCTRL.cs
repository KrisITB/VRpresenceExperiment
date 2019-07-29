using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ResolutionCTRL : MonoBehaviour
{
    
    //private Resolution[] resolutions;
    //Resolution currentResolution;
    //Resolution newResolution;
    //private int noOfRes = 999;

    private float minScale = 0.1f;
    private float maxScale = 1f;
    private float newScale = 999f;
    private float currentScale = 999f;

    public string GetCurrentResolutionScale()
    {
        return currentScale.ToString();
    }

    public string GetCurrentResolution()
    {
        string res = "W: " + XRSettings.eyeTextureWidth + " H: " + XRSettings.eyeTextureHeight;
        return res;
    }
    public string GetCurrentPixels()
    {

        int pixels = XRSettings.eyeTextureWidth * XRSettings.eyeTextureHeight;
        return pixels.ToString();
    }

    void Start()
    {
        newScale = 1f;
        XRSettings.eyeTextureResolutionScale = newScale;
        currentScale = 1f;

        /*
         //use to reset main (desktop) screen resolution 
        resolutions = Screen.resolutions;
        noOfRes = resolutions.Length;
        Screen.SetResolution(resolutions[noOfRes-1].width, resolutions[noOfRes - 1].height, true);
        */
    }

    /*
    public string GetCurrentScale()
    {
        return currentScale.ToString();
    }
    */

    public string ModulateResolution(float progress)
    {
        if(progress == 1)
        {
            newScale = 1f;
            if(currentScale != newScale)
            {
                XRSettings.eyeTextureResolutionScale = newScale;
                currentScale = newScale;
            }
        }
        else if(progress == 0f)
        {
            newScale = 0.1f;
            if (currentScale != newScale)
            {
                XRSettings.eyeTextureResolutionScale = newScale;
                currentScale = newScale;
            }
        }
        else
        {
            newScale = Mathf.Ceil(progress * 10) / 10f; //messy if at 0.01!
            /*
            newScale = Mathf.Lerp(minScale, maxScale, progress);

            //temp:
            if (newScale < 1 && newScale >= 0.9)
            {
                newScale = 0.9f;
            }
            else if (newScale < 0.9 && newScale >= 0.8)
            {
                newScale = 0.8f;
            }
            else if (newScale < 0.8 && newScale >= 0.7)
            {
                newScale = 0.7f;
            }
            else if (newScale < 0.7 && newScale >= 0.6)
            {
                newScale = 0.6f;
            }
            else if (newScale < 0.6 && newScale >= 0.5)
            {
                newScale = 0.5f;
            }
            else if (newScale < 0.5 && newScale >= 0.4)
            {
                newScale = 0.4f;
            }
            else if (newScale < 0.4 && newScale >= 0.3)
            {
                newScale = 0.3f;
            }
            else if (newScale < 0.3 && newScale >= 0.2)
            {
                newScale = 0.2f;
            }
            else if (newScale < 0.2 && newScale >= 0.1)
            {
                newScale = 0.1f;
            }*/

            if (currentScale != newScale)
            {
                XRSettings.eyeTextureResolutionScale = newScale;
                currentScale = newScale;
            }
        }

        string res = "W: " + XRSettings.eyeTextureWidth + " H: " + XRSettings.eyeTextureHeight;
        return res;

        //return currentScale.ToString();
    }

        /*
        // Start is called before the first frame update
        void Start()
        {
            resolutions = Screen.resolutions;
            foreach (Resolution res in resolutions)
            {
                Debug.Log(res.ToString());
            }
            noOfRes = resolutions.Length;
        }

        public string ModulateResolution(float progress)
        {
            if (progress == 1f)
            {
                newResolution = resolutions[noOfRes - 1];
            }
            else if(progress == 0f)
            {
                newResolution = resolutions[0];
            }
            else
            {
                int newResIndex = Mathf.CeilToInt(Mathf.Lerp(1, noOfRes-2, progress));
                newResolution = resolutions[newResIndex];

                Debug.Log("newResIndex : " + newResIndex);
            }
            if(!string.Equals(newResolution.ToString(), currentResolution.ToString()))
            {
                currentResolution = newResolution;
                Screen.SetResolution(newResolution.width, newResolution.height,true);
            }
            Debug.Log("newRes : " + Screen.currentResolution);

            return Screen.currentResolution.ToString();
        }
        */
    }
