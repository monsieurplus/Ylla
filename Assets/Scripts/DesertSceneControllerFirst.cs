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
	public AudioSource martianTelepathy;
	private MartianSphereMovementController martianSphereMovements;



	public GameObject player;
	public GameObject character;

	public float sphereMovingSpeed = 5.0f;
	public float spherePauseDuration = 1.0f;
	
	private Vector3[] spherePositions = new Vector3[9];

	private int currentPosition = 0;

	private bool cameraIsRotating = false;

	private int currentMartianDialog = 0;
	[SerializeField] AudioClip[] martianDialogs;


	// TODO Start methods
	public bool sceneStarted = true;
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
	private Quaternion lookRotationThreshold = new Quaternion(0.1f, 0.1f, 0.1f, 0.1f);

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
		//Debug.Log (currentPhase);

		if (sceneStarted == true && sceneFinished == false) {
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

			if (currentMartianDialog >= 5 || currentMartianDialog >= martianDialogs.Length) {
				currentPhase = "waitingForPlayer";
			}
			else {
				// Use the GUI to play a sound and display subtitles
				UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
				uiScripting.PlayLineWithSubtitles(martianTelepathy, martianDialogs[currentMartianDialog]);

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
		
		
		if (!martianTelepathy.isPlaying) {
			
			if (currentMartianDialog >= 10 || currentMartianDialog >= martianDialogs.Length) {
				currentPhase = "waitingForPlayer";
				// TODO Launch next Scene
			}
			else {
				// Use the GUI to play a sound and display subtitles
				UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
				uiScripting.PlayLineWithSubtitles(martianTelepathy, martianDialogs[currentMartianDialog]);
				
				currentMartianDialog++;
			}
		}
	}

	/*
	// Check the distance between the sphere and the player
	if (getDistanceFromPlayer () <= 3) {
		martianSphereMovements.stopMoving ();

		//if (cameraIsRotating == false) {

			// Lock player's movements
			player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
			
			// Make the player look at the sphere
			Camera camera = character.gameObject.GetComponent<Camera>();

			var rotation = Quaternion.LookRotation(martianSphere.transform.position - camera.transform.position);
			camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, rotation, Time.deltaTime * 5.0f);
			cameraIsRotating = true;

			//var rotation = Quaternion.LookRotation(target.position - transform.position);
			//transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
		//}
		//else {
			// LookAt
			//camera.transform.LookAt (martianSphere.transform.position);
		//}
		// TODO Launch Dialogue phase


	}
	
	// Check that the sphere is not moving and that it has made a pause
	else if (!martianSphereMovements.isMoving () && (Time.time - martianSphereMovements.getMovingEnd ()) > spherePauseDuration) {
		// Moving the sphere to the next position
		currentPosition++;
		if (currentPosition >= spherePositions.Length) {
			currentPosition = 0;
		}
		
		martianSphereMovements.animateTo (spherePositions [currentPosition], sphereMovingSpeed);
	}
 	*/
}
