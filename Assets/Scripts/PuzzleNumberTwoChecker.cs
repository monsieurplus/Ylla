using UnityEngine;
using System.Collections;

public class PuzzleNumberTwoChecker : MonoBehaviour {
	// Objects needed to check the puzzle
	public GameObject projectorA;
	public GameObject projectorB;
	public GameObject projectorC;

	private ProjectorController projectorControllerA;
	private ProjectorController projectorControllerB;
	private ProjectorController projectorControllerC;

	public Renderer door;


	// If true, the puzzle's conditions will be checked every frame
	public bool checkingActive = false;

	private bool completed = false;

	// Use this for initialization
	void Start () {
		projectorControllerA = projectorA.GetComponent<ProjectorController> ();
		projectorControllerB = projectorB.GetComponent<ProjectorController> ();
		projectorControllerC = projectorC.GetComponent<ProjectorController> ();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (checkingActive && !completed && checkPuzzleCompletion ()) {
			reactToPuzzleCompletion();
			completed = true;
		}
	}

	public bool isCompleted () {
		return completed;
	}

	/**
	 * Checks if the puzzle has been completed
	 */
	private bool checkPuzzleCompletion () {
		if (projectorControllerA.getCurrentRotation () != 45) {
			return false;
		}
		if (!projectorControllerA.isLightActive ("blue")) {
			return false;
		}

		if (projectorControllerB.getCurrentRotation () != 0) {
			return false;
		}
		if (!projectorControllerB.isLightActive ("red")) {
			return false;
		}

		if (projectorControllerC.getCurrentRotation () != 0) {
			return false;
		}
		if (!projectorControllerC.isLightActive ("green")) {
			return false;
		}

		return true;
	}

	/**
	 * Reaction when the puzzle has been completed
	 */
	private void reactToPuzzleCompletion() {
		// Make the door half-transparent
		door.material.color = new Color(
			door.material.color.r,
			door.material.color.g,
			door.material.color.b,
			0.75f
		);

		// Disables the box collider so the player can go through the door
		door.GetComponent<Collider> ().enabled = false;
	}
}
