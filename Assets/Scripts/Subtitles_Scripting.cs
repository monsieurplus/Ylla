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
		captions["desertScene01_dialog09"] = "Bon ! Puisque vous n’êtes pas lui, vous ne m’êtes d’aucune utilité.";
		captions["desertScene01_dialog10"] = "Maintenant, laissez-moi tranquille. Et bonne journée.";

		captions["desertScene02_dialog01"] = "Encore vous ?!!";
		captions["desertScene02_dialog02"] = "Mais ça ne va pas, ça ! Ça ne va pas du tout !";
		captions["desertScene02_dialog03"] = "C’est notre chanson à NOUS ! Elle ne devrait pas vous faire venir VOUS !";
		captions["desertScene02_dialog04"] = "Fichez moi la paix ! Bonne journée !";

		captions["desertScene03_dialog01"] = "Je ne le trouverai pas... Cela fait si longtemps que je le cherche, que j’essaie de comprendre...";
		captions["desertScene03_dialog02"] = "Je suis pourtant certaine qu’il était ici, les images, les sons, sa voix, tout pointait vers cet endroit…. Alors pourquoi ?";
		captions["desertScene03_dialog03"] = "Qu’a-t-il bien pu se passer ? J’ai besoin de réponses. Je tourne en rond sur cette histoire depuis si longtemps...";
		captions["desertScene03_dialog04"] = "Oh... Vous voilà encore ?";
		captions["desertScene03_dialog05"] = "Notre chanson semble vous attirer. Vous êtes de toute évidence de la troisième planète vous aussi... Ça ne peut pas être entièrement une coïncidence.";
		captions["desertScene03_dialog06"] = "Suivez moi, j’ai quelque chose à vous montrer.";

		captions["bridgeScene01_dialog01"] = "Vous avez pris votre temps !";
		captions["bridgeScene01_dialog02"] = "Cela m’a permis de préparer les derniers détails. J’ai besoin de toute votre attention, suivez-moi !";

		captions["bridgeScene02_dialog01"] = "Je suis à la recherche d’une personne qui m’est chère. Un homme de la Terre, tout comme vous.";
		captions["bridgeScene02_dialog02"] = "Il y a une chose que vous devez bien comprendre : tous ces évènement ce sont passés il y a des années de cela. A l’époque où nous autres martiens étions encore pourvus de corps, de jambes, de bras... comme vous.";
		captions["bridgeScene02_dialog03"] = "Bien du temps a passé depuis, et certains... évènements nous ont amené à chercher le moyen de nous libérer des maux physiques. Ce à quoi nous sommes finalement parvenus.";
		captions["bridgeScene02_dialog04"] = "Nous vivons désormais sous forme de sphères incandescentes, dans les vents, les cieux et les collines.";
		captions["bridgeScene02_dialog05"] = "Mais pour mener cette existence éthérée, le regret est un frein. Un frein qui depuis des années me retient ici. Je ne serai libre que lorsque j’aurai compris ce qui est arrivé à mon bel astronaute.";
		captions["bridgeScene02_dialog06"] = "Comme vous l’aurez peut-être compris, je piétine dans mes recherches. Un avis extérieur ne serait pas de refus… Accepteriez-vous de m’aider ?";
		captions["bridgeScene02_dialog07"] = "Je suis certaine que vous n’êtes pas là par hasard. Les forces de l’esprit y sont forcément pour quelque chose.";
		captions["bridgeScene02_dialog08"] = "Voilà ce que je vous propose : si vous m’aidez à comprendre ce qui s’est passé, je vous aiderais à rentrer chez vous. En tant qu’être d'éther, je possède certaines facultés. Aidez moi, et je promets de vous aider en retour.";

        captions["bridgeFall_dialog01"] = "Vous devriez faire attention. Ces eaux ne sont pas très amicales pour les êtres matériels comme vous.";
        captions["bridgeFall_dialog02"] = "Évidemment vous êtes sujet à la gravité... Faites un peu attention. Je ne vais pas passer ma journée à vous repêcher!";
        captions["bridgeFall_dialog03"] = "J'espère que vous serez plus adroit pour chercher dans mes souvenirs, que vous ne l'êtes pour marcher droit!";
        captions["bridgeFall_dialog04"] = "Mon mari aussi était maladroit. Évidemment maintenant qu'il flotte au dessus du sol, cela se voit beaucoup moins.";
    }
	
	// Function to display the proper text in this subtitle box. Name of Audioclip is used as search key.
	public void DisplaySubtitleMatchingTheSound ( AudioClip soundType ) {
        string subtitletext = "";
    
		if ( captions.ContainsKey(soundType.name) ) {
			subtitletext = captions [soundType.name];
		}
		else {
			subtitletext = "[MISSING CAPTION " + soundType.name + "]";
		}

        if ( subtitletext != "" ) {
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
