using UnityEngine;
using System.Collections;

public class ArrowCanvas : MonoBehaviour {

	public GameObject arrowLeft;
	public GameObject arrowRight;
	public GameObject pausedText;

	// Use this for initialization
	public void Start () {
		arrowLeft = transform.FindChild ("Arrow Left").gameObject;
		arrowRight = transform.FindChild ("Arrow Right").gameObject;
		pausedText = transform.FindChild ("Paused Text").gameObject;
	}
	
	public void SetRightArrow(bool active)
	{
        Start();
		arrowRight.SetActive (active);
	}

	public void SetLeftArrow(bool active)
	{
        Start();
        arrowLeft.SetActive (active);
	}

	public void SetPasedText(bool active)
	{
        Start();
        pausedText.SetActive (active);
	}
}

