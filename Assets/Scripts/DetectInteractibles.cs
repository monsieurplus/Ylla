using UnityEngine;
using System.Collections;

public class DetectInteractibles : MonoBehaviour {

    [SerializeField]private Canvas refCanvas;

    Camera cam;
    ShineOnLookScript prevObjectShineScript;
    string previousColliderName = "";

	// Use this for initialization
	void Start () {
        cam = this.GetComponent <Camera> ();
	}
	
	// Update is called once per frame
	void Update () {

        //casts a ray to test for presence of an object in front of the camera. Ray has range of 1.
        RaycastHit hit;

        //Debug.DrawLine(cam.transform.position + cam.transform.forward, cam.transform.position + cam.transform.forward + cam.transform.forward, Color.red);

        
        if (Physics.Raycast(cam.transform.position + cam.transform.forward, cam.transform.forward, out hit, 1f) ) {

            //if ray sucessfully hits something, it can cause a reaction if it is an interactible object
			ShineOnLookScript objectShineScript = hit.collider.GetComponent<ShineOnLookScript>();
			if (hit.collider.name != "FPSController" && objectShineScript != null )
            {
                //If object is different, start shine on that object. Mark it as last object highlighted
                if (hit.collider.name != previousColliderName) {
                    objectShineScript.ActivateShine();
                    previousColliderName = hit.collider.name;
                    prevObjectShineScript = objectShineScript;
                }
                
                //if "Fire1" button is down, player attempts interaction with the object
                if ( Input.GetButtonDown("Fire1") )
                {
                    Debug.Log("Clicked on interactible object :" + hit.collider.name);

                    //Depending on the tag detected on the clicked object, the behavior is different
                    if (hit.collider.tag == "PaperDocument")
                    {
                        UI_Scripting interfaceScript = refCanvas.GetComponent<UI_Scripting>();
                        interfaceScript.loadPaperDocument(hit.collider.name);
                    }
                }

            }
            else {
                //If the ray is not colliding an interactible object, deactivate shine on last highlighted object
                if (prevObjectShineScript != null) { prevObjectShineScript.DeactivateShine(); prevObjectShineScript = null; previousColliderName = ""; }
            }

        }


        
	}
}
