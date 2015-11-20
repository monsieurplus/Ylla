using UnityEngine;
using System.Collections;

public class WindEmitterScript : MonoBehaviour {

    [SerializeField]private GameObject refPlayer;
    [SerializeField]private GameObject refUI;
    [SerializeField]private GameObject windWallPrefab;

    private ParticleSystem windGenerator;
    private GameObject frameFirstWindWall = null;

    UI_Scripting UI_codeRef;

    [SerializeField]public float WALLSPAWN_DISTANCE;
    [SerializeField]public float GUST_INTERVAL;
    [SerializeField]public float GUST_INTERVAL_SPREAD;
    [SerializeField]public float GUST_DURATION;
    [SerializeField]public float GUST_DURATION_SPREAD;
    [SerializeField]public float GUST_POWER;
    [SerializeField]public float GUST_POWER_SPREAD;

    GameObject TemporaryWindWall;

    GameObject strongestWallThisFrame = null;
    Vector2 wallParameters = new Vector2(0, 1);

    bool LoopRandomGustsOfWind = false;
    float waitTimeUntilNextRandomGust;


    // Use this for initialization
    void Awake () {
        windGenerator = GetComponent<ParticleSystem>();
        windGenerator.simulationSpace = ParticleSystemSimulationSpace.World;

        waitTimeUntilNextRandomGust = GUST_INTERVAL + GUST_INTERVAL_SPREAD * ((Random.value * 2) - 1);

        startRandomWindGeneration(false);
    }
	
	//Start generating random gusts of wind. A bool allows to play the first gust immediately, or wait for first delay.
    public void startRandomWindGeneration( bool playFirstImmediately )
    {
        if ( playFirstImmediately ) { playRandomWindGust(); }
        LoopRandomGustsOfWind = true;

        StartCoroutine("WaitBetweenRandomWindGusts");
    }

    //Coroutine which manages the wait loop between 2 random gusts of wind.
    IEnumerator WaitBetweenRandomWindGusts()
    {
        while (LoopRandomGustsOfWind)
        {
            yield return new WaitForSeconds(waitTimeUntilNextRandomGust);

            playRandomWindGust();
            waitTimeUntilNextRandomGust = GUST_INTERVAL + GUST_INTERVAL_SPREAD * ((Random.value * 2) - 1);

        }

    }

    //Stop genrating random gusts of wind. If parameter RightNow == false, then one last gust of wind plays before ending random wind coroutine
    public void stopRandomWindGeneration( bool RightNow )
    {
        if (RightNow) { StopCoroutine("WaitBetweenRandomWindGusts"); }
        LoopRandomGustsOfWind = false;
    }

    //Change strength of storm. Affects particles and filter on GUI.
    public void setStormStrength(GameObject windWall, int newStrength = 0, int maxStrength = 500)
    {

        //First time functiuon is called, remember which object calls
        if (frameFirstWindWall == null) { frameFirstWindWall = windWall; }


        //When the "first object" calls this function, it meams we are at start of frame => Apply settings of strongest wall in previous frame
        if (windWall == frameFirstWindWall) {

            //We have a problem on the very first call : unassigned values. In that case use values of first wind wall as reference
            if ( strongestWallThisFrame == null ) { strongestWallThisFrame = windWall; wallParameters = new Vector2(newStrength, maxStrength); }

            //Get properties of the wall
            WindWall_Script wallProperties = strongestWallThisFrame.GetComponent<WindWall_Script>();
            
            //Because the storm strength is changed every frame while in range, we use this to replace the wind generator
            //First step is to get the forward vector of wind wall
            Vector3 relativeWallVector = strongestWallThisFrame.transform.forward;

            //Particle emitter is always at distance 10 from player, in same direction as the wall (= opposite of wall's relative forward vector)
            windGenerator.transform.position = refPlayer.transform.position - relativeWallVector.normalized * 10;

            //Particle emitter has to be rotated in same direction as wall, so particles "are in same direction as the wind"
            windGenerator.transform.rotation = strongestWallThisFrame.transform.rotation;

            //Move the particle generator and modify strength depending on the player's point of view, to get the most of the effect
            float playerWall_Angle = Vector3.Angle(refPlayer.transform.forward, windGenerator.transform.forward);
            float sign = Mathf.Sign(Vector3.Dot(refPlayer.transform.forward, windGenerator.transform.right));

            float angleRad = playerWall_Angle * sign * Mathf.PI / 180;

            //In case of tempest, move particle emitter depending on point of view. Not done for regular wind.
            float wallMove = 0;
            if ( wallProperties.causesStormFader == true ) { wallMove = Mathf.Sin(angleRad) * 20; }
             
            windGenerator.transform.position += wallMove * windGenerator.transform.right;

            windGenerator.emissionRate = (wallParameters.x / 2) + wallParameters.x * Mathf.Abs(Mathf.Cos(angleRad)) / 2;

            //In case of a "wind wall", an extra effect rakes the screen turn red. Effect is stronger when looking directly at the storm.
            if (wallProperties.causesStormFader)
            {
                float tempestFaderAlpha = wallParameters.x / wallParameters.y;

                if (playerWall_Angle > 90) { 
                    tempestFaderAlpha += tempestFaderAlpha*Mathf.Abs(Mathf.Cos(angleRad)) / 2;
                }

                if (tempestFaderAlpha > 0.95f ) { tempestFaderAlpha = 0.95f; }
                refUI.GetComponent<UI_Scripting>().changeTempestFaderOpacity(tempestFaderAlpha);
            }

            //Debug.Log(strongestWallThisFrame.name + " " + wallParameters + " " + (playerWall_Angle * sign) + " " + wallMove + " " + windGenerator.emissionRate + " " + newStrength);

            //Reset algorithm for next frame. This first wall is considered the strongest.
            strongestWallThisFrame = windWall;
            wallParameters = new Vector2(newStrength, maxStrength);

            
        }

        //Several wind walls are competing. Only the strongest shall prevail!
        if (newStrength > wallParameters.x)
        {
            strongestWallThisFrame = windWall;
            wallParameters = new Vector2(newStrength, maxStrength);
        }

    }

    //Immediately launches a random "Wind Gust" that will be witnessed by the player anywhere in the world.
    //The randomization occurs within the current parameters of value+spread for duration and power.
    public void playRandomWindGust()
    {
        //Generate its duration and power using the viariables : base value and spread of randomization
        float gustDuration = GUST_DURATION + GUST_DURATION_SPREAD*( (Random.value*2)-1 );
        float gustPower = GUST_POWER + GUST_POWER_SPREAD * ((Random.value * 2) - 1);

        //Generate a random direction for the wind
        float randomAngle = 2*Random.value*Mathf.PI - Mathf.PI;
        Vector3 gustVector = new Vector3(Mathf.Cos(randomAngle), 0,Mathf.Sin(randomAngle));

        //Launch generation
        TemporaryWindWall = generateWindSource( gustDuration, gustPower, gustVector);

    }

    //Genrates a temporary "wind source" using provided parameters. Function is public and can be used for winds effects in cutscenes, for instance.
    //Function returns the "wind source" gameObject. It is useful to keep a reference to that object just in case.
    public GameObject generateWindSource(float duration, float power, Vector3 windDirection)
    {
        GameObject generatedWindWall = Instantiate( windWallPrefab ) as GameObject;

        Vector3 spawnPos = refPlayer.transform.position - WALLSPAWN_DISTANCE*windDirection.normalized;
        generatedWindWall.transform.position = spawnPos;

        generatedWindWall.transform.LookAt(refPlayer.transform.position);

        WindWall_Script spawnedWallProperties = generatedWindWall.GetComponent<WindWall_Script>();

        spawnedWallProperties.refPlayer = refPlayer;
        spawnedWallProperties.refWindEmitter = gameObject;

        spawnedWallProperties.timedLife = duration;
        spawnedWallProperties.MAXPOW_DISTANCE = 2 * WALLSPAWN_DISTANCE;
        spawnedWallProperties.FADING_DISTANCE = 0;

        spawnedWallProperties.MAX_PARTICLE_INTENSITY = (int)power;
        spawnedWallProperties.causesStormFader = false;

        return generatedWindWall;
    }
}
