using UnityEngine;
using System.Collections;

// During this scene :
// - The martian sphere moves around the scenery
// - When the player get close to the sphere, he automatically look at it
// - The sphere speaks to the player
// - The sphere leaves the player and goes away
public class BridgeSceneControllerFirst : MonoBehaviour {
	public Canvas guiCanvas;

	public GameObject martianSphere;
	
	public AudioSource audioDialog;
	public AudioSource audioSong;
	public AudioSource audioReverb;
	public AudioSource audioAmbiant;

	private MartianSphereMovementController martianSphereMovements;

	public GameObject player;
	public GameObject character;

	public float sphereMovingSpeed = 5.0f;
	public float spherePauseDuration = 1.0f;
	
	private Vector3[] spherePositions = new Vector3[2];

	private int currentPosition = 0;

	private int currentMartianDialog = 0;
	[SerializeField] AudioClip[] martianDialogs;


	// TODO Start methods
	public bool sceneStarted = false;
	private bool sceneFinished = false;

	public GameObject bridge;
	private AnimationCurve bridgeAnimation;

	// Phases :
	// "moveAroundUnstoppable"
	// "moveAround"
	// "rotateCamera"
	// "dialog01"
	// "rotateCameraToBridge"
	// "showBridge"
	private string currentPhase = "moveAroundUnstoppable";

	private float movingAwayDuration = 3.0f;
	private float movingAwayStart = 0.0f;

	// Minimum distance to trigger the sphere dialog scene 
	private float minDistanceSphereFromPlayer = 3.0f;
	private float lookRotationSpeed = 5.0f;

	private float fadeDuration = 1.0f;
	private float volumeReverb = 0.05f;
	private float volumeAmbiant = 0.1f;
	private AnimationCurve fadeSong;
	private AnimationCurve fadeReverb;
	private AnimationCurve fadeAmbiant;
	private AnimationCurve fadeSpatial;

	public GameObject nextSceneController;

	// Use this for initialization
	private void Start () {
		initializePositions ();

		martianSphereMovements = martianSphere.GetComponent<MartianSphereMovementController> ();
	}

	private void initializePositions() {
		spherePositions [0]  = new Vector3 (-40.47f, 10.6f, 334.52f);
		spherePositions [1]  = new Vector3 (-45.62f, 10.8f, 331.51f);
	}

	// Update is called once per frame
	void LateUpdate () {
		if (sceneStarted == true && sceneFinished == false) {
			Debug.Log ("BridgeScene01 : " + currentPhase);

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

				case "moveToBridge":
					phaseMoveToBridge();
				break;

				case "showBridge":
					phaseShowBridge ();
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
				bridge.SetActive(true);

				movingAwayStart = 0.0f;
				currentPhase = "moveToBridge";
			}
			else {
				// Use the GUI to play a sound and display subtitles
				UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
				uiScripting.PlayLineWithSubtitles(audioDialog, martianDialogs[currentMartianDialog]);

				currentMartianDialog++;
			}
		}
	}

	private void phaseMoveToBridge() {
		bool phaseIsOver = true;

		// Look at the martian sphere continuously
		Camera camera = character.gameObject.GetComponent<Camera>();
		camera.transform.LookAt (martianSphere.transform);

		Vector3 spherePositionOnBridge = new Vector3 (-42.12f, 12.33f, 355.04f);

		// Moving the sphere away from the player
		if (movingAwayStart == 0.0f) {
			martianSphereMovements.animateTo (spherePositionOnBridge, sphereMovingSpeed * 2);
			movingAwayStart = Time.time;

			phaseIsOver = false;
		} else {
			if ((Time.time - movingAwayStart) <= movingAwayDuration) {
				phaseIsOver = false;
			}
		}

		// Create the sound fading animations
		if (fadeSpatial == null) {
			//fadeSong = AnimationCurve.EaseInOut(Time.time, audioSong.volume, Time.time + fadeDuration, volumeSong);
			//fadeReverb = AnimationCurve.EaseInOut (Time.time, audioReverb.volume, Time.time + fadeDuration, 0);
			fadeAmbiant = AnimationCurve.EaseInOut (Time.time, audioAmbiant.volume, Time.time + fadeDuration, volumeAmbiant);
			fadeSpatial = AnimationCurve.EaseInOut (Time.time, 1, Time.time + fadeDuration, 0);
		}
		
		// Process the fadeIn/fadeOut animation
		//audioSong.volume = fadeSong.Evaluate (Time.time);
		//audioReverb.volume = fadeReverb.Evaluate (Time.time);
		audioAmbiant.volume = fadeAmbiant.Evaluate (Time.time);

		// Process the spatialization animation
		audioSong.spatialBlend = fadeSpatial.Evaluate (Time.time);
		audioReverb.spatialBlend = fadeSpatial.Evaluate (Time.time);
		audioAmbiant.spatialBlend = fadeSpatial.Evaluate (Time.time);
		
		// Check if sound fading is over
		if (audioSong.spatialBlend > 0) {
			phaseIsOver = false;
		}

		// Launch next phase
		if (phaseIsOver) {
			currentPhase = "showBridge";
		}
	}

	private void phaseShowBridge() {
		// Look at the martian sphere continuously
		Camera camera = character.gameObject.GetComponent<Camera>();
		camera.transform.LookAt (martianSphere.transform);

		Renderer bridgeRenderer = bridge.GetComponent<Renderer> ();

		// Creates the animation Curve
		if (bridgeAnimation == null) {
			bridgeAnimation = AnimationCurve.EaseInOut(Time.time, 0, Time.time + 3, 1);

			bridgeRenderer.material.EnableKeyword ("_EMISSION");
		}

		float progress = bridgeAnimation.Evaluate (Time.time);

		float targetAlpha = 0.5f;
		float targetEmission = 1.0f;

		// Albedo transition
		Color bridgeColor = bridgeRenderer.material.color;
		bridgeColor.a = progress * targetAlpha;
		bridgeRenderer.material.color = bridgeColor;

		// Emission transition
		Color bridgeEmission = new Color (progress * targetEmission, progress * targetEmission, progress * targetEmission);
		bridgeRenderer.material.SetColor ("_EmissionColor", bridgeEmission);

		if (progress >= 0.99f) {
			sceneFinished = true;
			nextSceneController.GetComponent<BridgeSceneControllerSecond>().sceneStarted = true;

			// Unlock the player's controls
			player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
		}
	}

}
