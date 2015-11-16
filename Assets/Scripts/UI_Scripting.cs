using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class UI_Scripting : MonoBehaviour {

    [SerializeField]private GameObject refPlayer;
    [SerializeField]private GameObject SubtitleContainer;

    Canvas thisCanvas;
    UnityEngine.UI.Image blackFader;
    UnityEngine.UI.Image whiteFader;

    GameObject paperPieceObj;
    UnityEngine.UI.Image paperPiece;
    Vector2 desiredImageSize;

    List<GameObject> SubtitleEntities = new List<GameObject>();

    float fadeTime;

    // Use this for initialization
    void Start () {
        thisCanvas = GetComponent<Canvas>();

        //Initialize the black fader
        blackFader = transform.Find("BlackFader").gameObject.GetComponent<UnityEngine.UI.Image>();
        blackFader.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
        blackFader.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
        blackFader.canvasRenderer.SetAlpha(0f);

        //Initialize the white fader
        whiteFader = transform.Find("WhiteFader").gameObject.GetComponent<UnityEngine.UI.Image>();
        whiteFader.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
        whiteFader.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
        whiteFader.canvasRenderer.SetAlpha(0f);

    }

    // Loads a sprite from Asset list
    public void loadPaperDocument (string docName)
    {
        //For the script to work, we need "docName" to be "InteractiblePaper_(sprite asset name)"
        //The asset needs to be into the Assets/Resources folder, or it will not work

        //Cut the beginning of the string to isolate (sprite asset name). Uses "_" as the separator
        string[] AssetName = docName.Split(new char[] { '_' });

        //Create an "Image" (UI element) from the void. Fill it with the correct sprite using the string created previously
        paperPieceObj = new GameObject();
        paperPiece = paperPieceObj.AddComponent<UnityEngine.UI.Image>();

        Debug.Log(AssetName[1]);

        paperPiece.sprite = Resources.Load(AssetName[1], typeof(Sprite)) as Sprite;

        //Set sprite as child of canvas interface
        paperPiece.transform.parent = thisCanvas.transform;

        //Read original sprite texture size
        Vector2 dimensionsImage = new Vector2(paperPiece.sprite.texture.width, paperPiece.sprite.texture.height);

        //Read format of the canvas
        RectTransform canvasRect = thisCanvas.GetComponent<RectTransform>();
        Vector2 dimensionsCanvas = new Vector2(canvasRect.rect.width, canvasRect.rect.height);

        //Center the sprite in the middle of the screen
        paperPiece.transform.position = new Vector3( dimensionsCanvas.x / 2, dimensionsCanvas.y / 2 );

        //Calculate the size we want to display it (biggest dimension = 80% of canvas)
        float resizeRatio;
        desiredImageSize = dimensionsImage;

        if (desiredImageSize.x >= desiredImageSize.y )
        {
            if (desiredImageSize.x > 0.95f*dimensionsCanvas.x)
            {
                resizeRatio = (0.95f * dimensionsCanvas.x) / desiredImageSize.x;
                desiredImageSize *= resizeRatio;
            }
        }
        else
        {
            if (desiredImageSize.y > 0.95f * dimensionsCanvas.y)
            {
                resizeRatio = (0.95f * dimensionsCanvas.y) / dimensionsImage.y;
                desiredImageSize *= resizeRatio;
            }
        }

        Debug.Log("Canvas Size :" + dimensionsCanvas + "- Desired Size :" + desiredImageSize );

        //Disable normal FPS Controls
        refPlayer.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
        refPlayer.transform.GetChild(0).GetComponent<DetectInteractibles>().enabled = false;

        //Launch the coroutine which will make the paper appear
        StartCoroutine("FadePaperSpriteIn");

    }


    //Coroutine which causes the "pieces of paper" to fade in and grow over a few frames, and waits for click to make it dissapear
    IEnumerator FadePaperSpriteIn()
    {
        RectTransform spriteRect = paperPiece.GetComponent<RectTransform>();
        for (float f = 0.0f; f <= 1.0f; f += 0.1f)
        {
            blackFader.canvasRenderer.SetAlpha(0.7f*f);
            spriteRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, f*desiredImageSize.x);
            spriteRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, f*desiredImageSize.y);
            yield return null;
        }

        //Use yield to wait until button is pressed. When it is start new coroutine to make paper disappear
        while ( !Input.GetButtonDown("Fire1") ){ yield return null; }
        StartCoroutine("FadePaperSpriteOut");

    }

    //Coroutine which causes the "pieces of paper" to fade in and grow over a few frames
    IEnumerator FadePaperSpriteOut()
    {
        RectTransform spriteRect = paperPiece.GetComponent<RectTransform>();
        for (float f = 1.0f; f >= 0.0f; f -= 0.1f)
        {
            blackFader.canvasRenderer.SetAlpha(0.7f * f);
            spriteRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, f * desiredImageSize.x);
            spriteRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, f * desiredImageSize.y);
            yield return null;
        }



        //destroy the "paper" sprite
        Destroy(paperPiece);
        Destroy(paperPieceObj);

        //Give control back to player once fade is over
        refPlayer.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
        refPlayer.transform.GetChild(0).GetComponent<DetectInteractibles>().enabled = true;
    }

    //Requests a fade to black at a given speed (in seconds)
    public void requestFadeToBlack( float seconds )
    {
        fadeTime = seconds;
        StartCoroutine("FadeToBlack");
    }

    //Coroutine which causes a complete fade to black (used when "dying" from a fall)
    IEnumerator FadeToBlack()
    {
        float f = 0.0f;

        while (f < 1.0f)
        {
            f += Time.deltaTime/fadeTime;
            if (f>1.0f) { f = 1.0f; }

            blackFader.canvasRenderer.SetAlpha(f);
            yield return null;
        }

    }

    //Requests a white flash from a faded black screen (used when "respawning" after a fall
    public void requestWhiteFlashFromBlack()
    {
        StartCoroutine("WhiteFlashFromBlack");
    }

    //Coroutine which executes the white flash from black (when "respawning"). Fixed speed
    IEnumerator WhiteFlashFromBlack()
    {
        float f = 0.0f;

        //Make white fader appear first
        while (f < 1.0f)
        {
            f += Time.deltaTime*5;
            if (f > 1.0f) { f = 1.0f; }

            whiteFader.canvasRenderer.SetAlpha(f);
            yield return null;
        }

        //Instantly hide black fader
        blackFader.canvasRenderer.SetAlpha(0.0f);

        //Make white fader disappear slower
        while (f > 0.0f)
        {
            f -= Time.deltaTime;
            if (f < 0.0f) { f = 0.0f; }

            whiteFader.canvasRenderer.SetAlpha(f);
            yield return null;
        }
    }

    //Function to display a subtitle. It's just a data transfer.
    public void PlayLineWithSubtitles( AudioSource source, AudioClip audio )
    {
        source.PlayOneShot(audio);

        //Do the subtitles actually appear? Depends if they are enabled. Check the option
        if ( PlayerPrefs.GetInt("subtitles") == 1 ) { 
            GameObject NewSubtitleBox = Instantiate(SubtitleContainer) as GameObject;
            NewSubtitleBox.transform.SetParent(thisCanvas.transform, false );
            NewSubtitleBox.transform.position = new Vector3 (Screen.width/2 , Screen.height/6, 0);

            Subtitles_Scripting scriptSub = NewSubtitleBox.GetComponent<Subtitles_Scripting>();

            scriptSub.DisplaySubtitleMatchingTheSound(audio);

            //Review existing array of subtitles, if there are any, order them to move up
            for ( int i=0; i < SubtitleEntities.Count; i++ )
            {
                if ( SubtitleEntities[i] != null )
                {
                    //If there is previous subtitle, order it to move up
                    scriptSub = SubtitleEntities[i].GetComponent<Subtitles_Scripting>();
                    scriptSub.RequestMoveUp();
                }
                else
                {
                    //If array row is null (previous subtitle removed), remove it
                    SubtitleEntities.RemoveAt(i);
                    i--;
                }
            }

            //Push the new subtitle to end of array
            SubtitleEntities.Add(NewSubtitleBox);
        }
    }


    //--------------------------------------------------
    //ONLY FOR TEST PURPOSES
    //--------------------------------------------------
    AudioSource loopSource;
    AudioClip[] loopAudios;

    public void startQuickLinesLoop(AudioSource source, AudioClip[] audios )
    {
        loopSource = source;
        loopAudios = audios;
        StartCoroutine("PlayRandomLinesQuick");
    }

    //Coroutine to play subtitled lines in quick succession. Very annoying to human ears but tests subtible system.
    IEnumerator PlayRandomLinesQuick()
    {
        while (true) { 
            int n = Random.Range(0, loopAudios.Length);
            loopSource.clip = loopAudios[n];
            PlayLineWithSubtitles(loopSource, loopSource.clip);
            yield return new WaitForSeconds(1.5f);
        }
    }
    //--------------------------------------------------
    //ONLY FOR TEST PURPOSES
    //--------------------------------------------------

}
