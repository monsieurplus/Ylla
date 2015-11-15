using UnityEngine;
using System.Collections;

public class KeepPositionFromPlayer : MonoBehaviour {
	public Renderer follower;

	private Vector3 followerStarPosition;
	private Vector3 playerStartPosition;

	// Use this for initialization
	void Start () {
		followerStarPosition = follower.transform.position;
		playerStartPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		follower.transform.position = followerStarPosition + (playerStartPosition - transform.position);
	}
}
