using UnityEngine;
using System.Collections;

public class KeepPositionFromPlayer : MonoBehaviour {
	public Renderer follower;
	
	// Update is called once per frame
	void Update () {
		follower.transform.position = transform.position;
	}
}
