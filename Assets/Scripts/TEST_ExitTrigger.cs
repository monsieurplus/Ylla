using UnityEngine;
using System.Collections;

public class TEST_ExitTrigger : MonoBehaviour
{

    [SerializeField]GameObject refUI;
    [SerializeField]GameObject playerRef;


    // Use this for initialization
    void Start()
    {
        
    }

    // Test if player enters
    void Update()
    {

        if ( Vector3.Distance(transform.position, playerRef.transform.position) < 1000f)
        {
            GameObject OutroManager = refUI.transform.Find("OUTROGROUP").gameObject;

            OutroManager.SetActive(true);
            OutroCredits OutroScript = OutroManager.GetComponent<OutroCredits>();

            OutroScript.LaunchOutroSequence();

            this.enabled = false; 
        }
    }
}
