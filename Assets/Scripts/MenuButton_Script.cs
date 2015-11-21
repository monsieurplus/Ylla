using UnityEngine;
using System.Collections;

public class MenuButton_Script : MonoBehaviour {

    [SerializeField]private GameObject refUI;
    [SerializeField]private AudioClip ButtonRollOverSound;

    UnityEngine.UI.Text textField;
    AudioSource audioSrc;

    // Use this for initialization
    void Start () {
        textField = gameObject.transform.Find("Text").gameObject.GetComponent<UnityEngine.UI.Text>(); ;
        audioSrc = refUI.GetComponent<AudioSource>();
    }

    // Changes on roll over
    public void ApplyRollOver()
    {
        textField.color = Color.black;

        //Play the roll over sound using AudioSource on the UI refenrence
        audioSrc.clip = ButtonRollOverSound;
        audioSrc.PlayOneShot(audioSrc.clip);

    }

    // Changes on roll out
    public void ApplyRollOut()
    {
        textField.color = Color.white;
    }

    // Changes on click
    public void ApplyClick()
    {
        textField.color = Color.white;
    }

}
