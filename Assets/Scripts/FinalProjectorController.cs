using UnityEngine;
using System.Collections;

public class FinalProjectorController : MonoBehaviour {
	
	public Renderer rotator;

	public bool redIsActive = false;
	public bool redIsEnabled = false;
	public Renderer redSpot;
	public Light redLight;

	public bool greenIsActive = false;
	public bool greenIsEnabled = false;
	public Renderer greenSpot;
	public Light greenLight;

	public bool blueIsActive = false;
	public bool blueIsEnabled = false;
	public Renderer blueSpot;
	public Light blueLight;

	// Properties
	public char rotationAxis = 'y';
	public float rotationStart = 0f;

	public float lightIntensity = 5f;
	public float flareBrightness = 1f;

	// Specify if only one spot can be active at once
	public bool onlyOneSpotAtOnce = false;

	// Specify if the spot can be rotated
	public bool togglingIsEnabled = true;
	public bool rotationIsEnabled = true;

	// Specify if an active spot will only stay active for a limited duration
	public bool timedLight = false;
	public float timedLightDuration = 3f;
	private float timedLightStart = 0f;

	private float currentRotation = 0f;
	private AnimationCurve rotationCurve = null;
	private float rotationCurveEndValue;

	// Use this for initialization
	void Start () {
		// Initialize the emissive material
		redSpot.material.EnableKeyword ("_EMISSION");
		greenSpot.material.EnableKeyword ("_EMISSION");
		blueSpot.material.EnableKeyword ("_EMISSION");

		// During initialization, we enable toggling (turning lights on/off)
		bool togglingWasEnabled = togglingIsEnabled;
		togglingIsEnabled = true;

		// Initialize the spots
		if (!redIsEnabled) {
			disableLight("red");
		} else if (redIsActive) {
			turnOnLight("red");
		}

		if (!greenIsEnabled) {
			disableLight("green");
		} else if (greenIsActive) {
			turnOnLight ("green");
		}

		if (!blueIsEnabled) {
			disableLight("blue");
		} else if (blueIsActive) {
			turnOnLight ("blue");
		}

		// Initialize rotation
		RotateTo (rotationStart);
		currentRotation = rotationStart;

		// Reset the toggling authorization to its former state
		togglingIsEnabled = togglingWasEnabled;
	}
	
	// Update is called once per frame
	void Update () {
		// Animate the rotator
		if (rotationIsEnabled) {
			AnimateRotator ();
			AnimateRotation ();
		}

		AnimateSpots ();

		if (timedLight) {
			if (timedLightStart != 0 && Time.time > (timedLightStart + timedLightDuration)) {
				timedLightStart = 0f;
				turnOffAll();
			}
		}
	}

	/**
	 * Animates the Rotator button
	 */
	private void AnimateRotator() {
		//rotator.transform.Rotate (0, 1, 0);
	}

	private void AnimateSpots() {
		redSpot.transform.Rotate (-1, 1, 1);
		blueSpot.transform.Rotate (1, -1, 1);
		greenSpot.transform.Rotate (1, 1, -1);
	}

	/**
	 * Rotate the projector to a specific angle
	 */
	private void RotateTo (float angle) {
		if (rotationAxis == 'x') {
			transform.eulerAngles = new Vector3(angle, 0, 0);
		} else if (rotationAxis == 'y') {
			transform.eulerAngles = new Vector3(0, angle, 0);
		} else {
			transform.eulerAngles = new Vector3(0, 0, angle);
		}
	}

	/**
	 * Rotate the projector of 45°
	 */
	public bool Rotate () {
		if (!rotationIsEnabled) {
			return false;
		}

		// Checks if there is a rotation going on
		if (rotationCurve != null) {
			return false;
		}

		// Computes the new rotation angle
		/*float startValue;

		if (rotationAxis == 'x') {
			startValue = transform.rotation.eulerAngles.x;
		} else if (rotationAxis == 'y') {
			startValue = transform.rotation.eulerAngles.y;
		} else {
			startValue = transform.rotation.eulerAngles.z;
		}*/

		rotationCurveEndValue = Mathf.Round(currentRotation + 45f);

		// Creates a rotation curve to animate the projector
		rotationCurve = AnimationCurve.EaseInOut(
			Time.time, 
			currentRotation, 
			Time.time + 0.5f, 
			rotationCurveEndValue
		);

		// Local memory of the rotation (used to check if the answer is correct)
		currentRotation = rotationCurveEndValue % 360;

		return true;
	}

	/**
	 * Process an AnimationCurve to rotate the projector smoothly
	 * Called every frame to animate a rotation
	 */
	private void AnimateRotation() {
		if (rotationCurve != null) {
			float rotationValue = rotationCurve.Evaluate(Time.time);

			RotateTo (rotationValue);

			if (rotationValue == rotationCurveEndValue) {
				rotationCurve = null;
			}
		}
	}

	/**
	 * Returns the current rotation state
	 */
	public float getCurrentRotation() {
		return currentRotation;
	}

	/**
	 * Returns the spot object
	 */
	private Renderer getSpotFromColor (string color) {
		if (color == "red") {
			return redSpot;
		} else if (color == "green") {
			return greenSpot;
		} else {
			return blueSpot;
		}
	}

	/**
	 * Returns the light object
	 */
	private Light getLightFromColor (string color) {
		if (color == "red") {
			return redLight;
		} else if (color == "green") {
			return greenLight;
		} else {
			return blueLight;
		}
	}

	/**
	 * Verifies if one of the lights is enabled
	 */
	private bool isLightEnabled (string color) {
		if (color == "red") {
			return redIsEnabled;
		} else if (color == "green") {
			return greenIsEnabled;
		} else {
			return blueIsEnabled;
		}
	}

	/**
	 * Verifies if one of the lights is activated
	 */
	public bool isLightActive (string color) {
		Light light = this.getLightFromColor (color);

		return (light.intensity > 0);
	}

	/**
	 * Returns the color of the spot when active
	 */
	private Color getActiveColor (string color) {
		if (color == "red") {
			return Color.red;
		} else if (color == "green") {
			return Color.green;
		} else {
			return Color.blue;
		}
	}

	/**
	 * Returns the color of the spot when inactive
	 */
	private Color getInactiveColor (string color) {
		if (color == "red") {
			return new Color(0.5f, 0, 0);
		} else if (color == "green") {
			return new Color(0, 0.5f, 0);
		} else {
			return new Color(0, 0, 0.5f);
		}
	}

	/**
	 * Returns the emission color of the spot when active
	 */
	private Color getActiveEmission (string color) {
		if (color == "red") {
			return new Color(0.25f, 0, 0);
		} else if (color == "green") {
			return new Color(0, 0.25f, 0);
		} else {
			return new Color(0, 0, 0.25f);
		}
	}

	/**
	 * Returns the color of a spot when disabled
	 */
	private Color getDisabledColor (string color) {
		return Color.gray;
	}

	public void disableLight (string color) {
		turnOffLight (color);

		Renderer spot = this.getSpotFromColor (color);
		spot.material.SetColor ("_Color", this.getDisabledColor (color));

		// Changes highlight color so the spot doesn't appear interactible
		ShineOnLookScript shineScript = spot.GetComponent<ShineOnLookScript> ();
		shineScript.shiningColor = this.getDisabledColor (color);
	}

	/**
	 * Turn on a light
	 */
	public bool turnOnLight (string color) {
		if (!togglingIsEnabled) {
			return false;
		}

		// Checks if the asked light is enabled
		if (
			(color == "red" && !redIsEnabled) ||
			(color == "green" && !greenIsEnabled) ||
			(color == "blue" && !blueIsEnabled)
			) {
			return false;
		}

		// If it's only one light at a time, we first turn all lights off
		if (onlyOneSpotAtOnce) {
			turnOffAll ();
		}

		// If lights have to automatically turn off
		if (timedLight) {
			timedLightStart = Time.time;
		}

		Renderer spot = this.getSpotFromColor (color);
		Light light = this.getLightFromColor (color);

		spot.material.SetColor ("_Color", this.getActiveColor (color));
		//spot.material.SetColor ("_EmissionColor", this.getActiveEmission (color));

		spot.GetComponent<AudioSource> ().Play ();

		light.intensity = lightIntensity;
		light.GetComponent<LensFlare> ().brightness = flareBrightness;

		return true;
	}

	/**
	 * Turn off a light
	 */
	public bool turnOffLight (string color) {
		if (!togglingIsEnabled) {
			return false;
		}

		// Checks if the asked light is enabled
		if (
			(color == "red" && !redIsEnabled) ||
			(color == "green" && !greenIsEnabled) ||
			(color == "blue" && !blueIsEnabled)
			) {
			return false;
		}

		Renderer spot = this.getSpotFromColor (color);
		Light light = this.getLightFromColor (color);

		spot.material.SetColor ("_Color", this.getInactiveColor (color));
		spot.material.SetColor ("_EmissionColor", Color.black);

		spot.GetComponent<AudioSource> ().Pause ();

		light.intensity = 0f;
		light.GetComponent<LensFlare> ().brightness = 0f;

		return true;
	}

	/**
	 * Turn on all the lights
	 */
	public void turnOnAll () {
		turnOnLight ("red");
		turnOnLight ("green");
		turnOnLight ("blue");
	}

	/**
	 * Turn off all the lights
	 */
	public void turnOffAll () {
		turnOffLight ("red");
		turnOffLight ("green");
		turnOffLight ("blue");
	}

	/**
	 * Toggle a light
	 */
	public void toggleLight(string color) {
		if (this.isLightActive (color)) {
			this.turnOffLight (color);
		} else {
			this.turnOnLight (color);
		}
	}


}
