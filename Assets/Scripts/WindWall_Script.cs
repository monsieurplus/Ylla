using UnityEngine;
using System.Collections;

public class WindWall_Script : MonoBehaviour {

    [SerializeField]public GameObject refPlayer;
    [SerializeField]public GameObject refWindEmitter;

    [SerializeField]public int MAX_PARTICLE_INTENSITY;
    [SerializeField]public float FADING_DISTANCE;
    [SerializeField]public float MAXPOW_DISTANCE;

    private WindEmitterScript windScript;
    public float timedLife = 0;
    public bool causesStormFader = true;

    // Use this for initialization
    void Start () {
        windScript = refWindEmitter.GetComponent<WindEmitterScript>();
	}
	
	// Update is called once per frame
	void Update () {

        //Get the coordinates of the player relative to the wall
        Vector3 playerRelativePos = this.gameObject.transform.InverseTransformPoint(refPlayer.transform.position);
        playerRelativePos.z = playerRelativePos.z / this.gameObject.transform.lossyScale.z;

        //Depending on the distance, the intensity of the wind changes
        if (Mathf.Abs(playerRelativePos.z) < (MAXPOW_DISTANCE + FADING_DISTANCE))
        {
            int ParticleStrength = MAX_PARTICLE_INTENSITY;

            if (playerRelativePos.z > MAXPOW_DISTANCE) { ParticleStrength = Mathf.FloorToInt( ((FADING_DISTANCE - playerRelativePos.z + MAXPOW_DISTANCE) * MAX_PARTICLE_INTENSITY) / FADING_DISTANCE); }
            windScript.setStormStrength(this.gameObject, ParticleStrength, MAX_PARTICLE_INTENSITY);
        }
        else
        {
            windScript.setStormStrength(this.gameObject, 0, MAX_PARTICLE_INTENSITY);
        }

        //If Timed Life is different from 0, then the object has a limited lifespan
        if ( timedLife > 0 )
        {
            timedLife -= Time.deltaTime;

            //If timed life reaches 0 or less after this change, then the object is destroyed
            if ( timedLife <= 0 ) { Destroy(gameObject); }
        }

    }
}
