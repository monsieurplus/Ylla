using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Subtitles_Scripting : MonoBehaviour {

    private UnityEngine.UI.Text textField;
    private CanvasRenderer canvRenderer;

    float waitTime;

	private Dictionary<string, string> captions = new Dictionary<string, string>();


	// Use this for initialization
	void Awake () {
	    textField = transform.Find("Text").gameObject.GetComponent<UnityEngine.UI.Text>();
        canvRenderer = GetComponent<CanvasRenderer>();
        canvRenderer.SetAlpha(0.0f);

		initCaptions ();
    }

	private void initCaptions() {
		captions["desertScene01_dialog01"] = "Enfin! Te voilà ! Je te cherche depuis une étern...";
		captions["desertScene01_dialog02"] = "Oh. Oh non...";
		captions["desertScene01_dialog03"] = "Non, non non non, ce n’est pas vous...";
		captions["desertScene01_dialog04"] = "Vous lui ressemblez tellement... Mais non...";
		captions["desertScene01_dialog05"] = "Dites quelque chose pour voir.";

		captions["desertScene01_dialog06"] = "Non non non, ce n’est pas ça du tout.";
		captions["desertScene01_dialog07"] = "Sa voix… sa voix était douce, il me faisait rire.";
		captions["desertScene01_dialog08"] = "Rien à voir avec la vôtre, c’est le moins que l’on puisse dire.";
		captions["desertScene01_dialog09"] = "Bon! Puisque vous n’êtes pas lui, vous ne m’êtes d’aucune utilité.";
		captions["desertScene01_dialog10"] = "Maintenant, laissez-moi tranquille. Et bonne journée.";
	}
	
	// Function to display the proper text in this subtitle box. Name of Audioclip is used as search key.
	public void DisplaySubtitleMatchingTheSound ( AudioClip soundType ) {

		Debug.Log (soundType.name);

        string subtitletext = "";
    
		if ( captions [soundType.name] != null) {
			subtitletext = captions [soundType.name];
		}
		else {
			subtitletext = "[MISSING CAPTION " + soundType.name + "]";
		}

        if ( subtitletext != "")
        {
            textField.text = subtitletext;
            waitTime = soundType.length;
            StartCoroutine("SubtitleLifeSpan");
        }
        else
        {
            Destroy(this.gameObject);
        }
	}

    //Coroutine to manage the whole life span of a subtitle object
    IEnumerator SubtitleLifeSpan()
    {
        //Before going further, we need to check if text is too long. It must be done here because size of text takes 1 frame to update
        //We check if the text is too big for the screen width.
        //If yes the sentence is cut in two parts. 
        //This is a very imperfect system, but should be enough for our game.
        yield return new WaitForEndOfFrame();
        Rect fieldSize = textField.rectTransform.rect;

        if (fieldSize.width > Screen.width * 0.8)
        {
            char[] cutSubtitleText = textField.text.ToCharArray();
            int cutPosition = Mathf.FloorToInt(cutSubtitleText.Length/2);
            while (cutSubtitleText[cutPosition] != ' ')
            {
                cutPosition--;
            }

            cutSubtitleText[cutPosition] = '\n';
            textField.text = new string(cutSubtitleText);
        }

        float f = 0.0f;
        float movementTotal = 0f;
        float initialPosition = transform.position.y;

        //Fade the subtitle in quickly and move it up(0.2 seconds)
        while ( (f < 1.0f) && (movementTotal < 40f) )
        {
            movementTotal += Time.deltaTime * 200;
            if (movementTotal > 40f) { movementTotal = 40f; }
            transform.position = new Vector3(transform.position.x, initialPosition + movementTotal, transform.position.z);

            f += Time.deltaTime * 5;
            if (f > 1.0f) { f = 1.0f; }

            canvRenderer.SetAlpha(f);
            yield return null;
        }

        //Wait for the duration of the sound, saved previously
        yield return new WaitForSeconds(waitTime);

        //Fade the subtitle out quickly (0.2 seconds)
        while (f > 0.0f)
        {
            f -= Time.deltaTime*5;
            if (f < 0.0f) { f = 0.0f; }

            canvRenderer.SetAlpha(f);
            yield return null;
        }

        Destroy(this.gameObject);
    }

    // Function to request this subtitle to move up. Used when several subtitles are displayed at once
    public void RequestMoveUp()
    {
        StartCoroutine("MoveMeUp");
    }

    //Coroutine to handle the disappearance of the subtitle object
    IEnumerator MoveMeUp()
    {
        float movementTotal = 0f;
        float initialPosition = transform.position.y;

        Rect fieldSize = textField.rectTransform.rect;
        float moveAmount = fieldSize.height + 20f;

        while ( movementTotal < moveAmount) {
            movementTotal += Time.deltaTime*moveAmount*5;
            if (movementTotal > moveAmount) { movementTotal = moveAmount; }
            transform.position = new Vector3(transform.position.x, initialPosition + movementTotal, transform.position.z);
            yield return null;
        }
    }
}
