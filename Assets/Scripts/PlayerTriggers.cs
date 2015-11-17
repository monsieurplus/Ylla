using UnityEngine;
using System.Collections;

public class PlayerTriggers : MonoBehaviour {

    [SerializeField] private Canvas UI;

    private UI_Scripting UIScript;
    private AudioSource SoundPlayer;
    private Camera playerCamera;

    [SerializeField] AudioClip[] FallYellSounds;
    [SerializeField] AudioClip[] YllaRespawnLines;

    Vector3 lastCheckPointPosition;
    Quaternion lastCheckPointRotation;

    // Use this for initialization
    void Awake () {
        UIScript = UI.GetComponent<UI_Scripting>();
        SoundPlayer = GetComponent<AudioSource>();
        playerCamera = transform.Find("FirstPersonCharacter").gameObject.GetComponent<Camera>();
    }

    //--------------------------------------------------
    //ONLY FOR TEST PURPOSES
    //--------------------------------------------------
    void Start() { 
        UIScript.startQuickLinesLoop(SoundPlayer, YllaRespawnLines);
    }
    //--------------------------------------------------
    //ONLY FOR TEST PURPOSES
    //--------------------------------------------------

    // Update is called once per frame
    void OnTriggerEnter (Collider triggerCollider) {
	    
        //Test type of the trigger, first by tag, then mby name if necessary.
        //We probably will have lots of trigger types in the final code, so I am using "switch" and not "if"
        switch ( triggerCollider.tag )
        {
            //In case of a trigger to detect falling out of bounds
            case "FallHitbox":
                //A coroutine handles the "death by fall" event, it's easier to fine tune the real time aspect
                StartCoroutine("YllaSavesYourButt");
            break;

            //In case of a trigger to detect falling out of bounds
            case "CheckPoint":
                //Starts the function to save position and rotation of checkpoint
                SaveCheckPointCoordinates( triggerCollider );
            break;
        }

	}

    //Function to save the coordinates of the last collided checkpoint. The position and rotation of the checkpoint collider are used.
    void SaveCheckPointCoordinates( Collider checkpointCollider )
    {
        lastCheckPointPosition = checkpointCollider.transform.position;
        lastCheckPointRotation = checkpointCollider.transform.rotation;
    }

    //Function that teleports the player to a location. Also forces the camera ta a specific angle.
    void teleportPlayer(Vector3 position, Quaternion viewRotation )
    {
        //We need to split the rotation provided into two parts : the character rotation (look left/right) and the camera rotation (top/bottom)
        Vector3 angles = viewRotation.eulerAngles;
        Quaternion horizontalRot = Quaternion.Euler(0f, angles.y, 0f);
        Quaternion verticalRot = Quaternion.Euler(angles.x, 0f, 0f);

        //Get the script and transmit it the data
        UnityStandardAssets.Characters.FirstPerson.FirstPersonController charScript = GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        charScript.TeleportController(position, horizontalRot, verticalRot);
    }

    //Coroutine which manages the sequences of Events allowing the player to be saved by Ylla's space magic when falling
    IEnumerator YllaSavesYourButt()
    {
        //Requests screen fade to black
        UIScript.requestFadeToBlack(1.0f);
        yield return new WaitForSeconds(0.1f);

        //Plays sound of player character yelling from player audiosource
        //pick & play a random yell sound from the array,
        //excluding sound at index 0
        int n = Random.Range(1, FallYellSounds.Length); 
        SoundPlayer.clip = FallYellSounds[n]; 
        SoundPlayer.PlayOneShot(SoundPlayer.clip); 
        //move picked sound to index 0 so it's not picked next time
        FallYellSounds[n] = FallYellSounds[0]; 
        FallYellSounds[0] = SoundPlayer.clip; 
        //Wait for a duration equal to the sounds' length
        yield return new WaitForSeconds(SoundPlayer.clip.length + 0.5f);

        //While things are hidden, quickly teleport the player to last checkpoint
        teleportPlayer(lastCheckPointPosition, lastCheckPointRotation);

        //Request screen white flash out of black
        UIScript.requestWhiteFlashFromBlack();

        //Wait a little bit
        yield return new WaitForSeconds(0.2f);

        //Plays sound of Ylla making a snarky remark from player audiosource
        //pick & play a random line sound from the array,
        //excluding sound at index 0
        n = Random.Range(1, YllaRespawnLines.Length);
        SoundPlayer.clip = YllaRespawnLines[n];
        //Request the play of the line with subtitles through the function available in interface script
        UIScript.PlayLineWithSubtitles(SoundPlayer, SoundPlayer.clip);
        //move picked sound to index 0 so it's not picked next time
        YllaRespawnLines[n] = YllaRespawnLines[0];
        YllaRespawnLines[0] = SoundPlayer.clip;

    }
}
