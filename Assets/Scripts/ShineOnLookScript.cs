using UnityEngine;
using System.Collections;

public class ShineOnLookScript : MonoBehaviour {

    private MeshRenderer renderer;
    private Color startColor;
    private bool isShining = false;

	// Use this for initialization
	void Awake () {
        renderer = GetComponent<MeshRenderer>();
        startColor = renderer.material.color;   
    }

    //Activate "shine" effect 
    public void ActivateShine()
    {
        if (!isShining) {
            renderer.material.color = new Color(1f,1f,0f,1f);
            isShining = true;
        }
    }

    //Deactivate "Shine" effect
    public void DeactivateShine()
    {
        if (isShining)
        {
            renderer.material.color = startColor;
            isShining = false;
        }
    }
}
