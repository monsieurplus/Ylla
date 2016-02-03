using UnityEngine;
using System.Collections;

// During this scene :
// - The martian sphere moves around the scenery
// - When the player get close to the sphere, he automatically look at it
// - The sphere speaks to the player
// - The sphere leaves the player and goes away
public class DesertSceneControllerFirst : MonoBehaviour {
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
	
	private Vector3[] spherePositions = new Vector3[9];

	private int currentPosition = 0;

	private int currentMartianDialog = 0;
	[SerializeField] AudioClip[] martianDialogs;


	// TODO Start methods
	public bool sceneStarted = false;
	private bool sceneFinished = false;

	// Phases :
	// "moveAround"
	// "rotateCamera"
	// "dialog01"
	// "waitingForPlayer"
	// "dialog02"
	private string currentPhase = "moveAround";

	// Minimum distance to trigger the sphere dialog scene 
	private float minDistanceSphereFromPlayer = 3.0f;
	private float lookRotationSpeed = 5.0f;

	private float fadeDuration = 1.0f;
	private float volumeSong = 1.0f;
	private float volumeReverb = 0.5f;
	private AnimationCurve fadeSong;
	private AnimationCurve fadeReverb;


	public GameObject nextSceneController;

	// Use this for initialization
	private void Start () {
		initializePositions ();

		martianSphereMovements = martianSphere.GetComponent<MartianSphereMovementController> ();
	}

	private void initializePositions() {
		spherePositions [0] = new Vector3 (-57.27f, 2.26f, -69.58f);
		spherePositions [1] = new Vector3 (-12.95f, 5.73f, -96.80f);
		spherePositions [2] = new Vector3 ( -9.23f, 5.23f, -96.80f);
		spherePositions [3] = new Vector3 ( -5.53f, 5.21f, -97.67f);
		spherePositions [4] = new Vector3 (-26.78f, 2.21f, -70.15f);
		spherePositions [5] = new Vector3 (-51.78f, 3.94f, -39.52f);
		spherePositions [6] = new Vector3 (-52.39f, 2.00f, -65.29f);
		spherePositions [7] = new Vector3 (-62.77f, 2.00f, -70.92f);
		spherePositions [8] = new Vector3 (-59.08f, 3.74f, -64.30f);
	}

	// Update is called once per frame
	void LateUpdate () {
		if (sceneStarted == true && sceneFinished == false) {
			//Debug.Log ("DesertScene01 : " + currentPhase);

			switch (currentPhase) {
				case "moveAround":
					phaseMoveAround();	
				break;

				case "rotateCamera":
					phaseRotateCamera();
				break;

				case "dialog01":
					phaseDialogOne();
				break;

				case "waitingForPlayer":
					phaseWaitingForPlayer();
				break;

				case "dialog02":
					phaseDialogTwo();
				break;
			}
		}
	}

	private void phaseMoveAround() {
		// Give the sphere a destination regulary
		if (!martianSphereMovements.isMoving () && (Time.time - martianSphereMovements.getMovingEnd ()) > spherePauseDuration) {
			// Moving the sphere to the next position
			currentPosition++;
			if (currentPosition >= spherePositions.Length) {
				currentPosition = 0;
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

	private void phaseDialogOne() {
		// Look at the martian sphere continuously
		Camera camera = character.gameObject.GetComponent<Camera>();
		camera.transform.LookAt (martianSphere.transform);


		if (!audioDialog.isPlaying) {

			if (currentMartianDialog >= 5 || currentMartianDialog >= martianDialogs.Length) {
				currentPhase = "waitingForPlayer";
			}
			else {
				// Use the GUI to play a sound and display subtitles
				UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
				uiScripting.PlayLineWithSubtitles(audioDialog, martianDialogs[currentMartianDialog]);

				currentMartianDialog++;
			}
		}
	}

	private void phaseWaitingForPlayer() {
		// Look at the martian sphere continuously
		Camera camera = character.gameObject.GetComponent<Camera>();
		camera.transform.LookAt (martianSphere.transform);

		if (Input.GetButtonDown ("Fire1")) {
			currentPhase = "dialog02";
		}
	}

	private void phaseDialogTwo() {
		// Look at the martian sphere continuously
		Camera camera = character.gameObject.GetComponent<Camera>();
		camera.transform.LookAt (martianSphere.transform);
		
		
		if (!audioDialog.isPlaying) {
			
			if (currentMartianDialog >= 10 || currentMartianDialog >= martianDialogs.Length) {
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
					nextSceneController.GetComponent<DesertSceneControllerSecond>().sceneStarted = true;
					
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
}
