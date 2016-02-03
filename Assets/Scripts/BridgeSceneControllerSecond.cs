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

	// Audio sources
	public AudioSource audioDialog;
	public AudioSource audioSong;
	public AudioSource audioReverb;
	public AudioSource audioAmbiant;

	// Allow to get player's position and change it
	public GameObject player; // FPSController (Capsule with FirstPersonController + RigidBody)
	public GameObject character; // FirstPersonCharacter (Camera)
	public PlayerCollisionManager playerCollision;

	// Allow to give orders to the sphere
	private MartianSphereMovementController martianSphereMovements;
	public float sphereMovingSpeed = 6.0f;
	public float spherePauseDuration = 1.0f;

	// Positions where the sphere must go
	private Vector3[] spherePositions = new Vector3[16];
	private int currentPosition = 0;

	// Dialogs that the sphere will tell the player
	private int currentMartianDialog = 0;
	public AudioClip[] martianDialogs;
	private float dialogPauseStart = 0f;
	public float dialogPauseDuration = 10.0f;

	// Phase during which the sphere goes to a position and is unstoppable (the player can't talk to her)
	private float movingAwayDuration = 3.0f;
	private float movingAwayStart = 0.0f;

	// Respawn configuration
	public AudioClip[] fallDialogs; // Respawn sentences
	public GameObject fallCollider; // Cube to check if the player has fallen
	private string fallPreviousPhase;
	private int fallDialog = -1; // Current fall dialog (if -1, it means no fall dialog is playing)

	public GameObject respawn;
	public GameObject[] checkpoints;
	//public Vector3 fallRespawnPosition = new Vector3 (-44.38f, 9.15f, 331.04f);
	//public Vector3 fallRespawnDirection = new Vector3 (-1.0f, 0f, 0f);

	// Minimum distance to trigger the sphere dialog scene 
	private float minDistanceSphereFromPlayer = 3.0f;
	private float lookRotationSpeed = 5.0f;

	// TODO Start methods
	public bool sceneStarted = false;
	private bool sceneFinished = false;

	// Volume fading configuration
	private float fadeDuration = 1.0f;
	//private float volumeSong = 0.1f;
	//private float volumeReverb = 0.05f;
	//private float volumeAmbiant = 0.1f;
	private float volumeDialog = 0.1f;
	private AnimationCurve fadeSong;
	private AnimationCurve fadeReverb;
	private AnimationCurve fadeAmbiant;
	private AnimationCurve fadeDialog;

	// Objects needed to check the puzzle
	public GameObject projector;
	public Renderer door;
	private FinalProjectorController projectorController;
	//private bool puzzleCompleted = false;

	// This cube has a trigger, if the player enters it and it's active, the demo is over.
	// It is disabled by default, so once the final puzzle is completed, we just have to enable this cube to trigger the demo ending.
	public GameObject endingCube;

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

	// Use this for initialization
	private void Awake () {
		initializePositions ();

		projectorController = projector.GetComponent<FinalProjectorController> ();
		martianSphereMovements = martianSphere.GetComponent<MartianSphereMovementController> ();

		playerCollision = player.GetComponent<PlayerCollisionManager> ();
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
			//Debug.Log ("BridgeScene02 : " + currentPhase);

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

				// Fall handling
				case "falling":
					phaseFalling ();
				break;

				case "respawning":
					phaseRespawning ();
				break;

				case "respawned":
					phaseRespawned ();
				break;
			}

			// Check if the player is falling
			if (currentPhase != "falling" && currentPhase != "respawning" && currentPhase != "respawned") {
				checkForFall ();
			}

			// Check if the player is going through a checkpoint
			checkCheckpoint ();
		}
	}

	private float getSphereDistanceFromPlayer() {
		return Vector3.Distance (martianSphere.transform.position, Camera.main.transform.position);
	}

	/**
	 * Checks if the player is going through a checkpoint
	 */
	private void checkCheckpoint () {
		for (int i=0; i < checkpoints.Length; i++) {
			if (playerCollision.isColliding (checkpoints[i])) {
				respawn = checkpoints[i];
				break;
			}
		}
	}

	/**
	 * Checks if the player is falling
	 */
	private void checkForFall () {
		if (playerCollision.isColliding (fallCollider)) {
			fallPreviousPhase = currentPhase;
			currentPhase = "falling";

			// Fading to black
			UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
			uiScripting.requestFadeToBlack(0.1f);
		}
	}

	private void phaseFalling() {
		if (fadeDialog == null) {
			// Create the fading
			fadeDialog = AnimationCurve.EaseInOut (Time.time, audioDialog.volume, Time.time + fadeDuration, 0f);

			// Lock the player's controls
			player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;

		} else {
			// Apply the fading
			audioDialog.volume = fadeDialog.Evaluate (Time.time);
			//audioAmbiant.volume = fadeAmbiant.Evaluate (Time.time);
		}

		if (audioDialog.volume == 0f) {
			// Stop the current dialog (it has already been faded out, it won't be strange)
			audioDialog.Stop ();

			// Reset dialog volume to its former level
			audioDialog.volume = volumeDialog;

			// Go to the next phase !
			currentPhase = "respawning";
		}

	}

	private void phaseRespawning() {
		Vector3 respawnPosition = respawn.transform.position;
		Quaternion respawnRotation = respawn.transform.rotation;

		Quaternion horizontalRot = Quaternion.Euler(0f, respawnRotation.y, 0f);
		Quaternion verticalRot = Quaternion.Euler(respawnRotation.x, 0f, 0f);

		UnityStandardAssets.Characters.FirstPerson.FirstPersonController playerScript;
		playerScript = player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ();
		playerScript.TeleportController(respawnPosition, horizontalRot, verticalRot);

		//Request screen white flash out of black
		UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
		uiScripting.requestWhiteFlashFromBlack();

		// Move Ylla in front of the player
		Vector3 spherePosition = respawnPosition + (respawn.transform.forward * 3f);
		martianSphereMovements.stopMoving ();
		martianSphereMovements.moveTo (spherePosition);

		currentPhase = "respawned";
	}

	private void phaseRespawned () {
		// Look at the martian sphere continuously
		Camera camera = character.gameObject.GetComponent<Camera>();
		camera.transform.LookAt (martianSphere.transform);

		if (fallDialog == -1) {
			// TODO choose a dialog RANDOMLY
			System.Random rnd = new System.Random();
			fallDialog = rnd.Next(0, fallDialogs.Length);

			// Play the randomly chosen dialog
			UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
			uiScripting.PlayLineWithSubtitles(audioDialog, fallDialogs[fallDialog]);

			// Lock player's controls
			player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
		} else {
			// If the dialog has finished playing, we give the player his freedom back
			if (!audioDialog.isPlaying) {
				// Unlock the player's controls
				player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;

				// Back to the previous phase
				//currentPhase = fallPreviousPhase;

				// Some minor tweaks depending of the previous phase
				switch (fallPreviousPhase)
				{
					case "waitingForPlayerOnBridge":
						if (movingAwayStart == 0f) {
							// Move the sphere to the waiting position
							Vector3 spherePositionOnBridge = new Vector3 (-42.12f, 12.33f, 355.04f);
							martianSphereMovements.animateTo (spherePositionOnBridge, sphereMovingSpeed * 2);

							movingAwayStart = Time.time;
						} else {
							if (Time.time - movingAwayStart >= movingAwayDuration) {
								movingAwayStart = 0f;
								currentPhase = fallPreviousPhase;

								// Dialog choice is back to -1
								fallDialog = -1;
							}
						}
					break;

					case "moveAroundAndDialog":
						// Repeat last dialog go to last position
						if (currentMartianDialog > 0) {
							currentMartianDialog--;
						}

						if (currentPosition > 0) {
							currentPosition--;
						}

						currentPhase = fallPreviousPhase;

						// Dialog choice is back to -1
						fallDialog = -1;
					break;

					case "waitingForPlayerAtHome":
						// Go to waiting place then wait
						if (movingAwayStart == 0f) {
							// Move the sphere to the waiting position
							Vector3 spherePositionBeforeHouse = new Vector3 (-160f, -50f, 800f);
							martianSphereMovements.animateTo (spherePositionBeforeHouse, sphereMovingSpeed * 2);

							movingAwayStart = Time.time;
						} else {
							if (Time.time - movingAwayStart >= movingAwayDuration) {
								movingAwayStart = 0f;
								currentPhase = fallPreviousPhase;

								// Dialog choice is back to -1
								fallDialog = -1;
							}
						}
						
					break;

					case "moveToHouse":
					case "checkPuzzle":
						// TODO Move to house (maybe, just setting currentPhase to "moveToHouse" would work)
						currentPhase = "moveToHouse";

						// Dialog choice is back to -1
						fallDialog = -1;
					break;
				}
			}
		}
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

			//Debug.Log (dialogPauseStart);


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
		
		Vector3 spherePositionInHouse = new Vector3 (-205f, -50f, 818f);
		
		// Moving the sphere away from the player
		if (movingAwayStart == 0.0f) {
			martianSphereMovements.animateTo (spherePositionInHouse, sphereMovingSpeed * 2);
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
}
