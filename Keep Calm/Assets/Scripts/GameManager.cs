using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	//igrac - lista igraca, popunjena iz editora / inspektora
	//put - lista stupova popunjena iz editora / inspektora JER IH JE IZ NEKOG RAZLOGA BACALO NASUMICNO MAJKU MU JEBEM TREBA MI PO 5 MIN DA IH SVE POSPAJAM
	//kamera - referenca na kameru
	//kocka - referenca na kocku, trenutno sluzi samo za zoom kamere na nju, jos se ne vrti
	//dropdownListFigure - referenca na dropdown listu za odabir figure
	//text - ispisuje broj dobiven na kocki, privremeno rjesenje dok ne sredim kocku
	//pobjedaText - ispisuje pobjednika
	public Igrac[] igrac;
	public GameObject[] put;
	public CameraController kamera;
	public Kocka kocka;
	public Dropdown dropdownListFigure;
	public Text pobjedaText;
	public Text trenutniIgracText;

	//trenutniIgrac - index igraca koji je trenutno na potezu, 0 - 3
	//waitKocka - vrijeme cekanja da se kocka izvrti, skraceno na 2s dok ne sredim kocku, bit ce 5-6
	//waitKamera - vrijeme cekanja da kamera stigne do sljedece mete
	//waitNakonPomaka - vrijeme cekanja nakon pomaka, nisam siguran kolko je potrebno sad kad sam rijesil neke od gresaka
	//brojMjesta - broj dobiven na "kocki", maknut cu navodnike kad sredim kocku
	//gumbZaBacanje - bool vrijednost za prekid cekanja da neko stisne gumb i baci "kocku", pogledaj gore za navodnike
	//pomakniFiguru - index odabrane figure, dok je -1 ceka se odabir
	//pobjednik - self explanatory
	private int trenutniIgrac = 0;
	private WaitForSeconds waitKocka;
	private WaitForSeconds waitKamera;
	private WaitForSeconds waitNakonPomaka;
	private WaitForSeconds waitNakonReda;
	private int brojMjesta;
	private bool gumbZaBacanje = false;
	private int pomakniFiguru = -1;
	private Igrac pobjednik=null;
	private int brojIgraca=0;
	private GameObject buttonBaci;
	private bool bacanjeRed = true;
	private int bacanjeRedMax = 0;
	private int prviIgracIndex = 0;
	private bool kreni=false;

	private string[] imenaIgraca = { "", "", "", "" };
	private GameObject imeIgraca1;
	private GameObject imeIgraca2;
	private GameObject imeIgraca3;
	private GameObject imeIgraca4;
	private GameObject textImeIgraca1;
	private GameObject textImeIgraca2;
	private GameObject textImeIgraca3;
	private GameObject textImeIgraca4;
	private GameObject pocni;
	private GameObject quitButton;
	private GameObject win;
	private GameObject resetButton;

	//inicijalizacija
	void Start () {

		win = GameObject.Find ("Win");
		resetButton = GameObject.Find ("ResetButton");
		imeIgraca1 = GameObject.Find ("ImeIgraca1");
		imeIgraca2 = GameObject.Find ("ImeIgraca2");
		imeIgraca3 = GameObject.Find ("ImeIgraca3");
		imeIgraca4 = GameObject.Find ("ImeIgraca4");
		textImeIgraca1 = GameObject.Find ("TextIme1");
		textImeIgraca2 = GameObject.Find ("TextIme2");
		textImeIgraca3 = GameObject.Find ("TextIme3");
		textImeIgraca4 = GameObject.Find ("TextIme4");
		pocni = GameObject.Find ("Pocni");
		quitButton = GameObject.Find ("QuitButton");
		dropdownListFigure.gameObject.SetActive (false);
		buttonBaci = GameObject.Find ("Button");
		buttonBaci.SetActive (false);

		imeIgraca1.SetActive (false);
		imeIgraca2.SetActive (false);
		imeIgraca3.SetActive (false);
		imeIgraca4.SetActive (false);
		textImeIgraca1.SetActive (false);
		textImeIgraca2.SetActive (false);
		textImeIgraca3.SetActive (false);
		textImeIgraca4.SetActive (false);
		pocni.SetActive (false);
		win.SetActive (false);
		resetButton.SetActive (false);
		//postavljanje pocetnih pozicija figura
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				igrac [i].figure [j].startPozicija = igrac [i].figure [j].transform.position;
				igrac [i].figure [j].zeljenaPozicija = igrac [i].figure [j].transform.position;
			}
		}

		//postavljanje osnovnih stupova (ulazni, izlazni, broj slobodnih u cilju) - razlog zašto se tu rikta broj slobodnih: NEŠTO GA JE JEBALO PA JE ON JEBAL MENE SAT VREMENA PA SAM JA SJEBAL NJEGA
		igrac [0].izlazniStup = 39;
		igrac [0].ulazniStup = 1;
		igrac [0].zadnjiSlobodanIzlazni = 3;
		igrac [1].izlazniStup = 9;
		igrac [1].ulazniStup = 11;
		igrac [1].zadnjiSlobodanIzlazni = 3;
		igrac [2].izlazniStup = 19;
		igrac [2].ulazniStup = 21;
		igrac [2].zadnjiSlobodanIzlazni = 3;
		igrac [3].izlazniStup = 29;
		igrac [3].ulazniStup = 31;
		igrac [3].zadnjiSlobodanIzlazni = 3;

		//postavljanje vremena cekanja
		waitKocka = new WaitForSeconds (0.5f);
		waitKamera = new WaitForSeconds(1.2f);
		waitNakonPomaka = new WaitForSeconds (2f);
		waitNakonReda = new WaitForSeconds (1.2f);

		kamera.changeTarget (GameObject.Find ("Center"));

		//pokretanje igre
		StartCoroutine (gameloop ());
	}

	//main funkcija
	private IEnumerator gameloop(){

		while (brojIgraca == 0 || !kreni) {
			yield return null;
		}

		yield return StartCoroutine (turn ());

		//pogledaj da li postoji pobjednik, ako da ipisi ga, ako ne idemo dalje
		if ((pobjednik = pobjeda ()) != null) {
			pobjedaText.text = "Pobijedio je " + pobjednik.ime;
			quitButton.SetActive (true);
			resetButton.SetActive (true);
		} else {
			StartCoroutine (gameloop ());
		}
	}

	private IEnumerator turn(){
		string colorString="ffffffff";
		switch (trenutniIgrac) {
		case 0:
			colorString = "e00000ff";
			break;
		case 1:
			colorString = "e0e000ff";
			break;
		case 2:
			colorString = "00c000ff";
			break;
		case 3:
			colorString = "0060ffff";
			break;
		}
		trenutniIgracText.text ="Trenutno je na potezu <color=#"+colorString+">"+ imenaIgraca[trenutniIgrac]+"</color>";

		kamera.changeTarget(GameObject.Find("FokusKamere"+igrac[trenutniIgrac].ime));

		//cekaj da se stisne gumb za bacanje
		while (!gumbZaBacanje) {
			yield return null;
		}

		if (!bacanjeRed && igrac [trenutniIgrac].prvoBacanje) {
			igrac [trenutniIgrac].brojBacanja = 3;
			igrac [trenutniIgrac].prvoBacanje = false;
		}

		//pokreni bacanje "kocke" i cekaj da zavrsi
		yield return StartCoroutine (bacanjeKocke ());

		if (!bacanjeRed) {
			//resetiraj listu figura koje se mogu pomaknut
			dropdownListFigure.ClearOptions ();
			List<string> opcije = new List<string> ();
			//standardna opcija
			opcije.Add ("Odaberi figuru");
			//pregledaj koje figure se smiju pomaknut i stavi ih u listu
			if (eligable (igrac [trenutniIgrac].figure [0], 0))
				opcije.Add (igrac [trenutniIgrac].figure [0].gameObject.name);
			if (eligable (igrac [trenutniIgrac].figure [1], 1))
				opcije.Add (igrac [trenutniIgrac].figure [1].gameObject.name);
			if (eligable (igrac [trenutniIgrac].figure [2], 2))
				opcije.Add (igrac [trenutniIgrac].figure [2].gameObject.name);
			if (eligable (igrac [trenutniIgrac].figure [3], 3))
				opcije.Add (igrac [trenutniIgrac].figure [3].gameObject.name);
			dropdownListFigure.AddOptions (opcije);

			//cekaj dok se ne odabere figura, ako nijednoj nije dozvoljeno pomicanje nastavi dalje
			while (pomakniFiguru == -1) {
				if (dropdownListFigure.options.Count == 1)
					break;
				yield return null;
			}

			//ako je odabrana figura pomakni ju, ako nije preskoci
			if (pomakniFiguru != -1) {
				kamera.changeTarget (igrac [trenutniIgrac].figure [pomakniFiguru].gameObject);
				yield return StartCoroutine (cekanjeKamere ());

				//ako je na startu postavi ju na ulazni stup
				if (igrac [trenutniIgrac].figure [pomakniFiguru].naStartu) {
					float rotacija = (float)((trenutniIgrac - 1) * 90);
					igrac [trenutniIgrac].figure [pomakniFiguru].move (put [igrac [trenutniIgrac].ulazniStup].transform, true);
					igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup = igrac [trenutniIgrac].ulazniStup;
					igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (rotacija);
					igrac [trenutniIgrac].figure [pomakniFiguru].naStartu = false;
					yield return StartCoroutine (cekanjePomaka ());
				}
				//ako moze uc u cilj, teleportira ju tamo, da se napravit da ide korak po korak ako ce radit probleme
				else if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup + brojMjesta - igrac [trenutniIgrac].izlazniStup == igrac [trenutniIgrac].zadnjiSlobodanIzlazni + 1 &&
				         igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup <= igrac [trenutniIgrac].izlazniStup) {

					int uKucici=brojMjesta;

					for (int x = 1; x <= brojMjesta; x++) {
						igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup += 1;

						for (int i = 0; i < 4; i++) {
							if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == 1 + i * 10) {
								igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
								yield return StartCoroutine (cekanjeReda ());
							} else if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == 9 + i * 10) {
								igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
								yield return StartCoroutine (cekanjeReda ());
							} else if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == 5 + i * 10) {
								igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (-90.0f);
								yield return StartCoroutine (cekanjeReda ());
							}
						}
						if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == igrac [trenutniIgrac].izlazniStup) {
							igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
							yield return StartCoroutine (cekanjeReda ());
						}
						igrac [trenutniIgrac].figure [pomakniFiguru].move (put [igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup].transform, false);
						yield return StartCoroutine (cekanjePomaka ());

						uKucici--;

						if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == igrac [trenutniIgrac].izlazniStup) {
							igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
							break;
						}
					}
						
					for (int i = 0; i < uKucici; i++) {
						igrac [trenutniIgrac].figure [pomakniFiguru].move (igrac[trenutniIgrac].izlazniStupovi[i].transform, false);
						igrac [trenutniIgrac].figure [pomakniFiguru].trenutniIzlazniStup++;
					}

					igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup = -1;
					igrac [trenutniIgrac].figure [pomakniFiguru].uKucici = true;
						
					if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniIzlazniStup == igrac [trenutniIgrac].zadnjiSlobodanIzlazni) {
						igrac [trenutniIgrac].zadnjiSlobodanIzlazni--;
						igrac [trenutniIgrac].figure [pomakniFiguru].naCilju = true;
					}
				}
				//pomakni ju normalno, stup po stup
				else {
					for (int x = 1; x <= brojMjesta; x++) {
						igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup += 1;
						if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup >= 40)
							igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup = 0;
						
						for (int i = 0; i < 4; i++) {
							if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == 1 + i * 10) {
								igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
								yield return StartCoroutine (cekanjeReda ());
							} else if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == 9 + i * 10) {
								igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
								yield return StartCoroutine (cekanjeReda ());
							} else if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == 5 + i * 10) {
								igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (-90.0f);
								yield return StartCoroutine (cekanjeReda ());
							}
						}
						if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == igrac [trenutniIgrac].izlazniStup) {
							igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
							yield return StartCoroutine (cekanjeReda ());
						}
						igrac [trenutniIgrac].figure [pomakniFiguru].move (put [igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup].transform, false);
						yield return StartCoroutine (cekanjePomaka ());
					}
				}

				//vidi jel doslo do sudara
				sudar (igrac [trenutniIgrac].figure [pomakniFiguru]);

				igrac [trenutniIgrac].figure [pomakniFiguru].centriranje (put [igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup].transform);

				yield return StartCoroutine (cekanjeReda ());
			}
		}



		//resetira listu figura, resetira index odabrane figure, pomice kameru na centar, prebacuje na sljedeceg igraca, resetira gumb za bacanje
		dropdownListFigure.value = 0;
		dropdownListFigure.ClearOptions();
		pomakniFiguru = -1;
		kamera.changeTarget(GameObject.Find("FokusKamere"+igrac[trenutniIgrac].ime));
		igrac [trenutniIgrac].brojBacanja--;
		StartCoroutine (cekanjeKamere ());
		if (bacanjeRed) {
			if (brojMjesta > bacanjeRedMax) {
				prviIgracIndex = trenutniIgrac;
				bacanjeRedMax = brojMjesta;
			} else if (brojMjesta == bacanjeRedMax) {
				igrac [trenutniIgrac].brojBacanja++;
			}

			if (igrac [trenutniIgrac].brojBacanja == 0) {
				trenutniIgrac++;
				if (trenutniIgrac >= brojIgraca) {
					trenutniIgrac = prviIgracIndex;
					bacanjeRed = false;
				}
			}
		} else {
			if (brojMjesta == 6) {
				igrac [trenutniIgrac].brojBacanja = 1;
			}
				
			if (igrac [trenutniIgrac].brojBacanja == 0) {
				igrac [trenutniIgrac].brojBacanja = 1;
				trenutniIgrac++;
				if (trenutniIgrac >= brojIgraca) {
					trenutniIgrac = 0;
				}
			}
		}

		gumbZaBacanje = false;
		//kraj funkcije, nazad u gameloop
	}

	//bacanje kocke (duh...)
	private IEnumerator bacanjeKocke(){
		//pomakni kameru na kocku, odradi random, ispisi kolko je dobiveno na "kocki" jer jos nisam napravil da se kocka vrti
		kamera.changeTarget (kocka.kocka);
		brojMjesta = Random.Range (1, 7);
		
		//vrtnja kocke
		kocka.rotiraj (1);
		yield return waitKocka;
		kocka.rotiraj (0.9f);
		yield return waitKocka;
		kocka.rotiraj (0.8f);
		yield return waitKocka;
		kocka.rotiraj (0.7f);
		yield return waitKocka;
		kocka.rotiraj (0.6f);
		yield return waitKocka;
		kocka.rotiraj (0.5f);
		yield return waitKocka;
		kocka.rotiraj (0.4f);
		yield return waitKocka;
		kocka.rotiraj (0.3f);
		yield return waitKocka;
		kocka.rotiraj (0.2f);
		yield return waitKocka;
		kocka.naBroj (brojMjesta);
		kocka.rotiraj (0);
		yield return waitNakonPomaka;

		kamera.changeTarget(GameObject.Find("FokusKamere"+igrac[trenutniIgrac].ime));
	}
		
	private IEnumerator cekanjeKamere(){
		yield return waitKamera;
	}

	private IEnumerator cekanjePomaka(){
		yield return waitNakonPomaka;
	}

	private IEnumerator cekanjeReda(){
		yield return waitNakonReda;
	}

	//detektira pritisak gumba za bacanje kocke
	public void baci(){
		gumbZaBacanje = true;
	}

	//detektira koja figura je odabrana za pomicanje
	public void odaberiFiguru(){
		//izgleda komplicirano al je zapravo jako jednostavno, uzme samo zadnji znak stringa
		switch (dropdownListFigure.captionText.text.ToString ().Substring(dropdownListFigure.captionText.text.ToString().Length-1)) {
		case "1":
			pomakniFiguru = 0;
			break;
		case "2":
			pomakniFiguru = 1;
			break;
		case "3":
			pomakniFiguru = 2;
			break;
		case "4":
			pomakniFiguru = 3;
			break;
		}
	}

	//gleda da li je figuri dozvoljeno kretanje
	//dozvoljeno je ako je figura na startu i dobiven je 6, ako figura nije na startu niti cilju i pomakom za dobiven broj ne prelazi izlazni stup,
	//ako pomakom za dobiveni broj prelazi izlazni stup i ulazi u cilj
	private bool eligable(Figura figura, int index){
		if (figura.naStartu && brojMjesta == 6 && provjeraPozicije (figura, index))
			return true;
		else if (!figura.naStartu && !figura.naCilju && (figura.trenutniStup + brojMjesta <= igrac [trenutniIgrac].izlazniStup || figura.trenutniStup > igrac [trenutniIgrac].izlazniStup) && provjeraPozicije (figura, index))
			return true;
		else if (!figura.naStartu && !figura.naCilju && figura.trenutniStup + brojMjesta - igrac [trenutniIgrac].izlazniStup == igrac [trenutniIgrac].zadnjiSlobodanIzlazni + 1)
			return true;
		else if (figura.uKucici && brojMjesta+figura.trenutniIzlazniStup<=igrac[trenutniIgrac].zadnjiSlobodanIzlazni && provjeraKucice(figura))
			return true;
		else
			return false;
	}

	private bool provjeraPozicije(Figura figura, int index){
		for (int i = 0; i < 4; i++) {
			if (i != index) {
				if (!figura.naStartu && igrac [trenutniIgrac].figure [i].trenutniStup == figura.trenutniStup + brojMjesta) {
					return false;
				} else if (figura.naStartu && igrac [trenutniIgrac].figure [i].trenutniStup == igrac [trenutniIgrac].ulazniStup) {
					return false;
				}
			}
		}
		return true;
	}

	private bool provjeraKucice(Figura figura){
		for (int i = 0; i < 4; i++) {
			if (figura.trenutniIzlazniStup + brojMjesta == igrac [trenutniIgrac].figure [i].trenutniIzlazniStup)
				return false;
		}
		return true;
	}

	//gleda dali je neka figura već na tom mjestu i resetira ju ako je
	private void sudar(Figura pridoslica){
		for (int i = 0; i < 4; i++) {
			if (i != trenutniIgrac) {
				for (int j = 0; j < 4; j++) {
					if (igrac [i].figure [j].trenutniStup == pridoslica.trenutniStup) {
						igrac [i].figure [j].reset ();
						break;
					}
				}
			}
		}
	}

	//pregledava da li neki od igraca ima ispunjen uvjet za pobjedu, broj slobodnih stupova u cilju 0 ili manje preko index zadnjeg slobodnog
	private Igrac pobjeda(){
		for (int i = 0; i < 4; i++) {
			if (igrac [i].zadnjiSlobodanIzlazni < 0)
				return igrac [i];
		}
		return null;
	}

	public void brojIgraca2(){
		brojIgraca = 2;
		GameObject.Find ("BrojIgraca2").SetActive (false);
		GameObject.Find ("BrojIgraca3").SetActive (false);
		GameObject.Find ("BrojIgraca4").SetActive (false);

		imeIgraca1.SetActive (true);
		imeIgraca2.SetActive (true);
		textImeIgraca1.SetActive (true);
		textImeIgraca2.SetActive (true);
		pocni.SetActive (true);
	}

	public void brojIgraca3(){
		brojIgraca = 3;
		GameObject.Find ("BrojIgraca2").SetActive (false);
		GameObject.Find ("BrojIgraca3").SetActive (false);
		GameObject.Find ("BrojIgraca4").SetActive (false);

		imeIgraca1.SetActive (true);
		imeIgraca2.SetActive (true);
		imeIgraca3.SetActive (true);
		textImeIgraca1.SetActive (true);
		textImeIgraca2.SetActive (true);
		textImeIgraca3.SetActive (true);
		pocni.SetActive (true);
	}

	public void brojIgraca4(){
		brojIgraca = 4;
		GameObject.Find ("BrojIgraca2").SetActive (false);
		GameObject.Find ("BrojIgraca3").SetActive (false);
		GameObject.Find ("BrojIgraca4").SetActive (false);

		imeIgraca1.SetActive (true);
		imeIgraca2.SetActive (true);
		imeIgraca3.SetActive (true);
		imeIgraca4.SetActive (true);
		textImeIgraca1.SetActive (true);
		textImeIgraca2.SetActive (true);
		textImeIgraca3.SetActive (true);
		textImeIgraca4.SetActive (true);
		pocni.SetActive (true);
	}

	public void upisiImena(){
		switch (brojIgraca) {
		case 2:
			imenaIgraca [0] = imeIgraca1.GetComponent<InputField> ().text.ToString();
			imenaIgraca [1] = imeIgraca2.GetComponent<InputField> ().text.ToString();
			imeIgraca1.SetActive (false);
			imeIgraca2.SetActive (false);
			textImeIgraca1.SetActive (false);
			textImeIgraca2.SetActive (false);
			break;
		case 3:
			imenaIgraca [0] = imeIgraca1.GetComponent<InputField> ().text.ToString ();
			imenaIgraca [1] = imeIgraca2.GetComponent<InputField> ().text.ToString ();
			imenaIgraca [2] = imeIgraca3.GetComponent<InputField> ().text.ToString ();
			imeIgraca1.SetActive (false);
			imeIgraca2.SetActive (false);
			imeIgraca3.SetActive (false);
			textImeIgraca1.SetActive (false);
			textImeIgraca2.SetActive (false);
			textImeIgraca3.SetActive (false);
			break;
		case 4:
			imenaIgraca [0] = imeIgraca1.GetComponent<InputField> ().text.ToString();
			imenaIgraca [1] = imeIgraca2.GetComponent<InputField> ().text.ToString();
			imenaIgraca [2] = imeIgraca3.GetComponent<InputField> ().text.ToString();
			imenaIgraca [3] = imeIgraca4.GetComponent<InputField> ().text.ToString();
			imeIgraca1.SetActive (false);
			imeIgraca2.SetActive (false);
			imeIgraca3.SetActive (false);
			imeIgraca4.SetActive (false);
			textImeIgraca1.SetActive (false);
			textImeIgraca2.SetActive (false);
			textImeIgraca3.SetActive (false);
			textImeIgraca4.SetActive (false);
			break;
		}

		pocni.SetActive (false);
		quitButton.SetActive (false);
		kreni = true;
		buttonBaci.SetActive (true);
		dropdownListFigure.gameObject.SetActive (true);
	}

	public void quit(){
		Application.Quit ();
	}

	public void reset (){
		trenutniIgrac = 0;
		kreni = false;
		bacanjeRed = true;
		bacanjeRedMax = 0;
		prviIgracIndex = 0;
		pobjednik = null;

		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				igrac [i].figure [j].reset ();
				igrac [i].figure [j].naCilju = false;
				igrac [i].figure [j].uKucici = false;
				igrac [i].figure [j].trenutniIzlazniStup = -1;
			}
		}

		this.Start ();
	}

	//mala sala za kraj :)
	//try it
	void Update (){
		if ((Input.GetKey (KeyCode.RightControl) || Input.GetKey (KeyCode.LeftControl)) && Input.GetKeyDown (KeyCode.W)) {
			if (win.activeSelf) {
				win.SetActive (false);
			} else {
				win.SetActive (true);
			}
		}
	}
}