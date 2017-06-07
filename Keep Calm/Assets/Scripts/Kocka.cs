using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kocka : MonoBehaviour {

	//referenca na objekt kocke
	/*---------------------------------------Varijable-----------------------------------------*/
	public GameObject kocka;					//referenca na objekt kocke

	private Quaternion ciljnaRotacija;			//željena završna rotacija kocke
	private float speed;						//brzina rotacije kocke

	//postavljanje ciljne rotacije na pocetnu
	/*-----------------------------------------Metode------------------------------------------*/
	//postavljanje željene rotacije na početnu rotaciju
	void Start(){
		ciljnaRotacija = transform.rotation;
	}

	//izvršavanje rotacije
	void Update () {
		transform.Rotate (new Vector3 (45, 45, 45) * speed);

		if (speed == 0) {
			transform.rotation = Quaternion.Slerp (transform.rotation, ciljnaRotacija, 0.1f);
		}
	}

	//promjena brzine rotacije
	public void rotiraj (float brzina){
		speed = brzina;
	}

	//postavljanje ciljne rotacije s obzirom na dobiveni broj tako da je dobiveni broj na gornjoj strani kocke
	public void naBroj (int x){
		switch (x) {
		case 1:
			ciljnaRotacija.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
			break;
		case 2:
			ciljnaRotacija.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 90);
			break;
		case 3:
			ciljnaRotacija.eulerAngles = new Vector3 (-90, transform.eulerAngles.y, transform.eulerAngles.z);
			break;
		case 4:
			ciljnaRotacija.eulerAngles = new Vector3 (90, transform.eulerAngles.y, transform.eulerAngles.z);
			break;
		case 5:
			ciljnaRotacija.eulerAngles = new Vector3 (0, transform.eulerAngles.y, -90);
			break;
		case 6:
			ciljnaRotacija.eulerAngles = new Vector3 (180, transform.eulerAngles.y, 0);
			break;
		}
	}
}
