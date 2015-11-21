using UnityEngine;
using System.Collections;

// During this scene :
// - The martian sphere moves around the scenery
// - When the player get close to the sphere, he automatically look at it
// - The sphere speaks to the player
// - The sphere leaves the player and goes away
public class BridgeSceneControllerSecond : MonoBehaviour {
	public Canvas guiCanvas;

	public GameObject martianSphere;

	public AudioSource audioDialog;
	public AudioSource audioSong;
	public AudioSource audioReverb;
	public AudioSource audioAmbiant;

	private MartianSphereMovementController martianSphereMovements;

	public GameObject player;
	public GameObject character;

	public float sphereMovingSpeed = 6.0f;
	public float spherePauseDuration = 1.0f;
	
	private Vector3[] spherePositions = new Vector3[16];

	private int currentPosition = 0;

	private int currentMartianDialog = 0;
	[SerializeField] AudioClip[] martianDialogs;
	private float dialogPauseStart = 0f;
	public float dialogPauseDuration = 10.0f;


	// TODO Start methods
	public bool sceneStarted = false;
	private bool sceneFinished = false;

	// Phases :
	// "waitingForPlayerOnBridge"
	// "moveAroundAndDialog"
	// "waitingForPlayerAtHome"
	// "rotateCamera"
	// "dialog01"
	// "moveToHouse"
	// "checkPuzzle"
	// "ending"
	private string currentPhase = "waitingForPlayerOnBridge";

	private float movingAwayDuration = 3.0f;
	private float movingAwayStart = 0.0f;

	// Minimum distance to trigger the sphere dialog scene 
	private float minDistanceSphereFromPlayer = 3.0f;
	private float lookRotationSpeed = 5.0f;

	private float fadeDuration = 1.0f;
	//private float volumeSong = 0.1f;
	//private float volumeReverb = 0.05f;
	private float volumeAmbiant = 0.1f;
	private AnimationCurve fadeSong;
	private AnimationCurve fadeReverb;
	private AnimationCurve fadeAmbiant;

	// Objects needed to check the puzzle
	public GameObject projector;
	public Renderer door;
	private FinalProjectorController projectorController;
	private bool puzzleCompleted = false;

	public GameObject endingCube;

	// Use this for initialization
	private void Awake () {
		initializePositions ();

		projectorController = projector.GetComponent<FinalProjectorController> ();
		martianSphereMovements = martianSphere.GetComponent<MartianSphereMovementController> ();
	}

	private void initializePositions() {
		spherePositions [0] = new Vector3 (-42.12f, 17.21f, 388.37f);
		spherePositions [1] = new Vector3 (-42.62f, 17.7f, 413.5f);
		spherePositions [2] = new Vector3 (-46.23f, 14.4f, 439.11f);
		spherePositions [3] = new Vector3 (-60.9f, 8.44f, 461.1f);
		spherePositions [4] = new Vector3 (-98.6f, -7.96f, 503.6f);
		spherePositions [5] = new Vector3 (-98.6f, -18.36f, 535.8f);
		spherePositions [6] = new Vector3 (-69.58f, -20.58f, 573.9f);
		spherePositions [7] = new Vector3 (-66.3f, -1.13f, 643.5f);
		spherePositions [8] = new Vector3 (-66.7f, -20.9f, 700f);
		spherePositions [9] = new Vector3 (-16.45f, -19.5f, 702.8f);
		spherePositions [10] = new Vector3 (24.6f, -7.62f, 678.8f);
		spherePositions [11] = new Vector3 (95.5f, -29f, 704.7f);
		spherePositions [12] = new Vector3 (97.13f, -18.6f, 751f);
		spherePositions [13] = new Vector3 (8.24f, -34f, 773f);
		spherePositions [14] = new Vector3 (-22.6f, -34f, 763f);
		spherePositions [15] = new Vector3 (-160f, -50f, 800f);
	}

	// Update is called once per frame
	void LateUpdate () {
		if (sceneStarted == true && sceneFinished == false) {
			Debug.Log ("BridgeScene02 : " + currentPhase);

			switch (currentPhase) {
				case "waitingForPlayerOnBridge":
						phaseWaitingForPlayerOnBridge();	
				break;

				case "moveAroundAndDialog":
					phaseMoveAroundAndDialog();
				break;

				case "waitingForPlayerAtHome":
					phaseWaitingForPlayerAtHome();
				break;

				case "rotateCamera":
					phaseRotateCamera();
				break;

				case "dialog01":
					phaseDialogOne();
				break;

				case "moveToHouse":
					phaseMoveToHouse ();
				break;

				case "checkPuzzle":
					phaseCheckPuzzle();
				break;

				case "changeDoor":
					phaseChangeDoor ();
				break;

				case "ending":
					phaseEnding ();
				break;
			}
		}
	}

	private float getSphereDistanceFromPlayer() {
		return Vector3.Distance (martianSphere.transform.position, Camera.main.transform.position);
	}

	private void phaseWaitingForPlayerOnBridge() {
		if (getSphereDistanceFromPlayer () <= minDistanceSphereFromPlayer) {
			currentPhase = "moveAroundAndDialog";
		}
	}

	private void phaseMoveAroundAndDialog() {
		bool finishedDialog = false;
		bool finishedMoving = false;

		// Give the sphere a destination regulary
		if (!martianSphereMovements.isMoving () && (Time.time - martianSphereMovements.getMovingEnd ()) > spherePauseDuration) {
			// Moving the sphere to the next position
			currentPosition++;
			if (currentPosition >= spherePositions.Length) {
				finishedMoving = true;
			}
			else {
				martianSphereMovements.animateTo (spherePositions [currentPosition], sphereMovingSpeed);
			}
		}

		if (!audioDialog.isPlaying) {
			if (dialogPauseStart == 0.0f) {
				dialogPauseStart = Time.time;
			}

			Debug.Log (dialogPauseStart);


			if ((Time.time - dialogPauseStart) >= dialogPauseDuration) {
				if (currentMartianDialog >= 8 || currentMartianDialog >= martianDialogs.Length) {
					finishedDialog = true;
				}
				else {
					// Use the GUI to play a sound and display subtitles
					UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
					uiScripting.PlayLineWithSubtitles(audioDialog, martianDialogs[currentMartianDialog]);

					dialogPauseStart = 0f;

					currentMartianDialog++;
				}
			}

		}

		if (finishedMoving && finishedDialog) {
			currentPhase = "waitingForPlayerAtHome";
		}
		
		// Check the distance between the sphere and the player 
		/*if (getSphereDistanceFromPlayer () <= minDistanceSphereFromPlayer) {
			// We change the trigger distance so that the player doesn't exit the trigger zone while rotating
			minDistanceSphereFromPlayer--;
			
			// Lock player's movements
			player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
			
			// Stop the sphere's movements
			martianSphereMovements.stopMoving ();
			
			// Change phase
			currentPhase = "rotateCamera";
		}*/
	}

	private void phaseWaitingForPlayerAtHome() {
		if (getSphereDistanceFromPlayer () <= minDistanceSphereFromPlayer) {
			// Lock player's movements
			player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;

			// Change phase
			currentPhase = "rotateCamera";
		}
	}

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

	private void phaseDialogOne() {
		// Look at the martian sphere continuously
		Camera camera = character.gameObject.GetComponent<Camera>();
		camera.transform.LookAt (martianSphere.transform);
		
		if (!audioDialog.isPlaying) {
			
			if (currentMartianDialog >= 10 || currentMartianDialog >= martianDialogs.Length) {
				currentPhase = "moveToHouse";
			}
			else {
				// Use the GUI to play a sound and display subtitles
				UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
				uiScripting.PlayLineWithSubtitles(audioDialog, martianDialogs[currentMartianDialog]);
				
				currentMartianDialog++;
			}
		}
	}

	private void phaseMoveToHouse() {
		// Look at the martian sphere continuously
		Camera camera = character.gameObject.GetComponent<Camera>();
		camera.transform.LookAt (martianSphere.transform);
		
		Vector3 spherePositionOnBridge = new Vector3 (-205f, -50f, 818f);
		
		// Moving the sphere away from the player
		if (movingAwayStart == 0.0f) {
			martianSphereMovements.animateTo (spherePositionOnBridge, sphereMovingSpeed * 2);
			movingAwayStart = Time.time;
		} else {
			if ((Time.time - movingAwayStart) <= movingAwayDuration) {
				// Lock player's movements
				player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;

				currentPhase = "checkPuzzle";
			}
		}
	}

	private void phaseCheckPuzzle() {
		if (checkPuzzleCompletion ()) {
			currentPhase = "changeDoor";
		}
	}

	private bool checkPuzzleCompletion() {
		if (projectorController.getCurrentRotation () != 225) {
			return false;
		}
		if (!projectorController.isLightActive ("red")) {
			return false;
		}
		if (!projectorController.isLightActive ("green")) {
			return false;
		}
		if (!projectorController.isLightActive ("blue")) {
			return false;
		}
		
		return true;
	}

	private void phaseChangeDoor() {
		door.material.color = new Color (0f, 0f, 0f);

		currentPhase = "ending";
	}

	private void phaseEnding() {
		endingCube.SetActive (true);
	}

	/*
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

	private void phaseDialogOne() {
		// Look at the martian sphere continuously
		Camera camera = character.gameObject.GetComponent<Camera>();
		camera.transform.LookAt (martianSphere.transform);

		if (!martianTelepathy.isPlaying) {

			if (currentMartianDialog >= 6 || currentMartianDialog >= martianDialogs.Length) {
				bridge.SetActive(true);
				currentPhase = "rotateCameraToBridge";
			}
			else {
				// Use the GUI to play a sound and display subtitles
				UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
				uiScripting.PlayLineWithSubtitles(martianTelepathy, martianDialogs[currentMartianDialog]);

				currentMartianDialog++;
			}
		}
	}

	private void phaseRotateCameraToBridge() {
		// Make the player look at the sphere
		Camera camera = character.gameObject.GetComponent<Camera>();
		
		Quaternion lookRotation = Quaternion.LookRotation(bridge.transform.position - camera.transform.position);
		camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
		
		// If the remaining angle is little, we change phase
		
		float deltaAngle = Quaternion.Angle(camera.transform.rotation, lookRotation);
		if (deltaAngle < 1.0f) {
			movingAwayStart = 0.0f;
			currentPhase = "moveToBridge";
		}
	}

	private void phaseMoveToBridge() {
		Vector3 spherePositionOnBridge = new Vector3 (-42.07f, 15.56f, 356.16f);

		// Moving the sphere away from the player
		if (movingAwayStart == 0.0f) {
			martianSphereMovements.animateTo (spherePositionOnBridge, sphereMovingSpeed * 2);
			movingAwayStart = Time.time;
		} else {
			if ((Time.time - movingAwayStart) > movingAwayDuration) {
				currentPhase = "showBridge";
			}
		}
	}

	private void phaseShowBridge() {
		Renderer bridgeRenderer = bridge.GetComponent<Renderer> ();

		// Creates the animation Curve
		if (bridgeAnimation == null) {
			bridgeAnimation = AnimationCurve.EaseInOut(Time.time, 0, Time.time + 3, 1);

			bridgeRenderer.material.EnableKeyword ("_EMISSION");
		}

		float progress = bridgeAnimation.Evaluate (Time.time);

		//float targetAlpha = 0.5f;
		float targetEmission = 0.25f;

		//Color bridgeColor = bridgeRenderer.material.color;
		//bridgeColor.a = progress * targetAlpha;
		//bridgeRenderer.material.color = bridgeColor;

		Color bridgeEmission = new Color (progress * targetEmission, progress * targetEmission, progress * targetEmission);
		bridgeRenderer.material.SetColor ("_EmissionColor", bridgeEmission);

		if (progress >= 1.0f) {
			// TODO Launch next phase
			sceneFinished = true;
			nextSceneController.GetComponent<BridgeSceneControllerSecond>().sceneStarted = true;

			// Unlock the player's controls
			player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
		}
	}*/

}
