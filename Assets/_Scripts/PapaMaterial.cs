using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PapaMaterial : MonoBehaviour {

    private Renderer[] renderers;
    public Material Mat;
	// Use this for initialization
	void Start () {
        renderers = this.gameObject.transform.GetComponentsInChildren<Renderer>();
        setNewMaterials();
    }
	
	private void setNewMaterials()
    {
        foreach(Renderer renderer in renderers)
        {
            renderer.material = Mat;
        }
    } 
}
