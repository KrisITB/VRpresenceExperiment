using UnityEngine;
//using Valve.VR;
using UnityEngine.XR;

public class FpsCTRL : MonoBehaviour
{
    private int MaxFPS;
    private int MinFPS;
    private float fps;

    float c;

    private int targetFPS;
    private void Update()
    {
        fps = 1 / Time.deltaTime;
    }

    void Start()
    {
        MaxFPS = 90;
        MinFPS = 60;
        c = 5f;

        //SteamVR.settings.lockPhysicsUpdateRateToRenderFrequency = false;
        QualitySettings.vSyncCount = 0; 
        Application.targetFrameRate = MaxFPS;

        targetFPS = MaxFPS;
    }
    
    public int ModulateFrameRate(float progression)
    {
        //int currentTarget;
        int sleepFor = 0;

        if(progression <= 0.001f)
        {
            //currentTarget = 60;
            if (fps > 70)
            {
                c += 1f;
            }
            else if (fps > 60)
            {
                c += 0.1f;
            }
            else if(fps < 60)
            {
                c -= 0.1f;
            }
            else if(fps < 50)
            {
                c -= 2f;
            }

            if (fps > 50)
            {
                sleepFor = Mathf.RoundToInt(20f + c - (Time.smoothDeltaTime * 1000f)); //  - (Time.deltaTime * 1000f)
                if (sleepFor <= 0)
                {
                    sleepFor = 0;
                }
                else
                {
                    System.Threading.Thread.Sleep(sleepFor);
                }
                //sleepFor = Mathf.CeilToInt(28.6f - (Time.smoothDeltaTime * 1000f));
               //Application.targetFrameRate = 60;
            }
        }
        else
        {
            c = 5f;
        }
        /*
        else
        {
            currentTarget = 90;
            Application.targetFrameRate = 90;
        }
        /*
        if(targetFPS != currentTarget)
        {
            targetFPS = currentTarget;
            Application.targetFrameRate = targetFPS;
        }
        */
        


        
        
           //15 gives around 55.5 FPS
           //13 gives around 61.5 FPS
           //12 gives around 66.5 FPS
           //11 gives around 70 FPS
           //10 gives around 76 FPS
           //7 does nothing

           //16.6 ms in total = 60 FPS
        

        //System.Threading.Thread.Sleep(100 / targetFrameRate *10); //in theory *10 but brutal then

        //Debug.Log("Application.targetFrameRate : " + Application.targetFrameRate + "Vsync : " + QualitySettings.vSyncCount);
        //return Application.targetFrameRate;
        return sleepFor;
    }

}
