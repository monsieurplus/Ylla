using UnityEngine;
using System.Collections;

public class IntroCredits : MonoBehaviour {

    [SerializeField]GameObject refInterface;

    [SerializeField]float PRESENTS_DURATION;
    [SerializeField]float TITLE_DURATION;
    [SerializeField]float MADEBY_DURATION;
    [SerializeField]float SOUND_DURATION;

    [SerializeField]float FADE_DURATION;

    UI_Scripting UIscript;

    CanvasGroup presentsGroup;
    CanvasGroup titleGroup;
    CanvasGroup madebyGroup;
    CanvasGroup soundGroup;

    CanvasGroup objectToFade;
    float fadeSign;
    bool isFadeOver = false;

	public bool enabled = true;

	public bool launchSceneAfter = true;
	public GameObject firstScene;

    // Use this for initialization
    void Start () {
        UIscript = refInterface.GetComponent<UI_Scripting>();
        presentsGroup = transform.Find("PRESENTS").gameObject.GetComponent<CanvasGroup>();
        titleGroup = transform.Find("TITLE").gameObject.GetComponent<CanvasGroup>();
        madebyGroup = transform.Find("MADEBY").gameObject.GetComponent<CanvasGroup>();
        soundGroup = transform.Find("SOUND").gameObject.GetComponent<CanvasGroup>();

        //--------------------------------------------------
        //ONLY FOR TEST PURPOSES
        //--------------------------------------------------
		if (enabled) {
			LaunchIntroSequence ();
		}

		if (enabled) {
			UIscript.requestFadeToBlack (0.01f);
		} else {
			UIscript.requestFadeFromBlack( 1.0f, true );
		}
        //--------------------------------------------------
        //ONLY FOR TEST PURPOSES
        //--------------------------------------------------
    }

	void Awake () {
		/*
		if (enabled) {
			UIscript.requestFadeToBlack (0.01f);
		} else {
			UIscript.requestFadeFromBlack( 1.0f, true );
		}
		*/
	}

    //Launches the intro sequence. Did not do it in start in case more complex sync with the music is required
    public void LaunchIntroSequence()
    {
        StartCoroutine("ManageIntroTimings");
    }

    //Manage the timings of the differnet screens of the intro.
    //Since the user can not skip anything, it is a big scripted coroutine. 
    //For clarity, fade in and fade outs are handled through other coroutines.
    IEnumerator ManageIntroTimings()
    {
        //Wait a little bit...
        yield return new WaitForSeconds(1.0f);

        //First the "Gobelins and CNAM present..." screen
        objectToFade = presentsGroup;
        fadeSign = 1.0f;
        StartCoroutine("ManageFadeEffects");

        yield return new WaitForSeconds(PRESENTS_DURATION);

        fadeSign = -1.0f;
        StartCoroutine("ManageFadeEffects");

        while ( !isFadeOver ) { yield return null; }

        //Second, the title card
        objectToFade = titleGroup;
        fadeSign = 1.0f;
        StartCoroutine("ManageFadeEffects");

        yield return new WaitForSeconds(TITLE_DURATION);

        fadeSign = -1.0f;
        StartCoroutine("ManageFadeEffects");

        while (!isFadeOver) { yield return null; }

        //Third, the credits of IDE team
        objectToFade = madebyGroup;
        fadeSign = 1.0f;
        StartCoroutine("ManageFadeEffects");

        yield return new WaitForSeconds(MADEBY_DURATION);

        fadeSign = -1.0f;
        StartCoroutine("ManageFadeEffects");

        while (!isFadeOver) { yield return null; }

        //Fourth, the credits for the people who helped us with audio
        objectToFade = soundGroup;
        fadeSign = 1.0f;
        StartCoroutine("ManageFadeEffects");

        yield return new WaitForSeconds(SOUND_DURATION);

        fadeSign = -1.0f;
        StartCoroutine("ManageFadeEffects");

        while (!isFadeOver) { yield return null; }

        //Wait a little bit...
        yield return new WaitForSeconds(1.0f);

        //We have seen everything, fade out of black and show us the game!
        UIscript.requestFadeFromBlack( 3, true );

        //Once the fade to black is launched, the gameobject of the intro can be disabled
        refInterface.transform.Find("INTROGROUP").gameObject.SetActive(false);

		// Launch the first scene
		if (launchSceneAfter) {
			DesertSceneControllerFirst firstSceneController = firstScene.GetComponent<DesertSceneControllerFirst> ();
			firstSceneController.sceneStarted = true;
		}
    }

    //Manages the fading in or out of an object.
    //Because parameters are stored by the class, only one fade can occur at a time.
    IEnumerator ManageFadeEffects()
    {
        float f;

        //Depending on fade sign, we either fade the object in (+) or out (-)
        if (fadeSign > 0.0f)
        {
            f = 0.0f;
            while (f < 1.0f)
            {
                f += Time.deltaTime / FADE_DURATION;
                if (f > 1.0f) { f = 1.0f; }

                objectToFade.alpha = f;
                yield return null;
            }
        }
        else
        {
            f = 1.0f;
            while (f > 0.0f)
            {
                f -= Time.deltaTime / FADE_DURATION;
                if (f < 0.0f) { f = 0.0f; }

                objectToFade.alpha = f;
                yield return null;
            }
        }

        //Once the fading is over, we set a flag to notify other coroutines it is.
        //The flag only remains "true" for one frame.
        isFadeOver = true; yield return null;

        isFadeOver = false;
    }
}
