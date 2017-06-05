using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Igrac {

	/*----------------------------------------------Varijable----------------------------------------*/
	public Figura[] figure;									//lista igračevih figura
	public string ime;										//ime igrača
	public GameObject[] izlazniStupovi;						//lista stupova koji čine kučicu

	[HideInInspector]public int izlazniStup;				//index zadnjeg stupa prije kučice
	[HideInInspector]public int ulazniStup;					//index prvog stupa / ulazne pozicije
	[HideInInspector]public int zadnjiSlobodanIzlazni;		//index zadnjeg slobodnog stupa u kučici
	[HideInInspector]public int brojBacanja = 1;			//preostali broj bacanja u trenutnom krugu
	[HideInInspector]public bool prvoBacanje = true;		//oznaka je li trenutno igračev prvi krug
}
