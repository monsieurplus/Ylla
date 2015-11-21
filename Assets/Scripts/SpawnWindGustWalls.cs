using UnityEngine;
using System.Collections;

public class SpawnWindGustWalls : MonoBehaviour {

    [SerializeField] public float WALLSPAWN_DISTANCE;

    [SerializeField] public float GUST_INTERVAL;
    [SerializeField] public float GUST_INTERVAL_SPREAD;

    [SerializeField] public float GUST_DURATION;
    [SerializeField] public float GUST_DURATION_SPREAD;

    [SerializeField] public float GUST_POWER;
    [SerializeField] public float GUST_POWER_SPREAD;

    private const double ARC4RANDOM_MAX = 0x100000000;

    // Use this for initialization
    void Start () {
	
	}
	
	//Genrates a temporary "wind source" using provided parameters. Function is public and can be used for winds effects in cutscenes, for instance.
    //Function returns the "wind source" gameObject, which must be added to the scene by other means
    public void generateWindSource ( float duration, float position, float power, float windAngle)
    {

    }
    
}
