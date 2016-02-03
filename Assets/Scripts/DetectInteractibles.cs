using UnityEngine;
using System.Collections;

public class DetectInteractibles : MonoBehaviour {
	public bool enabled = false;

    [SerializeField]private Canvas refCanvas;
    [SerializeField]private GameObject refPlayer;
    [SerializeField]private AudioClip[] PaperSounds;

    Camera cam;
    ShineOnLookScript prevObjectShineScript;
    string previousColliderName = "";

    AudioSource audioSrc;

	// Use this for initialization
	void Start () {
        cam = this.GetComponent <Camera> ();
        audioSrc = refPlayer.GetComponent<AudioSource>(); ;
	}
	
	// Update is called once per frame
	void Update () {
		if (enabled == true) {

			//casts a ray to test for presence of an object in front of the camera. Ray has range of 1.
			RaycastHit hit;

			//Debug.DrawLine(cam.transform.position + cam.transform.forward, cam.transform.position + cam.transform.forward + cam.transform.forward, Color.red);

			// If an object is already shining, we unshine it
			UnshinePreviousObject ();
	        
			if (Physics.Raycast (cam.transform.position + cam.transform.forward, cam.transform.forward, out hit, 2f)) {

				//if ray sucessfully hits something, it can cause a reaction if it is an interactible object
				ShineOnLookScript objectShineScript = hit.collider.GetComponent<ShineOnLookScript> ();
				if (hit.collider.name != "FPSController" && objectShineScript != null) {
					//If object is different, start shine on that object. Mark it as last object highlighted
					if (hit.collider.name != previousColliderName) {
						objectShineScript.ActivateShine ();
						previousColliderName = hit.collider.name;
						prevObjectShineScript = objectShineScript;
					}
	                
					//if "Fire1" button is down, player attempts interaction with the object
					if (Input.GetButtonDown ("Fire1")) {

						//Depending on the tag detected on the clicked object, the behavior is different
						if (hit.collider.tag == "PaperDocument") {
							//Play a sound that evoques paper folding
							int n = Random.Range (0, PaperSounds.Length);
							audioSrc.clip = PaperSounds [n];
							audioSrc.PlayOneShot (audioSrc.clip);

							UI_Scripting interfaceScript = refCanvas.GetComponent<UI_Scripting> ();
							interfaceScript.loadPaperDocument (hit.collider.name);
						}

						// All the Projector related cases
						else if (hit.collider.tag.StartsWith ("Projector")) {

							// Find the FinalProjectorController
							FinalProjectorController finalProjector = hit.collider.gameObject.transform.parent.GetComponent<FinalProjectorController> ();
							
							// Rotator click
							if (hit.collider.tag == "ProjectorRotator") {
								finalProjector.Rotate ();
							}
							// Spot click
							else {
								// Extract the clicked spot color
								string color;
								if (hit.collider.tag == "ProjectorSpotRed") {
									color = "red";
								} else if (hit.collider.tag == "ProjectorSpotGreen") {
									color = "green";
								} else {
									color = "blue";
								}
								
								// Toggle the spot
								finalProjector.toggleLight (color);
							}
						}
					}
				} else {
					//If the ray is not colliding an interactible object, deactivate shine on last highlighted object
					if (prevObjectShineScript != null) {
						prevObjectShineScript.DeactivateShine ();
						prevObjectShineScript = null;
						previousColliderName = "";
					}
				}

			}

		}
        
	}

	void UnshinePreviousObject() {
		if (prevObjectShineScript != null) {
			prevObjectShineScript.DeactivateShine();
			prevObjectShineScript = null;
			previousColliderName = "";
		}
	}
}
