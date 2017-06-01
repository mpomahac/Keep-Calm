using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

	public Transform[] put;
	private int indexTrenutnePozicije;
	private int indexZeljenePozicije;
	private Vector3 zeljenaPozicija;
	private Vector3 speed;

	// Use this for initialization
	void Start () {
		int i = 0;
		do {
			if (transform.position.x == put [i].position.x && transform.position.z == put [i].position.z) {
				indexTrenutnePozicije = i;
				indexZeljenePozicije = i;
				zeljenaPozicija = new Vector3 (put [indexZeljenePozicije].position.x, transform.position.y, put [indexZeljenePozicije].position.z);
				break;
			} else
				i++;
		} while(true);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.SmoothDamp (transform.position, zeljenaPozicija, ref speed, 0.2f);
		if (transform.position == zeljenaPozicija)
			indexTrenutnePozicije = indexZeljenePozicije;
	}

	public void zeljenaPozicijaChange(){
		indexZeljenePozicije = indexTrenutnePozicije + 1;
		if (indexZeljenePozicije >= 40)
			indexZeljenePozicije = 0;
		zeljenaPozicija = new Vector3 (put [indexZeljenePozicije].position.x, transform.position.y, put [indexZeljenePozicije].position.z);
	}
}
