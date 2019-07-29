using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyCTRL : MonoBehaviour
{
    public GameObject[] Models;
    private Renderer[][] renderers;
    int noOfModels = 0;
    int currentModel;

    bool update = false;
    bool modelsReady = false;

    private void Start()
    {
        //polyCounter();
        Invoke("initModels", 45f);
        //Invoke("initPhysics", 1); // models need time to initialize the cloth component
    }

    private void initPhysics()
    {
        StartCoroutine(enableModels());
    }

    IEnumerator enableModels() //during adaptation PE phase -> run through the physics cycle for each tent model
    {
        for (float i = 0f; i <= 1f; i += 0.01f)
        {
            ModulateFidelity(i);
            if (update)
            {
                yield return new WaitForSeconds(10f);
            }
            //yield return new WaitForFixedUpdate();
        }
        // loop above sometimes doesn't go to 1f due to float
        //yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(10f);
        ModulateFidelity(1f);
    }

    public int ModulateFidelity(float progression)
    {
        // from 1 = 100% To 0 = 0%

        if (progression == 1f && currentModel != noOfModels - 1)
        {
            currentModel = noOfModels - 1;
            update = true;
        }
        else if (progression == 0f && currentModel != 0)
        {
            currentModel = 0;
            update = true;
        }
        else if (progression != 1 && progression != 0)
        {
            int toBeUpdated = (int)Mathf.Lerp(1, noOfModels - 1, progression);
            if (currentModel != toBeUpdated)
            {
                currentModel = toBeUpdated;
                update = true;
            }
        }
        if (update)
        {
            crawlThroughModels();
            update = false;
        }
        return currentModel;
        //Debug.Log("progression : " + progression + " | currentModel : " + currentModel);
    }

    public int initModels()
    {
        if (!modelsReady)
        {
            noOfModels = Models.Length;
            currentModel = Models.Length - 1; // 0-4
            renderers = new Renderer[noOfModels][];
            for (int i = 0; i < Models.Length; i++)
            {
                renderers[i] = getRenderersInChildren(Models[i]);
                if (i < currentModel)
                {
                    foreach (Renderer renderer in renderers[i])
                    {
                        renderer.enabled = false;
                    }
                }
                else
                {
                    foreach (Renderer renderer in renderers[i])
                    {
                        renderer.enabled = true;
                    }
                }
            }
            modelsReady = true;
        }
        return noOfModels;
    }

    private Renderer[] getRenderersInChildren(GameObject model)
    {
        Renderer[] renderersInChildren = model.GetComponentsInChildren<Renderer>();
        return renderersInChildren;
    }

    private void crawlThroughModels()
    {
        for (int i = 0; i < noOfModels; i++)
        {
            if (i == currentModel)
            {
                foreach (Renderer renderer in renderers[i])
                {
                    renderer.enabled = true;
                }
            }
            else
            {
                foreach (Renderer renderer in renderers[i])
                {
                    renderer.enabled = false;
                }
            }
        }
    }
}
