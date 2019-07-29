using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyCounter : MonoBehaviour {

    //Experimental code for counting polygons below:
    private GameObject[] AllGO;

    private void Start()
    {
        AllGO = FindObjectsOfType<GameObject>();
    }

    public string polyCounter()
    {
        int polycount = 0;
        int thisPoly = 0;
        
        foreach (GameObject GO in AllGO)
        {
            MeshFilter newMesh = null;

            if (GO.gameObject.activeSelf)
            {
                Renderer rend = GO.GetComponent<Renderer>();
                if (rend != null && rend.isVisible)
                {
                    newMesh = GO.GetComponent<MeshFilter>();
                }
            }
            
            if (newMesh != null)
            {
                thisPoly = newMesh.mesh.triangles.Length / 3;
                polycount += thisPoly;
                Debug.Log(GO.name + " this = " + thisPoly + " |new count = " + polycount);
            }
        }
        return polycount.ToString();
    }
}
