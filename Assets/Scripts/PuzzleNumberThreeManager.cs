using UnityEngine;
using System.Collections;

public class PuzzleNumberThreeManager : MonoBehaviour {
	// Objects needed to check the puzzle
	public GameObject projectorA;
	public GameObject projectorB;
	public GameObject projectorC;
	public GameObject projectorD;

	public GameObject projectorParticlesC;

	// Cube that will fall on projector C to disable it
	public GameObject cubeC;

	private ProjectorController projectorControllerA;
	private ProjectorController projectorControllerB;
	private ProjectorController projectorControllerC;
	private ProjectorController projectorControllerD;

	public Renderer door;


	// If true, the puzzle's conditions will be checked every frame
	public bool checkingActive = false;

	private bool puzzleCompleted = false;
	private bool actionZoneActivatedB = false;
	private bool actionZoneActivatedC = false;

	private AnimationCurve projectorAnimationB = null;
	private AnimationCurve projectorAnimationC = null;

	// Use this for initialization
	void Start () {
		projectorControllerA = projectorA.GetComponent<ProjectorController> ();
		projectorControllerB = projectorB.GetComponent<ProjectorController> ();
		projectorControllerC = projectorC.GetComponent<ProjectorController> ();
		projectorControllerD = projectorD.GetComponent<ProjectorController> ();
	}

	void Update() {
		animateProjectorB ();
		animateProjectorC ();
	}

	// Update is called once per frame
	bool LateUpdate () {
		if (!checkingActive) {
			return false;
		}

		if (!actionZoneActivatedB) {
			if (checkActionZoneB()) {
				reactToActionZoneB();
				actionZoneActivatedB = true;
			}
		}

		if (!actionZoneActivatedC) {
			if (checkActionZoneC()) {
				reactToActionZoneC();
				actionZoneActivatedC = true;
			}
		}

		if (actionZoneActivatedB && actionZoneActivatedC && !puzzleCompleted) {
			if (checkPuzzleCompletion()) {
				reactToPuzzleCompletion();
				puzzleCompleted = true;
			}
		}

		return true;
	}

	public bool isCompleted () {
		return puzzleCompleted;
	}

	/**
	 * Detect if the action zone B is lighten with the good color (blue)
	 */
	private bool checkActionZoneB() {
		return (
			projectorControllerA.getCurrentRotation () == 270 &&
			!projectorControllerA.isLightActive("red") &&
			!projectorControllerA.isLightActive("green") &&
			projectorControllerA.isLightActive("blue")
		);
	}

	/**
	 * Reaction when the action zone B is lighten with the good color (blue)
	 */
	private void reactToActionZoneB() {
		projectorAnimationB = AnimationCurve.EaseInOut (
			Time.time,
			projectorB.transform.position.y,
			Time.time + 3,
			1.69038f
		);
	}

	/**
	 * Handles the animation of the projector C (while disabling itself)
	 */
	private bool animateProjectorB() {
		if (projectorAnimationB == null) {
			return false;
		}
		
		float animationEvaluation = projectorAnimationB.Evaluate (Time.time);

		projectorB.transform.position = new Vector3(
			projectorB.transform.position.x,
			animationEvaluation,
			projectorB.transform.position.z
		);

		if (animationEvaluation >= 1.69038f) {
			// Rise the zoneC flag
			actionZoneActivatedB = true;
			
			// Stop the AnimationCurve
			projectorAnimationB = null;
		}

		return true;
	}

	/**
	 * Detect if the action zone C is lighten with the good color (red)
	 */
	private bool checkActionZoneC() {
		return (
			projectorControllerD.getCurrentRotation () == 270 &&
			projectorControllerD.isLightActive("red") &&
			!projectorControllerD.isLightActive("green") &&
			!projectorControllerD.isLightActive("blue")
		);
	}

	/**
	 * Reaction when the action zone B is lighten with the good color (red)
	 */
	private void reactToActionZoneC() {
		cubeC.SetActive (true);

		// Simple linear animation
		projectorAnimationC = AnimationCurve.Linear (
			Time.time,
			0,
			(Time.time + 0.5f),
			100
		);
	}

	/**
	 * Handles the animation of the projector C (while disabling itself)
	 */
	private bool animateProjectorC() {
		if (projectorAnimationC == null) {
			return false;
		}

		if (projectorAnimationC.Evaluate (Time.time) >= 100) {
			// Turn the projector off
			projectorControllerC.togglingIsEnabled = true;
			projectorControllerC.turnOffAll ();
			projectorControllerC.togglingIsEnabled = false;

			// Move the projector down
			projectorC.transform.Translate (new Vector3 (0, -0.25f, 0));

			// Stop the particles
			projectorParticlesC.SetActive (false);

			// Rise the zoneC flag
			actionZoneActivatedC = true;

			// Stop the AnimationCurve
			projectorAnimationC = null;
		}

		return true;
	}

	/**
	 * Checks if the puzzle has been completed
	 */
	private bool checkPuzzleCompletion () {
		// If the two preliminary actions have not been completed, the puzzle won't be completed
		if (!actionZoneActivatedB || !actionZoneActivatedC) {
			return false;
		}

		// Projector A : O° Magenta (R+B)
		if (projectorControllerA.getCurrentRotation () != 0) {
			return false;
		}
		if (!projectorControllerA.isLightActive ("red") || projectorControllerA.isLightActive ("green") || !projectorControllerA.isLightActive ("blue")) {
			return false;
		}

		// Projector B : 315° Cyan (G+B)
		if (projectorControllerB.getCurrentRotation () != 315) {
			return false;
		}
		if (projectorControllerB.isLightActive ("red") || !projectorControllerB.isLightActive ("green") || !projectorControllerB.isLightActive ("blue")) {
			return false;
		}

		// Projector C : Black (no light) resolved by actionZone C

		// Projector D : 0° Yellow (R+G)
		if (projectorControllerD.getCurrentRotation () != 0) {
			return false;
		}
		if (!projectorControllerD.isLightActive ("red") || !projectorControllerD.isLightActive ("green") || projectorControllerD.isLightActive ("blue")) {
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
