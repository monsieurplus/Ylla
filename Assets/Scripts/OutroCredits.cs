using UnityEngine;
using System.Collections;

public class OutroCredits : MonoBehaviour
{

    [SerializeField]GameObject refInterface;

    [SerializeField]float FADETOBLACK_DURATION;
    [SerializeField]float TOBECONTINUED_DURATION;

    [SerializeField]float FADE_DURATION;

    UI_Scripting UIscript;

    CanvasGroup continueGroup;
    CanvasGroup thanksGroup;

    CanvasGroup objectToFade;
    float fadeSign;
    bool isFadeOver = false;

    // Use this for initialization
    void Awake()
    {
        UIscript = refInterface.GetComponent<UI_Scripting>();

        continueGroup = transform.Find("TOBECONTINUED").gameObject.GetComponent<CanvasGroup>();
        thanksGroup = transform.Find("THANKSFORPLAYING").gameObject.GetComponent<CanvasGroup>();
    }

    //Launches the intro sequence. Did not do it in start in case more complex sync with the music is required
    public void LaunchOutroSequence()
    {
        StartCoroutine("ManageIntroTimings");
    }

    //Manage the timings of the differnet screens of the intro.
    //Since the user can not skip anything, it is a big scripted coroutine. 
    //For clarity, fade in and fade outs are handled through other coroutines.
    IEnumerator ManageIntroTimings()
    {

        //Launch the global fading to black of the scene over duration
        UIscript.requestFadeToBlack(FADETOBLACK_DURATION);

        //Wait for the same time
        yield return new WaitForSeconds(FADETOBLACK_DURATION);

        //Wait a little bit...
        yield return new WaitForSeconds(0.5f);

        //First the "To be continued..." screen
        objectToFade = continueGroup;
        fadeSign = 1.0f;
        StartCoroutine("ManageFadeEffects");

        yield return new WaitForSeconds(TOBECONTINUED_DURATION);

        fadeSign = -1.0f;
        StartCoroutine("ManageFadeEffects");

        while (!isFadeOver) { yield return null; }

        //Second, the "thanks for playing" screen
        objectToFade = thanksGroup;
        fadeSign = 1.0f;
        StartCoroutine("ManageFadeEffects");

        yield return new WaitForSeconds(FADE_DURATION);

        //Wait until user clicks to leave this screen
        while ( !Input.GetButtonDown("Fire1") ) { yield return null; }

        fadeSign = -1.0f;
        StartCoroutine("ManageFadeEffects");

        while (!isFadeOver) { yield return null; }

        //Wait a little bit...
        yield return new WaitForSeconds(3.0f);

        //Bring us back to the menu scene
        Application.LoadLevel("MainMenu");

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