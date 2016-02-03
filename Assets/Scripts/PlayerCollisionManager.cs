using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCollisionManager : MonoBehaviour {
	//private Collider[] colliders;

	List<Collider> colliders = new List<Collider>();

	// Use this for initialization
	/*void Start () {
	
	}*/
	
	// Update is called once per frame
	/*void Update () {
	
	}*/

	void OnTriggerEnter (Collider collider) {
		
		// If the collider is not in the array, add it
		if (!colliders.Contains(collider)) {
			colliders.Add (collider);
		}
	}

	void OnTriggerExit (Collider collider) {
		colliders.Remove (collider);
	}

	public bool isColliding (GameObject item) {
		Collider itemCollider = item.GetComponent<Collider> ();

		if (itemCollider == null) {
			return false;
		}

		return (colliders.Contains (itemCollider));
	}
}
