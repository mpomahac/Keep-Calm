using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour{

	//target - objekt na koji se kamera fokusira
	//offset - udaljenost kamere od objekta fokusa
	//pozicijaKamere - trenutna pozicija kamere
	//rotacija - zeljeni smjer pogleda kamere
	//trenutnaRotacija - trenutni smjer pogleda kamere, koristi se isključivo za zoom na kocku
	private GameObject target;
	private Vector3 offset;
	private Transform pozicijaKamere;
	private Quaternion rotacija;
	private Vector3 trenutnaRotacija;

	//referenca na trenutnu brzinu kamere prilikom kretanja
	//vrijednost se postavlja / mijenja automatski
	private Vector3 speed;

	//postavlja zeljenu rotaciju na trenutnu
	void Start(){
		rotacija = transform.rotation;
	}

	//pomice kameru na potrebnu poziciju tijekom cca. 0.5s i rotira ju prema centru
	void Update () {
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

		transform.position = Vector3.SmoothDamp (transform.position, offset, ref speed, 0.5f);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotacija, 0.1f);
	}

	//prima novi objekt fokusa
	public void changeTarget(GameObject newTarget){
		target = newTarget;
	}
}
