using UnityEngine;
using System.Collections;

public class ShineOnLookScript : MonoBehaviour {

    //private MeshRenderer Mrenderer;
    private Color startColor;
    private bool isShining = false;

	public Color shiningColor = Color.yellow;

	// Use this for initialization
	void Awake () {
		//startColor = GetComponent<Renderer> ().material.color;
    }

    //Activate "shine" effect 
    public void ActivateShine()
    {
		if (!isShining) {
			startColor = GetComponent<Renderer> ().material.color;
			GetComponent<Renderer>().material.color = shiningColor;
			isShining = true;
		}
    }

    //Deactivate "Shine" effect
    public void DeactivateShine()
    {
		if (isShining) {
			GetComponent<Renderer>().material.color = startColor;
			isShining = false;
		}
    }
}
