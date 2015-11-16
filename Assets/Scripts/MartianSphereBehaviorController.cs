using UnityEngine;
using System.Collections;

public class MartianSphereBehaviorController : MonoBehaviour {

	/**
	 * Breathing is visually showed by a change in the Y position of the sphere
	 * Heartbeating is visually showed by a change of the ligthing intensity
	 */

	public float breathingSpeed = 1.0f;
	public float breathingAmplitude = 0.1f;

	// We save the current translation to be able to only slightly move it
	private float breathingCurrentHeight = 0.0f;

	// Time (in seconds) between 2 heartbeats
	public float heartbeatInterval = 1.0f;

	public float heartbeatLightAmplitude = 1.0f;
	public float heartbeatFlareAmplitude = 0.25f;


	public float heartbeatLightIntensity = 5.0f;
	public float heartbeatFlareBrightness = 1.0f;
	private float heartbeatFlareBrightnessDefault; 

	private float[] heartbeatSequence = {
		// First beat
		0.0f, 0.5f , 1.0f, -0.5f,
		0.0f, 0.25f, 0.0f,  0.0f,
		// Second beat
		0.0f, 0.5f , 1.0f, -0.5f,
		0.0f, 0.25f, 0.0f,  0.0f,

		// Pause before the next sequence
		0.0f, 0.0f, 0.0f, 0.0f,
		0.0f, 0.0f, 0.0f, 0.0f,
		0.0f, 0.0f, 0.0f, 0.0f,
		0.0f, 0.0f, 0.0f, 0.0f,
	};


	// Use this for initialization
	void Start () {
		heartbeatFlareBrightnessDefault = heartbeatFlareBrightness;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateFlareSizeFromDistanceAndObstacles ();
		
		UpdateBreathing ();
		UpdateHeartbeat ();
	}

	/**
	 * Changes the flare's size as its distance grows
	 * Hides the flare when it's hidden behind an obstacle
	 */
	void UpdateFlareSizeFromDistanceAndObstacles () {
		// Get distance between the object and the camera
		float distance = Vector3.Distance (transform.position, Camera.main.transform.position);

		// Checks if the flare's hidden behind an object
		Vector3 heading = Camera.main.transform.position - transform.position;

		bool obstacleDetected = Physics.Raycast(
			transform.position,
			heading.normalized,
			distance-1
		);

		if (obstacleDetected == true) {
			// The flare is hidden behind an object, it should be hidden
			heartbeatFlareBrightness = 0;
		}
		else {
			// Update the flares brightness when distance changes
			heartbeatFlareBrightness = heartbeatFlareBrightnessDefault / Mathf.Sqrt (distance);
		}
	}

	/**
	 * Handles the breathing effect
	 */ 
	void UpdateBreathing () {
		// Compute the new translation
		float breathingNewHeight = breathingAmplitude * Mathf.Cos (Time.time * breathingSpeed);
		
		// Compute the delta between current and new translation to just move the sphere with the delta
		float breathingDelta = breathingCurrentHeight - breathingNewHeight;
		
		// We save the new translation
		breathingCurrentHeight = breathingNewHeight;
		
		// Moving the sphere
		transform.Translate(new Vector3(0, breathingDelta, 0));
	}

	/**
	 * Handles the heartbeat effect
	 */
	void UpdateHeartbeat () {
		// Included in [0-1] tells how much time has passed since the beggining of the sequence
		float timePosition = Time.time % heartbeatInterval;

		// Determine where we are in the sequence (index)
		float sequencePartDuration = heartbeatInterval / ((float) heartbeatSequence.Length);
		float sequencePosition = timePosition / sequencePartDuration;
		
		int sequencePositionPrevious = (int) Mathf.Floor (sequencePosition);
		int sequencePositionNext = (int) Mathf.Ceil (sequencePosition);
		if (sequencePositionNext >= heartbeatSequence.Length) {
			sequencePositionNext = 0;
		}

		// Interpolation to detect which value exactly we should have
		// Should give a value included in [0-1]
		float sequencePositionInterpolation = sequencePosition - ((float) sequencePositionPrevious);
		
		float heartbeatInterpolation = (heartbeatSequence [sequencePositionNext] - heartbeatSequence [sequencePositionPrevious]) * sequencePositionInterpolation;
		float heartbeatValue = heartbeatSequence [sequencePositionPrevious] + heartbeatInterpolation;
		
		// Changes the light intensity considering the heartbeat value
		Light sphereLight = GetComponent<Light> ();
		sphereLight.intensity = heartbeatLightIntensity + heartbeatValue * heartbeatLightAmplitude;
		
		LensFlare sphereFlare = GetComponent<LensFlare> ();
		sphereFlare.brightness = heartbeatFlareBrightness + heartbeatValue * heartbeatFlareAmplitude * heartbeatFlareBrightness;
	}
}
