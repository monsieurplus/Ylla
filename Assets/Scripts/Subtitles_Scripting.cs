using UnityEngine;
using System.Collections;

public class Subtitles_Scripting : MonoBehaviour {

    private UnityEngine.UI.Text textField;
    private CanvasRenderer canvRenderer;

    float waitTime;

	// Use this for initialization
	void Awake () {
	    textField = transform.Find("Text").gameObject.GetComponent<UnityEngine.UI.Text>();
        canvRenderer = GetComponent<CanvasRenderer>();
        canvRenderer.SetAlpha(0.0f);
    }
	
	// Function to display the proper text in this subtitle box. Name of Audioclip is used as search key.
	public void DisplaySubtitleMatchingTheSound ( AudioClip soundType ) {

        string subtitletext = "";
    
        switch ( soundType.name )
        {
            case "RespawnLine_Test1":
                subtitletext = "Roh... Tu pourrais faire attention quand même!";
            break;
            case "RespawnLine_Test2":
                subtitletext = "C...c'est bon! J'ai rien vu! Continue comme avant!";
            break;
            case "RespawnLine_Test3":
                subtitletext = "Et voilà, même pas mort!";
            break;
            case "RespawnLine_Test4":
                subtitletext = "Et hop! Il s'est rien passé!";
            break;
            case "RespawnLine_Test5":
                subtitletext = "Alala... T'es un peu neuneu, quand même.";
            break;
        }

        if ( subtitletext != "")
        {
            //--------------------------------------------------
            //ONLY FOR TEST PURPOSES
            //--------------------------------------------------
            //Randomly make text longer...or not
            int n = Random.Range(0, 3);
            switch (n) {
                case 1:
                    subtitletext += " Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
                    break;
                case 2:
                    subtitletext += " Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam et mauris at purus porta bibendum.Nunc vel turpis scelerisque nunc fringilla ultrices. Pellentesque a tincidunt.";
                break;   
            }
            //--------------------------------------------------
            //ONLY FOR TEST PURPOSES
            //--------------------------------------------------

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
