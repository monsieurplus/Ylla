using UnityEngine;
using System.Collections;

// During this scene :
// - The martian sphere moves around the scenery
// - When the player get close to the sphere, he automatically look at it
// - The sphere speaks to the player
// - The sphere leaves the player and goes away
public class DesertSceneControllerThird : MonoBehaviour {
	public Canvas guiCanvas;

	public GameObject martianSphere;

	public AudioSource audioDialog;
	public AudioSource audioSong;
	public AudioSource audioReverb;
	
	private MartianSphereMovementController martianSphereMovements;

	public GameObject player;
	public GameObject character;

	public float sphereMovingSpeed = 5.0f;
	public float spherePauseDuration = 1.0f;
	
	private Vector3[] spherePositions = new Vector3[10];

	private int currentPosition = 0;

	private int currentMartianDialog = 0;
	[SerializeField] AudioClip[] martianDialogs;


	// TODO Start methods
	public bool sceneStarted = false;
	private bool sceneFinished = false;

	// Phases :
	// "moveAroundUnstoppable01"
	// "moveAround"
	// "rotateCamera"
	// "dialog01"
	private string currentPhase = "moveAroundUnstoppable";

	private float movingAwayDuration = 3.0f;
	private float movingAwayStart = 0.0f;

	// Minimum distance to trigger the sphere dialog scene 
	private float minDistanceSphereFromPlayer = 3.0f;
	private float lookRotationSpeed = 5.0f;

	private float fadeDuration = 1.0f;
	private float volumeSong = 1.0f;
	private float volumeReverb = 0.05f;
	private AnimationCurve fadeSong;
	private AnimationCurve fadeReverb;

	public GameObject nextSceneController;

	// Use this for initialization
	private void Start () {
		initializePositions ();

		martianSphereMovements = martianSphere.GetComponent<MartianSphereMovementController> ();
	}

	private void initializePositions() {
		spherePositions [0]  = new Vector3 (-26.30f, 6.70f, 104.16f);
		spherePositions [1]  = new Vector3 (  2.90f, 0.80f, 123.80f);
		spherePositions [2]  = new Vector3 ( 10.10f, 0.80f, 127.04f);
		spherePositions [3]  = new Vector3 ( 10.67f, 0.80f, 132.54f);
		spherePositions [4]  = new Vector3 ( 15.84f, 0.80f, 136.56f);
		spherePositions [5]  = new Vector3 ( 15.84f, 4.63f, 160.55f);
		spherePositions [6]  = new Vector3 ( 33.72f, 3.23f, 166.23f);
		spherePositions [7]  = new Vector3 ( 38.32f, 5.01f, 173.40f);
		spherePositions [8]  = new Vector3 ( 38.32f, 7.11f, 184.34f);
		spherePositions [9]  = new Vector3 ( 16.42f, 4.14f, 177.42f);
	}

	// Update is called once per frame
	void LateUpdate () {
		Debug.Log ("DesertScene03 : " + currentPhase);

		if (sceneStarted == true && sceneFinished == false) {
			switch (currentPhase) {
				case "moveAroundUnstoppable":
					phaseMoveAroundUnstoppable();	
				break;
				case "moveAround":
				phaseMoveAround();
				break;

				case "rotateCamera":
					phaseRotateCamera();
				break;

				case "dialog01":
					phaseDialogOne();
				break;
			}
		}
	}

	private void phaseMoveAroundUnstoppable() {
		// Moving the sphere away from the player
		if (movingAwayStart == 0.0f) {
			martianSphereMovements.animateTo (spherePositions [0], sphereMovingSpeed * 2);
			movingAwayStart = Time.time;
		} else {
			if ((Time.time - movingAwayStart) > movingAwayDuration) {
				currentPhase = "moveAround";
			}
		}
	}

	private void phaseMoveAround() {
		// Give the sphere a destination regulary
		if (!martianSphereMovements.isMoving () && (Time.time - martianSphereMovements.getMovingEnd ()) > spherePauseDuration) {
			// Moving the sphere to the next position
			currentPosition++;
			if (currentPosition >= spherePositions.Length) {
				currentPosition = 5;
			}
			
			martianSphereMovements.animateTo (spherePositions [currentPosition], sphereMovingSpeed);
		}

		// Check the distance between the sphere and the player 
		if (getSphereDistanceFromPlayer () <= minDistanceSphereFromPlayer) {
			// We change the trigger distance so that the player doesn't exit the trigger zone while rotating
			minDistanceSphereFromPlayer--;

			// Lock player's movements
			player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;

			// Stop the sphere's movements
			martianSphereMovements.stopMoving ();

			// Change phase
			currentPhase = "rotateCamera";
		}
	}

	private float getSphereDistanceFromPlayer() {
		return Vector3.Distance (martianSphere.transform.position, Camera.main.transform.position);
	}

	private void phaseRotateCamera() {
		bool phaseIsOver = true;
		
		// Create the sound fading animations
		if (fadeSong == null || fadeReverb == null) {
			fadeSong = AnimationCurve.EaseInOut(Time.time, audioSong.volume, Time.time + fadeDuration, 0);
			fadeReverb = AnimationCurve.EaseInOut (Time.time, audioReverb.volume, Time.time + fadeDuration, volumeReverb);
		}
		
		// Process the fadeIn/fadeOut animation
		audioSong.volume = fadeSong.Evaluate (Time.time);
		audioReverb.volume = fadeReverb.Evaluate (Time.time);
		
		// Check if sound fading is over
		if (audioSong.volume > 0) {
			phaseIsOver = false;
		}
		
		// Make the player look at the sphere
		Camera camera = character.gameObject.GetComponent<Camera>();
		
		Quaternion lookRotation = Quaternion.LookRotation(martianSphere.transform.position - camera.transform.position);
		camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
		
		// If the remaining angle is little, we change phase
		float deltaAngle = Quaternion.Angle(camera.transform.rotation, lookRotation);
		if (deltaAngle >= 1.0f) {
			phaseIsOver = false;
		}
		
		if (phaseIsOver) {
			fadeSong = null;
			fadeReverb = null;
			currentPhase = "dialog01";
		}
	}

	/*
	private void phaseRotateCamera() {
		// Make the player look at the sphere
		Camera camera = character.gameObject.GetComponent<Camera>();
		
		Quaternion lookRotation = Quaternion.LookRotation(martianSphere.transform.position - camera.transform.position);
		camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);

		// If the remaining angle is little, we change phase

		float deltaAngle = Quaternion.Angle(camera.transform.rotation, lookRotation);
		if (deltaAngle < 1.0f) {
			currentPhase = "dialog01";
		}
	}
	*/

	private void phaseDialogOne() {
		// Look at the martian sphere continuously
		Camera camera = character.gameObject.GetComponent<Camera>();
		camera.transform.LookAt (martianSphere.transform);
		
		if (!audioDialog.isPlaying) {
			
			if (currentMartianDialog >= 6 || currentMartianDialog >= martianDialogs.Length) {
				// Create the sound fading animations
				if (fadeSong == null || fadeReverb == null) {
					fadeSong = AnimationCurve.EaseInOut(Time.time, audioSong.volume, Time.time + fadeDuration, volumeSong);
					fadeReverb = AnimationCurve.EaseInOut (Time.time, audioReverb.volume, Time.time + fadeDuration, 0);
				}
				
				audioSong.volume = fadeSong.Evaluate(Time.time);
				audioReverb.volume = fadeReverb.Evaluate(Time.time);
				
				if (audioReverb.volume <= 0) {
					// Stop current scene / Launch next Scene
					sceneFinished = true;
					nextSceneController.GetComponent<BridgeSceneControllerFirst>().sceneStarted = true;
					
					// Unlock the player's controls
					player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
				}
			}
			else {
				// Use the GUI to play a sound and display subtitles
				UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
				uiScripting.PlayLineWithSubtitles(audioDialog, martianDialogs[currentMartianDialog]);
				
				currentMartianDialog++;
			}
		}
	}

	/*
	private void phaseDialogOne() {
		// Look at the martian sphere continuously
		Camera camera = character.gameObject.GetComponent<Camera>();
		camera.transform.LookAt (martianSphere.transform);

		if (!audioDialog.isPlaying) {

			if (currentMartianDialog >= 6 || currentMartianDialog >= martianDialogs.Length) {
				// Stop current scene / Launch next Scene
				sceneFinished = true;
				nextSceneController.GetComponent<BridgeSceneControllerFirst>().sceneStarted = true;
				
				// Unlock the player's controls
				player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
			}
			else {
				// Use the GUI to play a sound and display subtitles
				UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
				uiScripting.PlayLineWithSubtitles(audioDialog, martianDialogs[currentMartianDialog]);

				currentMartianDialog++;
			}
		}
	}
	*/
}
