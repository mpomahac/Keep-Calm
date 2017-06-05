using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Igrac {

	//figure - lista figura
	//ime - ime igraca
	//izlazniStupovi - lista stupova koji predstavljaju cilj
	public Figura[] figure;
	public string ime;
	public GameObject[] izlazniStupovi;

	//public varijable skrivene u editoru/inspektoru
	//izlazniStup - index zadnjeg stupa prije cilja
	//ulazniStup - index prvog stupa / ulazna pozicija
	//zadnjiSlobodanIzlazni - zadnji prazan stup u cilji, vrijednost 0 - 3, vrijednost ipod 0 ozačava pobjednika
	[HideInInspector]public int izlazniStup;
	[HideInInspector]public int ulazniStup;
	[HideInInspector]public int zadnjiSlobodanIzlazni;
	[HideInInspector]public int brojBacanja = 1;
	[HideInInspector]public bool prvoBacanje = true;
}
