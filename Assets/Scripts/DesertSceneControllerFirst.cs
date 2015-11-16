using UnityEngine;
using System.Collections;

public class DesertSceneControllerFirst : MonoBehaviour {
	public GameObject martianSphere;
	private MartianSphereMovementController martianSphereMovements;

	public float movingSpeed = 5.0f;
	public float pauseDuration = 1.0f;
	
	private Vector3[] spherePositions = new Vector3[9];

	private int currentPosition = 0;

	// Use this for initialization
	private void Start () {
		initializePositions ();

		martianSphereMovements = martianSphere.GetComponent<MartianSphereMovementController> ();
	}

	// Update is called once per frame
	void LateUpdate () {
		// Check the distance between the sphere and the player
		if (getDistanceFromPlayer () <= 5) {
			martianSphereMovements.stopMoving ();

			// TODO Make the player look at the sphere
			// TODO Lock player's movements
			// TODO Launch Dialogue phase
		}

		// Check that the sphere is not moving and that it has made a pause
		else if (!martianSphereMovements.isMoving () &&
			(Time.time - martianSphereMovements.getMovingEnd ()) > pauseDuration) {


			// Moving the sphere to the next position
			currentPosition++;
			if (currentPosition >= spherePositions.Length) {
				Debug.Log ("currentPosition init");
				currentPosition = 0;
			}

			martianSphereMovements.AnimateTo (spherePositions [currentPosition], movingSpeed);
		}
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


	private float getDistanceFromPlayer() {
		return Vector3.Distance (martianSphere.transform.position, Camera.main.transform.position);
	}

}
