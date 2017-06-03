using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Figura :MonoBehaviour{

	//referenca na model figure
	public GameObject figura;

	//public varijable skrivene u editoru/inspektoru
	//startPozicija - koordinate pozicije na koju je figura postavljena, koristi se u resetu
	//zeljenaPozicija - koordinate pozicije na koju bi figura trebala doc, jednaka startnoj do prvog pomaka figure
	//naStartu /naCilju - bool vrijednosti da li je figura na pocetnoj / krajnjoj poziciji
	//trenutniStup - index stupa na kojem se figura trenutno nalazi
	[HideInInspector]public Transform startPozicija;
	[HideInInspector]public Vector3 zeljenaPozicija;
	[HideInInspector]public bool naStartu = true;
	[HideInInspector]public bool naCilju=false;
	[HideInInspector]public int trenutniStup = -1;

	//referenca na trenutnu brzinu figure prilikom kretanja
	//vrijednost se postavlja / mijenja automatski
	private Vector3 speed;

	//primanje nove pozicije
	public void move(Transform novaPozicija){
		zeljenaPozicija=new Vector3 (novaPozicija.position.x, 6.01f, novaPozicija.position.z);
	}

	//pomice figuru na zeljenu poziciju tijeko cca. 0.5s
	void Update () {
		figura.transform.position = Vector3.SmoothDamp (figura.transform.position, zeljenaPozicija, ref speed, 0.5f);
	}

	//instantno resetira figuru na startnu poziciju, teleportira ju
	public void reset(){
		figura.transform.position = startPozicija.position;
		naStartu = true;
		trenutniStup = -1;
	}
}
