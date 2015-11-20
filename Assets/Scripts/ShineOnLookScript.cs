using UnityEngine;
using System.Collections;

public class ShineOnLookScript : MonoBehaviour {

    private MeshRenderer Mrenderer;
    private Color startColor;
    private bool isShining = false;

	// Use this for initialization
	void Awake () {
        Mrenderer = GetComponent<MeshRenderer>();
        startColor = Mrenderer.material.color;   
    }

    //Activate "shine" effect 
    public void ActivateShine()
    {
        if (!isShining) {
            Mrenderer.material.color = new Color(1f,1f,0f,1f);
            isShining = true;
        }
    }

    //Deactivate "Shine" effect
    public void DeactivateShine()
    {
        if (isShining)
        {
            Mrenderer.material.color = startColor;
            isShining = false;
        }
    }
}
