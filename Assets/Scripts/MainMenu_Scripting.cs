using UnityEngine;
using System.Collections;

public class MainMenu_Scripting : MonoBehaviour {

    uint currentSubMenu = 0;

    Canvas thisCanvas;
    UnityEngine.UI.Image blackFader;

    UnityEngine.UI.Image gameLogo;
    UnityEngine.UI.Text clickText;

    CanvasGroup mainMenuGroup;
    CanvasGroup optionsMenuGroup;

    UnityEngine.UI.Slider graphSlider;
    UnityEngine.UI.Toggle subtitleToggle;

    bool isFadeOutGameExit = false;

    // Use this for initialization
    void Awake () {

        //Get the canvas
        thisCanvas = GetComponent<Canvas>();

        //Get the independant text elements
        gameLogo = transform.Find("FakeTitle").gameObject.GetComponent<UnityEngine.UI.Image>();
        clickText = transform.Find("Text_ClicktoBegin").gameObject.GetComponent<UnityEngine.UI.Text>();

        //Get the Canvas groups for the different sub-menus
        mainMenuGroup = transform.Find("Group_MainMenu").gameObject.GetComponent<CanvasGroup>();
        mainMenuGroup.interactable = false;
        optionsMenuGroup = transform.Find("Group_OptionsMenu").gameObject.GetComponent<CanvasGroup>();
        optionsMenuGroup.interactable = false;

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

        //Setup the black fader to fill the screen
        blackFader = transform.Find("BlackFader").gameObject.GetComponent<UnityEngine.UI.Image>();
        blackFader.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
        blackFader.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);

        //Start the coroutines to get this show on the road
        StartCoroutine("ShineClicktext");
        StartCoroutine("FadeScreenFromBlack");
    }

    //Coroutine which fades the menu in from black after the logos
    IEnumerator FadeScreenFromBlack()
    {
        for (float f = 1.0f; f >= 0.0f; f -= 0.1f)
        {
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
            clickText.canvasRenderer.SetAlpha(0.8f + Mathf.Sin((Time.frameCount-20f)/40f) * 0.2f);
            yield return null;
        }

        //After click, launches fadeout of text through other coroutine
        currentSubMenu = 1;

        clickText.canvasRenderer.SetAlpha(1.0f);
        StartCoroutine("FadeClickTextOut");

    }


    //Coroutine which fades the "click to begin" text out
    IEnumerator FadeClickTextOut()
    {
        Debug.Log("FadeClickTextOut");

        for (float f = 1.0f; f >= 0.0f; f -= 0.1f)
        {
            clickText.canvasRenderer.SetAlpha(f);
            yield return null;
        }

        //Once this text is faded out, remove text and display Main Menu
        clickText.transform.SetParent(null);
        DisplayMainMenu();

    }

    //---------------------------------------------------------------------------------------------------------------------------------------------
    //Main Menu and transitions--------------------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------------------------------------------------

    //Displays the main part of the menu (3buttons)
    private void DisplayMainMenu()
    {
        Debug.Log("DisplayMainMenu");

        //Launches fade-in effect
        StartCoroutine("FadeMainButtonsIn");
    }

    //Coroutine which fades the main menu in
    IEnumerator FadeMainButtonsIn()
    {
        for (float f = 0.0f; f <= 1.0f; f += 0.1f)
        {
            mainMenuGroup.alpha = f;
            yield return null;
        }

        //Once this text is faded in, make the buttons clickable
        mainMenuGroup.alpha = 1.0f;
        mainMenuGroup.interactable = true;

    }


    //Starts the process to display options menu
    public void DisplayOptionsMenu()
    {
        Debug.Log("DisplayOptionsMenu");

        currentSubMenu = 2;
        mainMenuGroup.interactable = false;

        //As usual, a coroutine takes care of offering a smoother transition
        StartCoroutine("FadeToOptionsMenu");
    }

    //Coroutine handling transition from main menu to options menu
    IEnumerator FadeToOptionsMenu()
    {
        //First, fade out the main menu
        for (float f = 1.0f; f >= 0.0f; f -= 0.1f)
        {
            mainMenuGroup.alpha = f;
            yield return null;
        }

        mainMenuGroup.alpha = 0.0f;

        //Second, fade in the options menu
        for (float f = 0.0f; f <= 1.0f; f += 0.1f)
        {
            optionsMenuGroup.alpha = f;
            yield return null;
        }

        optionsMenuGroup.alpha = 1.0f;

        //Once the options menu is faded in, make the buttons clickable
        optionsMenuGroup.interactable = true;

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


    //Fades the options out, then call the coroutine to fade in the menu
    IEnumerator FadeOutOptions()
    {
        for (float f = 1.0f; f >= 0.0f; f -= 0.1f)
        {
            optionsMenuGroup.alpha = f;
            yield return null;
        }

        optionsMenuGroup.alpha = 0.0f;
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

    //Coroutine which fades to back and then exits the scene
    IEnumerator FadeOutOfMenu()
    {
        blackFader.raycastTarget = true;

        for (float f = 0.0f; f >= 0.0f; f -= 0.1f)
        {
            blackFader.canvasRenderer.SetAlpha(f);
            yield return null;
        }
        blackFader.canvasRenderer.SetAlpha(0.0f);

        yield return new WaitForSeconds(0.5f);

        if (isFadeOutGameExit)
        {
            Application.Quit();
        }
        else
        {
            Application.LoadLevel("TestScene");
        }
    }

    
}
