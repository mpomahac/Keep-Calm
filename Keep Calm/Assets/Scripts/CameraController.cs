using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour{

	/*---------------------------------------Privatne varijable---------------------------------------------*/
	private GameObject target; 				//objekt na koji se kamera fokusira
	private Vector3 offset;					//udaljenost kamere od objekta fokusa
	private Transform pozicijaKamere;		//trenutna pozicija kamere
	private Quaternion rotacija;			//željeni smjer pogleda kamere
	private Vector3 trenutnaRotacija;		//trenutni smjer pogleda kamere (koristi se za zoom na kocku)
	private Vector3 speed;					//brzina kamere prilikom kretanja, automatski se postavlja i mijenja

	/*---------------------------------------------Metode---------------------------------------------------*/
	//postavlja kameru na željenu rotaciju
	void Start(){
		rotacija = transform.rotation;		
	}

	//funkcija koja se poziva svaki frame, služi za pomicanje kamere ovisno o fokusu i rotiranje kamere prema sredini
	void Update () {

		//if-elseif-else petlja za određivanje rotacije ovisno o postavljenom fokusu
		if (target.name == "Center") {		
			offset = new Vector3 (0, 17, -17);
			rotacija.eulerAngles = new Vector3 (45, 0, 0);
			trenutnaRotacija = new Vector3 (45, 0, 0);
		} else if (target.name == "FokusKamereCrveni") {
			offset = new Vector3 (12, 15, -12);
			rotacija.eulerAngles = new Vector3 (45, -45, 0);
			trenutnaRotacija = new Vector3 (45, -45, 0);
		} else if (target.name == "FokusKamereZuti") {
			offset = new Vector3 (-12, 15, -12);
			rotacija.eulerAngles = new Vector3 (45, 45, 0);
			trenutnaRotacija = new Vector3 (45, 45, 0);
		} else if (target.name == "FokusKamereZeleni") {
			offset = new Vector3 (-12, 15, 12);
			rotacija.eulerAngles = new Vector3 (45, 135, 0);
			trenutnaRotacija = new Vector3 (45, 135, 0);
		} else if (target.name == "FokusKamerePlavi") {
			offset = new Vector3 (12, 15, 12);
			rotacija.eulerAngles = new Vector3 (45, -135, 0);
			trenutnaRotacija = new Vector3 (45, -135, 0);
		}
		else {
			if (trenutnaRotacija.y == -45) {
				offset = new Vector3 (target.transform.position.x + 3, 10, target.transform.position.z - 3);
			} else if (trenutnaRotacija.y == 45) {
				offset = new Vector3 (target.transform.position.x - 3, 10, target.transform.position.z - 3);
			} else if (trenutnaRotacija.y == 135) {
				offset = new Vector3 (target.transform.position.x - 3, 10, target.transform.position.z + 3);
			} else if (trenutnaRotacija.y == -135) {
				offset = new Vector3 (target.transform.position.x + 3, 10, target.transform.position.z + 3);
			}
		}

		//glatko pomicanje kamere s jedne pozicije na drugu
		transform.position = Vector3.SmoothDamp (transform.position, offset, ref speed, 0.5f);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotacija, 0.1f);
	}

	//postavljanje novog fokusa
	public void changeTarget(GameObject newTarget){
		target = newTarget;
	}
}
