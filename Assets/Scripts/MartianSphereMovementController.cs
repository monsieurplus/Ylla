using UnityEngine;
using System.Collections;

public class MartianSphereMovementController : MonoBehaviour {

	public Terrain sphereShouldNeverBeUnderThisTerrain;

	public float sphereMinDistanceFromGround = 1.8f;
	public float sphereSpeed = 10.0f;
	
	private AnimationCurve movingCurveX;
	private AnimationCurve movingCurveY;
	private AnimationCurve movingCurveZ;
	private float movingEnd = 0.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		// Handles moving the sphere around
		if (isMoving()) {
			moveTo(
				movingCurveX.Evaluate(Time.time),
				movingCurveY.Evaluate(Time.time),
				movingCurveZ.Evaluate(Time.time)
			);
		}
	}

	/**
	 * Animate the object to a position
	 * //Takes care that the destination is not under the terrain
	 * speed : meters per seconds
	 */
	public void animateTo (float destinationX, float destinationY, float destinationZ, float speed) {
		Vector3 destination = new Vector3 (destinationX, destinationY, destinationZ);

		// Checks that the destination point is above the terrain
		//float terrainDestinationHeight = sphereShouldNeverBeUnderThisTerrain.SampleHeight (destination);

		// Correct the destination to be above the terrain
		//if ((terrainDestinationHeight + sphereMinDistanceFromGround) > destination.y) {
		//	destination.y = terrainDestinationHeight + sphereMinDistanceFromGround;
		//}

		float distance = Vector3.Distance (transform.position, destination);
		float movingDuration = distance / speed;
		float movingStart = Time.time;
		movingEnd = movingStart + movingDuration;

		// Creates an AnimationCurve to elegantly move the sphere around
		movingCurveX = AnimationCurve.EaseInOut (
			movingStart,
			transform.position.x,
			movingEnd,
			destination.x
		);

		movingCurveY = AnimationCurve.EaseInOut (
			movingStart,
			transform.position.y,
			movingEnd,
			destination.y
		);

		movingCurveZ = AnimationCurve.EaseInOut (
			movingStart,
			transform.position.z,
			movingEnd,
			destination.z
		);
	}

	public void animateTo(Vector3 destination, float speed) {
		animateTo (destination.x, destination.y, destination.z, speed);
	}

	/**
	 * Moves the object to a position
	 * Takes care not to position it under the terrain
	 */
	public void moveTo (float destinationX, float destinationY, float destinationZ) {
		Vector3 destination = new Vector3 (destinationX, destinationY, destinationZ);

		// Checks that the destination point is above the terrain
		if (sphereShouldNeverBeUnderThisTerrain != null) {
			float terrainPositionHeight = sphereShouldNeverBeUnderThisTerrain.transform.position.y;
			float terrainDestinationHeight = sphereShouldNeverBeUnderThisTerrain.SampleHeight (destination);
		
			// Correct the destination to be above the terrain
			if ((terrainDestinationHeight + terrainPositionHeight + sphereMinDistanceFromGround) > destination.y) {
				destination.y = terrainDestinationHeight + sphereMinDistanceFromGround;
			}
		}

		transform.position = destination;
	}

	public bool isMoving() {
		return (movingEnd >= Time.time && movingCurveX != null);
	}

	public float getMovingEnd() {
		return movingEnd;
	}

	public void stopMoving() {
		Debug.Log ("stopMoving");
		movingEnd = 0.0f;
	}
}
