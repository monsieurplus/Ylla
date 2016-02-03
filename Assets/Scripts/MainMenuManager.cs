using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	public Image darkPanel;
	public RawImage gobelinsLogo;
	public float gobelinsLogoMaxWidth;
	public float gobelinsLogoMaxHeight;

	public RawImage enjminLogo;
	public float enjminLogoMaxWidth;
	public float enjminLogoMaxHeight;

	public RawImage ilyaLogo;
	public Text ilyaSubtitle;
	public Text ilyaClickToBegin;
	public Text ilyaLoading;

	public RawImage menuLogo;
	public Image menuPanel;
	public Button menuStart;
	public Button menuOptions;
	public Button menuQuit;

	private AnimationCurve menuAnimation;

	public float gobelinsShowDuration;
	public float gobelinsStayDuration;
	public float gobelinsHideDuration;

	public float enjminShowDuration;
	public float enjminStayDuration;
	public float enjminHideDuration;

	public float ilyaShowDuration;
	public float ilyaHideDuration;

	public float menuShowDuration;
	public float menuHideDuration;

	public float loadingShowDuration;

	//private float screenWidth;
	//private float screenHeight;

	// starting
	// gobelins_show
	// gobelins_stay
	// gobelins_hide
	// enjmin_show
	// enjmin_stay
	// enjmin_hide
	// ilya_show
	// ilya_stay
	// ilya_hide
	// menu_show
	// menu_stay
	// menu_hide
	// submenu_show
	// submenu_stay
	// submenu_hide
	// loading_show
	// loading_stay
	private string currentPhase = "starting";
	private float currentPhaseStart;


	// Use this for initialization
	protected void Start () {
		// init logo color
		darkPanel.color = Color.black;

		gobelinsLogo.CrossFadeAlpha(0f, 0f, false);
		enjminLogo.CrossFadeAlpha(0f, 0f, false);
		ilyaLogo.CrossFadeAlpha(0f, 0f, false);
		ilyaSubtitle.CrossFadeAlpha(0f, 0f, false);
		ilyaClickToBegin.CrossFadeAlpha(0f, 0f, false);
		ilyaLoading.CrossFadeAlpha(0f, 0f, false);
		menuLogo.CrossFadeAlpha(0f, 0f, false);
		menuPanel.CrossFadeAlpha(0f, 0f, false);

		menuStart.GetComponent<MainMenuButtonManager>().setOpacity(0f);
		menuStart.GetComponent<MainMenuButtonManager>().enabled = false;
		menuStart.interactable = false;

		menuOptions.GetComponent<MainMenuButtonManager>().setOpacity(0f);
		menuOptions.GetComponent<MainMenuButtonManager>().enabled = false;
		menuOptions.interactable = false;

		menuQuit.GetComponent<MainMenuButtonManager>().setOpacity(0f);
		menuQuit.GetComponent<MainMenuButtonManager>().enabled = false;
		menuQuit.interactable = false;

		currentPhaseStart = Time.time;
	}
	
	// Update is called once per frame
	protected void Update () {
		switch (currentPhase)
		{
		case "starting":
			phaseStarting();
			break;

		case "gobelins_show":
			phaseGobelinsShow();
			break;

		case "gobelins_stay":
			phaseGobelinsStay();
			break;

		case "gobelins_hide":
			phaseGobelinsHide();
			break;

		case "enjmin_show":
			phaseEnjminShow();
			break;

		case "enjmin_stay":
			phaseEnjminStay();
			break;

		case "enjmin_hide":
			phaseEnjminHide();
			break;

		case "ilya_show":
			phaseIlyaShow();
			break;

		case "ilya_stay":
			phaseIlyaStay();
			break;

		case "ilya_hide":
			phaseIlyaHide();
			break;

		case "menu_show":
			phaseMenuShow();
			break;

		case "menu_stay":
			phaseMenuStay();
			break;

		case "menu_hide":
			phaseMenuHide();
			break;

		case "loading_show":
			phaseLoadingShow();
			break;
		}
	}

	private void phaseStarting () {
		if (Time.time - currentPhaseStart > 1) {
			currentPhase = "gobelins_show";
			currentPhaseStart = -1f;
		}
	}

	private void phaseGobelinsShow () {
		if (currentPhaseStart < 0) {
			gobelinsLogo.CrossFadeAlpha(1.0f, gobelinsShowDuration, false);
			currentPhaseStart = Time.time;
		}
		else {
			if (Time.time - currentPhaseStart > gobelinsShowDuration) {
				currentPhase = "gobelins_stay";
				currentPhaseStart = -1f;
			}

			if (Input.GetMouseButtonDown(0)) {
				currentPhase = "gobelins_hide";
				currentPhaseStart = -1f;
			}
		}

	}

	private void phaseGobelinsStay () {
		if (currentPhaseStart < 0) {
			currentPhaseStart = Time.time;
		}
		else {
			if (Input.GetMouseButtonDown(0) || Time.time - currentPhaseStart > gobelinsStayDuration) {
				currentPhase = "gobelins_hide";
				currentPhaseStart = -1f;
			}
		}
	}

	private void phaseGobelinsHide () {
		if (currentPhaseStart < 0) {
			gobelinsLogo.CrossFadeAlpha(0.0f, gobelinsHideDuration, false);
			currentPhaseStart = Time.time;
		}
		else {
			if (Time.time - currentPhaseStart > gobelinsHideDuration) {
				currentPhase = "enjmin_show";
				currentPhaseStart = -1f;
			}
		}
	}

	private void phaseEnjminShow () {
		if (currentPhaseStart < 0) {
			enjminLogo.CrossFadeAlpha(1.0f, enjminShowDuration, false);
			currentPhaseStart = Time.time;
		}
		else {
			if (Time.time - currentPhaseStart > enjminShowDuration) {
				currentPhase = "enjmin_stay";
				currentPhaseStart = -1f;
			}

			if (Input.GetMouseButtonDown(0)) {
				currentPhase = "enjmin_hide";
				currentPhaseStart = -1f;
			}
		}
	}

	private void phaseEnjminStay () {
		if (currentPhaseStart < 0) {
			currentPhaseStart = Time.time;
		}
		else {
			if (Input.GetMouseButtonDown(0) || Time.time - currentPhaseStart > enjminStayDuration) {
				currentPhase = "enjmin_hide";
				currentPhaseStart = -1f;
			}
		}
	}

	private void phaseEnjminHide () {
		if (currentPhaseStart < 0) {
			enjminLogo.CrossFadeAlpha(0.0f, enjminHideDuration, false);
			currentPhaseStart = Time.time;
		}
		else {
			if (Time.time - currentPhaseStart > enjminHideDuration) {
				currentPhase = "ilya_show";
				currentPhaseStart = -1f;
			}
		}
	}

	private void phaseIlyaShow () {
		if (currentPhaseStart < 0) {
			ilyaLogo.CrossFadeAlpha(1.0f, ilyaShowDuration, false);
			ilyaSubtitle.CrossFadeAlpha(1.0f, ilyaShowDuration, false);
			ilyaClickToBegin.CrossFadeAlpha(1.0f, ilyaShowDuration, false);
			darkPanel.CrossFadeAlpha(0.75f, ilyaShowDuration, false);

			currentPhaseStart = Time.time;
		}
		else{
			if (Time.time - currentPhaseStart > ilyaShowDuration) {
				currentPhase = "ilya_stay";
				currentPhaseStart = -1f;
			}

			if (Input.GetMouseButtonDown(0)) {
				currentPhase = "ilya_hide";
				currentPhaseStart = -1f;
			}
		}
	}

	private void phaseIlyaStay () {
		Color textColor = Color.white;
		textColor.a = 0.75f + Mathf.Sin(Time.time * 2f) * 0.25f;

		ilyaClickToBegin.color = textColor;

		if (Input.GetMouseButtonDown(0)) {
			currentPhase = "ilya_hide";
			currentPhaseStart = -1f;
		}
	}

	private void phaseIlyaHide () {
		if (currentPhaseStart == -1f) {
			// Transition to small logo
			ilyaLogo.CrossFadeAlpha(0f, ilyaHideDuration, false);
			ilyaSubtitle.CrossFadeAlpha(0f, ilyaHideDuration, false);
			ilyaClickToBegin.CrossFadeAlpha(0f, ilyaHideDuration, false);

			currentPhaseStart = Time.time;
		}
		else {
			if (Time.time - currentPhaseStart > ilyaHideDuration) {
				currentPhase = "menu_show";
				currentPhaseStart = -1f;
			}
		}
	}

	private void phaseMenuShow() {
		if (currentPhaseStart == -1f) {
			menuLogo.CrossFadeAlpha(1f, menuShowDuration, false);
			menuPanel.CrossFadeAlpha(1f, menuShowDuration, false);

			menuAnimation = AnimationCurve.EaseInOut(Time.time, 0f, Time.time + menuShowDuration, 1f);
			currentPhaseStart = Time.time;
		}
		else {
			float currentOpacity = menuAnimation.Evaluate(Time.time);
			menuStart.GetComponent<MainMenuButtonManager>().setOpacity(currentOpacity);
			menuOptions.GetComponent<MainMenuButtonManager>().setOpacity(currentOpacity/2);
			menuQuit.GetComponent<MainMenuButtonManager>().setOpacity(currentOpacity);

			if (Time.time - currentPhaseStart > menuShowDuration) {
				currentPhase = "menu_stay";
				currentPhaseStart = -1f;
			}
		}
	}

	private void phaseMenuStay () {
		menuStart.interactable = true;
		menuStart.GetComponent<MainMenuButtonManager>().enabled = true;

		menuQuit.interactable = true;
		menuQuit.GetComponent<MainMenuButtonManager>().enabled = true;
	}

	private void phaseMenuHide() {
		if (currentPhaseStart == -1f) {
			menuLogo.CrossFadeAlpha(0f, menuShowDuration, false);
			menuPanel.CrossFadeAlpha(0f, menuShowDuration, false);

			menuAnimation = AnimationCurve.EaseInOut(Time.time, 1f, Time.time + menuShowDuration, 0f);
			currentPhaseStart = Time.time;
		}
		else {
			float currentOpacity = menuAnimation.Evaluate(Time.time);
			menuStart.GetComponent<MainMenuButtonManager>().setOpacity(currentOpacity);
			menuOptions.GetComponent<MainMenuButtonManager>().setOpacity(currentOpacity/2);
			menuQuit.GetComponent<MainMenuButtonManager>().setOpacity(currentOpacity);

			if (Time.time - currentPhaseStart > menuHideDuration) {
				currentPhase = "loading_show";
				currentPhaseStart = -1f;
			}
		}
	}

	private void phaseLoadingShow() {
		if (currentPhaseStart == -1f) {
			ilyaLogo.CrossFadeAlpha(1f, loadingShowDuration, false);
			ilyaSubtitle.CrossFadeAlpha(1f, loadingShowDuration, false);
			ilyaLoading.CrossFadeAlpha(1f, loadingShowDuration, false);

			currentPhaseStart = Time.time;
		}
		else {
			if (Time.time - currentPhaseStart > loadingShowDuration) {
				Application.LoadLevel("main_scene");
			}
		}
	}

	public void LaunchGame () {
		currentPhase = "menu_hide";
		currentPhaseStart = -1f;
	}

	public void QuitGame () {
		Application.Quit();
	}
}
