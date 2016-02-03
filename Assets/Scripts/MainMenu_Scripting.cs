using UnityEngine;
using System.Collections;

public class MainMenu_Scripting : MonoBehaviour {

    [SerializeField]private AudioClip[] ButtonClicSounds;

    uint currentSubMenu = 0;

    Canvas thisCanvas;
    UnityEngine.UI.Image blackFader;

    UnityEngine.UI.Text clickText;

    CanvasGroup welcomeGroup;
    CanvasGroup commonsMenuGroup;
    CanvasGroup mainMenuGroup;
    CanvasGroup optionsMenuGroup;

    UnityEngine.UI.Slider graphSlider;
    UnityEngine.UI.Toggle subtitleToggle;

    AudioSource audioSrc;

    bool isFadeOutGameExit = false;

    // Use this for initialization
    void Start () {

        //We can come from the game into this menu. In the game the cursor is hidden. Force it visible to be sure.
        Cursor.visible = true;

        //Get the canvas
        thisCanvas = GetComponent<Canvas>();

        //Get the Canvas groups for the different sub-menus
        welcomeGroup = transform.Find("Group_Welcome").gameObject.GetComponent<CanvasGroup>();
        welcomeGroup.interactable = false;
        commonsMenuGroup = transform.Find("Group_Commons").gameObject.GetComponent<CanvasGroup>();
        commonsMenuGroup.interactable = false;
        mainMenuGroup = transform.Find("Group_MainMenu").gameObject.GetComponent<CanvasGroup>();
        mainMenuGroup.interactable = false;
        optionsMenuGroup = transform.Find("Group_OptionsMenu").gameObject.GetComponent<CanvasGroup>();
        optionsMenuGroup.interactable = false;

        //Get the dynamic elements from welcome screen
        clickText = welcomeGroup.transform.Find("Text_ClicktoBegin").gameObject.GetComponent<UnityEngine.UI.Text>();

        //Get the interactive elements from options screen
        graphSlider = optionsMenuGroup.transform.Find("Graph_Quality_Slider").gameObject.GetComponent<UnityEngine.UI.Slider>();
        graphSlider.value = QualitySettings.GetQualityLevel();

        subtitleToggle = optionsMenuGroup.transform.Find("Subtitle_Toggle").gameObject.GetComponent<UnityEngine.UI.Toggle>();

        //Reads subtitles preferences in "preferences" save file. 0 if no value. Checkbox is changed accordingly.
        if ( PlayerPrefs.GetInt("subtitles", 0) == 1 ) { 
            subtitleToggle.isOn = true;
        }
        else
        {
            subtitleToggle.isOn = false;
        }

        PlayerPrefs.SetInt("subtitles", 0);

        //Modify size of background image to make sure it fills screen
        RectTransform backgroundSize = gameObject.transform.Find("BackgroundImage").gameObject.GetComponent<RectTransform>();
        backgroundSize.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
        backgroundSize.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);

        //Setup the black fader to fill the screen
        blackFader = transform.Find("BlackFader").gameObject.GetComponent<UnityEngine.UI.Image>();
        blackFader.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
        blackFader.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);

        //Fetch and store menu audio source
        audioSrc = GetComponent<AudioSource>();

        //Start first step : showing the Unity logo. It starts the chain of events
        StartCoroutine("ShowGobelinsLogo");
    }

    //Coroutine which displays the Gobelins Logo. Clicking skips to next logo
    IEnumerator ShowGobelinsLogo()
    {
        UnityEngine.UI.Image logo = transform.Find("Logo Gobelins").gameObject.GetComponent<UnityEngine.UI.Image>();
        float f = 0.0f;

        //Short wait for comfort
        yield return new WaitForSeconds(1.0f);
        logo.gameObject.SetActive(true);

        //Fade the logo in (0.5 seconds)
        f = 0.0f;
        while (f < 1.0f)
        {
            f += Time.deltaTime * 2;
            if (f > 1.0f) { f = 1.0f; }

            logo.canvasRenderer.SetAlpha(f);
            yield return null;
        }

        //Keep it on screen for 3 seconds. Clicking here skips.
        f = 0.0f;
        while ( f < 6.0f && !Input.GetButton("Fire1"))
        {
            f += Time.deltaTime;
            yield return null;
        }

        //Fade the logo out (0.5 seconds)
        f = 1.0f;
        while (f > 0.0f)
        {
            f -= Time.deltaTime * 2;
            if (f < 0.0f) { f = 0.0f; }

            logo.canvasRenderer.SetAlpha(f);
            yield return null;
        }

        logo.gameObject.SetActive(false);

        //Start next step : showing the Gobelins logo
        StartCoroutine("ShowCNAMLogo");
    }

    //Coroutine which displays the Unity logo. Clicking skips to main menu
    IEnumerator ShowCNAMLogo()
    {
        UnityEngine.UI.Image logo = transform.Find("Logo CNAM").gameObject.GetComponent<UnityEngine.UI.Image>();
        float f = 1.0f;

        //Short wait for comfort
        yield return new WaitForSeconds(0.5f);
        logo.gameObject.SetActive(true);

        //Fade the logo in (0.5 seconds)
        f = 0.0f;
        while (f < 1.0f)
        {
            f += Time.deltaTime * 2;
            if (f > 1.0f) { f = 1.0f; }

            logo.canvasRenderer.SetAlpha(f);
            yield return null;
        }

        //Keep it on screen for 3 seconds. Clicking here skips.
        f = 0.0f;
        while (f < 6.0f && !Input.GetButton("Fire1"))
        {
            f += Time.deltaTime;
            yield return null;
        }

        //Fade the logo out (0.5 seconds)
        f = 1.0f;
        while (f > 0.0f)
        {
            f -= Time.deltaTime * 2;
            if (f < 0.0f) { f = 0.0f; }

            logo.canvasRenderer.SetAlpha(f);
            yield return null;
        }

        logo.gameObject.SetActive(false);

        //Start next step : fading from black and start base menu animations
        StartCoroutine("ShineClicktext");
        StartCoroutine("FadeScreenFromBlack");
    }


    //Coroutine which fades the menu in from black after the logos
    IEnumerator FadeScreenFromBlack()
    {
        float f = 1.0f;

        //Fade the subtitle out slowly (2 seconds)
        while (f > 0.0f)
        {
            f -= Time.deltaTime / 2;
            if (f < 0.0f) { f = 0.0f; }

            blackFader.canvasRenderer.SetAlpha(f);
            yield return null;
        }

        blackFader.canvasRenderer.SetAlpha(0.0f);
        blackFader.raycastTarget = false;
    }

    //Coroutine which causes the "click to begin" text to have variable luminosity
    IEnumerator ShineClicktext()
    {
        //Until user clicks (left or right button) coroutine is stuck in loop that changes alpha
        while (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
        {
            clickText.canvasRenderer.SetAlpha(0.6f + Mathf.Sin((Time.frameCount-20f)/40f) * 0.4f);
            yield return null;
        }

        //After click, launches fadeout of text through other coroutine
        currentSubMenu = 1;

        clickText.canvasRenderer.SetAlpha(1.0f);
        StartCoroutine("FadeWelcomeOut");

    }


    //Coroutine which fades the welcome screen out
    IEnumerator FadeWelcomeOut()
    {
        float f = 1.0f;

        //Fade the welcome menu out (over 0.5 second)
        while (f > 0.0f)
        {
            f -= Time.deltaTime*2;
            if (f < 0.0f) { f = 0.0f; }

            welcomeGroup.alpha = f;
            yield return null;
        }

        //disable the gameobject entirely
        welcomeGroup.gameObject.SetActive(false);

        //Once this text is faded out, remove text and display Main Menu
        DisplayMainMenu();

        //Because this is the first time we access the main menu. We also need to have the "commons" appear.
        //They are the commons between main menu and options menu : the only two menus the user will see from here.
        StartCoroutine("FadeCommonsIn");

    }

    //Coroutine which fades the "commons" in
    IEnumerator FadeCommonsIn()
    {
        commonsMenuGroup.gameObject.SetActive(true);

        float f = 0.0f;

        //Fade the commons in (over 0.5 second)
        while (f < 1.0f)
        {
            f += Time.deltaTime;
            if (f > 1.0f) { f = 1.0f; }

            commonsMenuGroup.alpha = f;
            yield return null;
        }

        commonsMenuGroup.alpha = 1.0f;

    }

    //---------------------------------------------------------------------------------------------------------------------------------------------
    //Main Menu and transitions--------------------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------------------------------------------------

    //Displays the main part of the menu (3buttons)
    private void DisplayMainMenu()
    {
        //Launches fade-in effect
        StartCoroutine("FadeMainButtonsIn");
    }

    //Coroutine which fades the main menu in
    IEnumerator FadeMainButtonsIn()
    {
        mainMenuGroup.gameObject.SetActive(true);

        float f = 0.0f;

        //Fade the welcome menu out (over 1 second)
        while (f < 1.0f)
        {
            f += Time.deltaTime;
            if (f > 1.0f) { f = 1.0f; }

            //Once this text is faded in enough, make the buttons clickable
            if ( f > 0.2f) { mainMenuGroup.interactable = true; }

            mainMenuGroup.alpha = f;
            yield return null;
        }

    }


    //Starts the process to display options menu
    public void DisplayOptionsMenu()
    {
        currentSubMenu = 2;
        mainMenuGroup.interactable = false;

        //As usual, a coroutine takes care of offering a smoother transition
        StartCoroutine("FadeToOptionsMenu");
    }

    //Coroutine handling transition from main menu to options menu
    IEnumerator FadeToOptionsMenu()
    {
        float f = 1.0f;

        //Fade the main menu out (over 0.5 second)
        while (f > 0.0f)
        {
            f -= Time.deltaTime * 2;
            if (f < 0.0f) { f = 0.0f; }

            mainMenuGroup.alpha = f;
            yield return null;
        }

        mainMenuGroup.alpha = 0.0f;
        mainMenuGroup.gameObject.SetActive(false);
        optionsMenuGroup.gameObject.SetActive(true);

        //Second, fade in the options menu
        while (f < 1.0f)
        {
            f += Time.deltaTime;
            if (f > 1.0f) { f = 1.0f; }

            //Once this text is faded in enough, make the buttons clickable
            if (f > 0.2f) { optionsMenuGroup.interactable = true; }

            optionsMenuGroup.alpha = f;
            yield return null;
        }

        optionsMenuGroup.alpha = 1.0f;    

    }

    //---------------------------------------------------------------------------------------------------------------------------------------------
    //Managing the Options Menu--------------------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------------------------------------------------

    //Changes the graphic quality depending on the slider position
    public void UpdateGraphicsQuality ()
    {
        QualitySettings.SetQualityLevel( Mathf.RoundToInt(graphSlider.value) , true);
    }

    //Changes the status saved for subtitles on toggle change
    public void SaveSubtitleToggle () {

        int valueToSave;
        if (subtitleToggle.isOn) valueToSave = 1;  else valueToSave = 0;
        PlayerPrefs.SetInt("subtitles", valueToSave);

        Debug.Log("Value Saved : "+ valueToSave);
    }


    //Starts the process to display options menu
    public void ReturnToMainMenu()
    {
        Debug.Log("ReturnToMainMenu");

        currentSubMenu = 1;
        optionsMenuGroup.interactable = false;

        //As usual, a coroutine takes care of offering a smoother transition
        StartCoroutine("FadeOutOptions");
    }


    //Fades the options out (0.5 sec), then call the coroutine to fade in the menu
    IEnumerator FadeOutOptions()
    {
        float f = 1.0f;

        while (f > 0.0f)
        {
            f -= Time.deltaTime * 2;
            if (f < 0.0f) { f = 0.0f; }

            optionsMenuGroup.alpha = f;
            yield return null;
        }

        optionsMenuGroup.gameObject.SetActive(false);

        DisplayMainMenu();

    }

    //---------------------------------------------------------------------------------------------------------------------------------------------
    //Exiting this menu (to start or quit game) - includes fade to black---------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------------------------------------------------

    //Launches process of starting the main game (button is setup to call this function through UNITY interface)
    public void StartGame()
    {
        mainMenuGroup.interactable = false;
        StartCoroutine("FadeOutOfMenu");
    }

    //Launches process of quitting the game (button is setup to call this function through UNITY interface)
    public void QuitGame()
    {
        isFadeOutGameExit = true;

        mainMenuGroup.interactable = false;
        StartCoroutine("FadeOutOfMenu");
    }

    //Coroutine which fades to back (2 seconds) and then exits the scene
    IEnumerator FadeOutOfMenu()
    {
        blackFader.raycastTarget = true;

        float f = 0.0f;

        //Second, fade in the options menu
        while (f < 1.0f)
        {
            f += Time.deltaTime;
            if (f > 1.0f) { f = 1.0f; }

            blackFader.canvasRenderer.SetAlpha(f);
            yield return null;
        }

        blackFader.canvasRenderer.SetAlpha(1.0f);


        //Wait a short time with black screen before exiting menu
        yield return new WaitForSeconds(0.5f);

        if (isFadeOutGameExit)
        {
            Application.Quit();
        }
        else
        {
            Application.LoadLevel("main_scene");
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------
    //Managing the sound in the menu---------------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------------------------------------------------
    public void PlayButtonSound()
    {

        int n = Random.Range(1, ButtonClicSounds.Length);
        audioSrc.clip = ButtonClicSounds[n];
        //Request the play of the line with subtitles through the function available in interface script
        audioSrc.PlayOneShot(audioSrc.clip);
        //move picked sound to index 0 so it's not picked next time
        ButtonClicSounds[n] = ButtonClicSounds[0];
        ButtonClicSounds[0] = audioSrc.clip;
    }
}
