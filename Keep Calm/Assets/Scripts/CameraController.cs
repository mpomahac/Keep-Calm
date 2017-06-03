using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour{

	//target - objekt na koji se kamera fokusira
	public GameObject target;

	//offset - udaljenost kamere od objekta fokusa
	//pozicijaKamere - trenutna pozicija kamere
	private Vector3 offset;
	private Transform pozicijaKamere;

	//referenca na trenutnu brzinu kamere prilikom kretanja
	//vrijednost se postavlja / mijenja automatski
	private Vector3 speed;
	
	//pomice kameru na potrebnu poziciju tijekom cca. 0.5s
	void Update () {
		if (target.name == "Center") {
			offset = new Vector3 (0, 20, -15);
		} else {
			offset = new Vector3 (target.transform.position.x, 10, target.transform.position.z-5);
		}
		transform.position = Vector3.SmoothDamp (transform.position, offset, ref speed, 0.5f);
	}

	//prima novi objekt fokusa
	public void changeTarget(GameObject newTarget){
		target = newTarget;
	}
}
