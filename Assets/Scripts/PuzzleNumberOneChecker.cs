using UnityEngine;
using System.Collections;

public class PuzzleNumberOneChecker : MonoBehaviour {
	// Objects needed to check the puzzle
	public GameObject projector;
	public Renderer door;
	private ProjectorController projectorController;

	// If true, the puzzle's conditions will be checked every frame
	public bool checkingActive = false;

	private bool completed = false;

	// Use this for initialization
	void Start () {
		projectorController = projector.GetComponent<ProjectorController> ();
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
		if (projectorController.getCurrentRotation () != 0) {
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
