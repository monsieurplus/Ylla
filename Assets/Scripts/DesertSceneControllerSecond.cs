﻿using UnityEngine;
using System.Collections;

// During this scene :
// - The martian sphere moves around the scenery
// - When the player get close to the sphere, he automatically look at it
// - The sphere speaks to the player
// - The sphere leaves the player and goes away
public class DesertSceneControllerSecond : MonoBehaviour {
	public Canvas guiCanvas;

	public GameObject martianSphere;
	public AudioSource martianTelepathy;
	private MartianSphereMovementController martianSphereMovements;

	public GameObject player;
	public GameObject character;

	public float sphereMovingSpeed = 5.0f;
	public float spherePauseDuration = 1.0f;
	
	private Vector3[] spherePositions = new Vector3[14];

	private int currentPosition = 0;

	private int currentMartianDialog = 0;
	[SerializeField] AudioClip[] martianDialogs;


	// TODO Start methods
	public bool sceneStarted = false;
	private bool sceneFinished = false;

	// Phases :
	// "moveAroundUnstoppable"
	// "moveAround"
	// "rotateCamera"
	// "dialog01"
	private string currentPhase = "moveAroundUnstoppable";

	private float movingAwayDuration = 3.0f;
	private float movingAwayStart = 0.0f;

	// Minimum distance to trigger the sphere dialog scene 
	private float minDistanceSphereFromPlayer = 3.0f;
	private float lookRotationSpeed = 5.0f;

	public GameObject nextSceneController;

	// Use this for initialization
	private void Start () {
		initializePositions ();

		martianSphereMovements = martianSphere.GetComponent<MartianSphereMovementController> ();
	}

	private void initializePositions() {
		spherePositions [0]  = new Vector3 (-49.77f, 6.77f, -39.07f);
		spherePositions [1]  = new Vector3 (  7.36f, 4.09f,  -2.19f);
		spherePositions [2]  = new Vector3 ( 17.69f, 3.53f,   1.81f);
		spherePositions [3]  = new Vector3 ( 20.43f, 3.53f,  10.03f);
		spherePositions [4]  = new Vector3 ( 14.59f, 3.53f,  13.39f);
		spherePositions [5]  = new Vector3 ( 31.48f, 3.89f,  10.19f);
		spherePositions [6]  = new Vector3 ( -3.91f, 6.65f, -35.11f);
		spherePositions [7]  = new Vector3 (-24.25f, 1.18f,  56.94f);
		spherePositions [8]  = new Vector3 (-25.74f, 1.18f, -62.86f);
		spherePositions [9]  = new Vector3 (-30.84f, 3.55f, -57.00f);
		spherePositions [10] = new Vector3 (-69.18f, 2.69f, -25.46f);
		spherePositions [11] = new Vector3 (-96.74f, 1.62f,   2.83f);
		spherePositions [12] = new Vector3 (-96.74f, 5.75f, -10.78f);
		spherePositions [13] = new Vector3(-103.75f, 1.24f, -34.88f);
	}

	// Update is called once per frame
	void LateUpdate () {
		//Debug.Log (currentPhase);

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
				// Stop current scene / Launch next Scene
				sceneFinished = true;
				nextSceneController.GetComponent<DesertSceneControllerThird>().sceneStarted = true;
				
				// Unlock the player's controls
				player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
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
				// Stop current scene / Launch next Scene
				sceneFinished = true;
				nextSceneController.GetComponent<DesertSceneControllerThird>().sceneStarted = true;
				
				// Unlock the player's controls
				player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
			}
			else {
				// Use the GUI to play a sound and display subtitles
				UI_Scripting uiScripting = guiCanvas.GetComponent<UI_Scripting>();
				uiScripting.PlayLineWithSubtitles(martianTelepathy, martianDialogs[currentMartianDialog]);
				
				currentMartianDialog++;
			}
		}
	}
	*/
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
