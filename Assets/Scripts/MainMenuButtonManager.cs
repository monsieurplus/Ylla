using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenuButtonManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	public bool enabled = true;

	private Image theImage;
	private Text theLabel;

	// Use this for initialization
	void Start () {
		/*
		Debug.Log("Start");
		theImage = GetComponent<Image>();
		theLabel = GetComponentInChildren<Text>();

		Debug.Log(theImage);
		Debug.Log(theLabel);
		*/
	}

	void Awake () {
		if (theImage == null || theLabel == null) {
			theImage = GetComponent<Image>();
			theLabel = GetComponentInChildren<Text>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerDown (PointerEventData eventData) {
		if (enabled) {
			theLabel.color = Color.black;
		}
	}

	public void OnPointerUp (PointerEventData eventData) {
		if (enabled) {
			theLabel.color = Color.white;
		}
	}

	public void setOpacity (float opacity) {
		if (theImage != null || theLabel != null) {
			Color imageColor = theImage.color;
			imageColor.a = opacity;
			theImage.color = imageColor;

			Color labelColor = theLabel.color;
			labelColor.a = opacity;
			theLabel.color = labelColor;
		}
	}
}
