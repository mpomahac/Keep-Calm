using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Figura :MonoBehaviour{

	//public varijable skrivene u editoru/inspektoru
	//startPozicija - koordinate pozicije na koju je figura postavljena, koristi se u resetu
	//zeljenaPozicija - koordinate pozicije na koju bi figura trebala doc, jednaka startnoj do prvog pomaka figure
	//naStartu /naCilju - bool vrijednosti da li je figura na pocetnoj / krajnjoj poziciji
	//trenutniStup - index stupa na kojem se figura trenutno nalazi
	[HideInInspector]public Vector3 startPozicija;
	[HideInInspector]public Vector3 zeljenaPozicija;
	[HideInInspector]public bool naStartu = true;
	[HideInInspector]public bool naCilju=false;
	[HideInInspector]public int trenutniStup = -1;
	[HideInInspector]public float udaljenostX;
	[HideInInspector]public float udaljenostZ;

	private Animator anim;
	private bool ulaz = true;
	private Quaternion ciljnaRotacija;

	//referenca na trenutnu brzinu figure prilikom kretanja
	//vrijednost se postavlja / mijenja automatski
	private Vector3 speed;

	void Start(){
		anim = GetComponent<Animator> ();
		ciljnaRotacija = transform.rotation;
	}

	//primanje nove pozicije
	public void move(Transform novaPozicija, bool ulaz){
		zeljenaPozicija=new Vector3 (novaPozicija.position.x, 6.01f, novaPozicija.position.z);
		this.ulaz = ulaz;
	}

	//pomice figuru na zeljenu poziciju tijeko cca. 0.5s
	void Update () {
		if (ulaz) {
			this.transform.position = Vector3.SmoothDamp (gameObject.transform.position, zeljenaPozicija, ref speed, 1f);
		}
		else {
			udaljenostX = gameObject.transform.position.x - zeljenaPozicija.x;
			udaljenostZ = gameObject.transform.position.z - zeljenaPozicija.z;

			if (udaljenostX > 0.2) {
				anim.SetBool ("Move", true);
			} else if (udaljenostX < -0.2) {
				anim.SetBool ("Move", true);
			} else if (udaljenostZ > 0.2) {
				anim.SetBool ("Move", true);
			} else if (udaljenostZ < -0.2) {
				anim.SetBool ("Move", true);
			} else {
				anim.SetBool ("Move", false);
			}
		}

		//rotiranje
		transform.rotation = Quaternion.Slerp (transform.rotation,ciljnaRotacija,0.15f);
	}

	public void reset(){
		transform.position = startPozicija;
		zeljenaPozicija = startPozicija;
		transform.eulerAngles = new Vector3 (0, 0, 0);
		naStartu = true;
		trenutniStup = -1;
	}

	public void centriranje(Transform stup){
		if(!ulaz)
			this.transform.SetPositionAndRotation (new Vector3 (stup.position.x, 6.01f, stup.position.z), this.transform.rotation);
	}

	public void rotiraj(float stupnjevi){
		anim.SetBool ("Move", false);
		ciljnaRotacija.eulerAngles = new Vector3 (0, transform.eulerAngles.y+stupnjevi, 0);
	}
}
