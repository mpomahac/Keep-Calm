using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Igrac {

	//figure - lista figura
	//ime - ime igraca
	//izlazniStupovi - lista stupova koji predstavljaju cilj / kucicu
	public Figura[] figure;
	public string ime;
	public GameObject[] izlazniStupovi;

	//public varijable skrivene u editoru/inspektoru
	//izlazniStup - index zadnjeg stupa prije cilja / kucice
	//ulazniStup - index prvog stupa / ulazna pozicija
	//zadnjiSlobodanIzlazni - zadnji prazan stup u cilji, vrijednost 0 - 3, vrijednost ipod 0 ozačava pobjednika
	//brojBacanja - preostali broj bacanja u trenutnom krugu
	//prvoBacanje - provjerava je li igracevo prvo bacanje kockice
	[HideInInspector]public int izlazniStup;
	[HideInInspector]public int ulazniStup;
	[HideInInspector]public int zadnjiSlobodanIzlazni;
	[HideInInspector]public int brojBacanja = 1;
	[HideInInspector]public bool prvoBacanje = true;
}
