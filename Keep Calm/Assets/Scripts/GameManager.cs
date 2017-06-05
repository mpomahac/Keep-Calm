using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	//igrac - lista igraca, popunjena iz editora / inspektora
	//put - lista stupova popunjena iz editora / inspektora
	//kamera - referenca na kameru
	//kocka - referenca na kocku
	//dropdownListFigure - referenca na dropdown listu za odabir figure
	//pobjedaText - ispisuje pobjednika
	//trenutniIgracText - ispisuje koji je igrac trenutno na potezu
	public Igrac[] igrac;
	public GameObject[] put;
	public CameraController kamera;
	public Kocka kocka;
	public Dropdown dropdownListFigure;
	public Text pobjedaText;
	public Text trenutniIgracText;

	//trenutniIgrac - index igraca koji je trenutno na potezu, 0 - 3
	//waitKocka - vrijeme cekanja izmedju promjena brzine vrtnje kocke
	//waitKamera - vrijeme cekanja da kamera stigne do sljedece mete
	//waitNakonPomaka - vrijeme cekanja nakon pomaka
	//waitNakonReda -
	//brojMjesta - broj dobiven na kocki
	//gumbZaBacanje - bool vrijednost za prekid cekanja da neko stisne gumb i baci kocku
	//pomakniFiguru - index odabrane figure, dok je -1 ceka se odabir
	//pobjednik - podaci o pobjedniku
	//brojIgraca - 
	//buttonBaci - referenca na gumb za bacanje kocke
	//bacanjeRed -
	//bacanjeRedMax -
	//prviIgracIndex -
	//kreni -
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

	//imenaIgraca - lista imena igraca
	//reference na interface elemente za odabir broja igraca, unos imena igraca, pokretanje igre, reset, vracanje natrag u izborniku, izlazak iz igre
	private string[] imenaIgraca = { "", "", "", "" };
	private GameObject imeIgraca1;
	private GameObject imeIgraca2;
	private GameObject imeIgraca3;
	private GameObject imeIgraca4;
	private GameObject textImeIgraca1;
	private GameObject textImeIgraca2;
	private GameObject textImeIgraca3;
	private GameObject textImeIgraca4;
	private GameObject brojIgraca2Button;
	private GameObject brojIgraca3Button;
	private GameObject brojIgraca4Button;
	private GameObject pocni;
	private GameObject quitButton;
	private GameObject win;
	private GameObject resetButton;
	private GameObject backButton;

	//inicijalizacija
	void Start () {

		//postavljanje referenci na objekte
		win = GameObject.Find ("Win");
		resetButton = GameObject.Find ("ResetButton");
		backButton = GameObject.Find ("BackButton");
		imeIgraca1 = GameObject.Find ("ImeIgraca1");
		imeIgraca2 = GameObject.Find ("ImeIgraca2");
		imeIgraca3 = GameObject.Find ("ImeIgraca3");
		imeIgraca4 = GameObject.Find ("ImeIgraca4");
		textImeIgraca1 = GameObject.Find ("TextIme1");
		textImeIgraca2 = GameObject.Find ("TextIme2");
		textImeIgraca3 = GameObject.Find ("TextIme3");
		textImeIgraca4 = GameObject.Find ("TextIme4");
		brojIgraca2Button = GameObject.Find ("BrojIgraca2");
		brojIgraca3Button = GameObject.Find ("BrojIgraca3");
		brojIgraca4Button=GameObject.Find("BrojIgraca4");
		pocni = GameObject.Find ("Pocni");
		quitButton = GameObject.Find ("QuitButton");
		buttonBaci = GameObject.Find ("Button");

		//gasenje trenutno nepotrebnih objekata
		win.SetActive (false);
		resetButton.SetActive (false);
		backButton.SetActive (false);
		imeIgraca1.SetActive (false);
		imeIgraca2.SetActive (false);
		imeIgraca3.SetActive (false);
		imeIgraca4.SetActive (false);
		textImeIgraca1.SetActive (false);
		textImeIgraca2.SetActive (false);
		textImeIgraca3.SetActive (false);
		textImeIgraca4.SetActive (false);
		pocni.SetActive (false);
		buttonBaci.SetActive (false);
		dropdownListFigure.gameObject.SetActive (false);

		//postavljanje pocetnih pozicija figura
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				igrac [i].figure [j].startPozicija = igrac [i].figure [j].transform.position;
				igrac [i].figure [j].zeljenaPozicija = igrac [i].figure [j].transform.position;
			}
		}

		//postavljanje osnovnih stupova (ulazni, izlazni, broj slobodnih u cilju)
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

		//postavljanje kamere na centar
		kamera.changeTarget (GameObject.Find ("Center"));

		//pokretanje igre
		StartCoroutine (gameloop ());
	}

	//main funkcija
	private IEnumerator gameloop(){

		//cekaj odabir broja igraca i pritisak na Kreni!
		//nakon pokretanja prvog kruga ovaj dio se preskače
		while (brojIgraca == 0 || !kreni) {
			yield return null;
		}

		//pokreni krug i cekaj njegov kraj
		yield return StartCoroutine (turn ());

		//pogledaj da li postoji pobjednik, ako da ipisi ga, ako ne pokreni novi krug
		if ((pobjednik = pobjeda ()) != null) {
			pobjedaText.text = "Pobijedio je " + pobjednik.ime;
			quitButton.SetActive (true);
			resetButton.SetActive (true);
		} else {
			StartCoroutine (gameloop ());
		}
	}

	//
	private IEnumerator turn(){
		//promjena boje imena igraca u ispisu ovisno o boji igraca
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

		//postavljanje kamere iznad startne pozicije igraca
		kamera.changeTarget(GameObject.Find("FokusKamere"+igrac[trenutniIgrac].ime));

		//cekaj da se stisne gumb za bacanje
		while (!gumbZaBacanje) {
			yield return null;
		}

		//
		if (!bacanjeRed && igrac [trenutniIgrac].prvoBacanje) {
			igrac [trenutniIgrac].brojBacanja = 3;
			igrac [trenutniIgrac].prvoBacanje = false;
		}

		//pokreni bacanje kocke i cekaj da zavrsi
		yield return StartCoroutine (bacanjeKocke ());

		//
		if (!bacanjeRed) {
			//resetiraj listu figura koje se mogu pomaknuti
			dropdownListFigure.ClearOptions ();
			List<string> opcije = new List<string> ();
			//standardna opcija
			opcije.Add ("Odaberi figuru");
			//pregledaj koje figure se smiju pomaknuti i stavi ih u listu
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
				//postavljanje kamere na figuru
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
				//ako moze uci u kucicu, pomakni ju i provjeri da li je stigla u najdublje polje u kucici, ako je smanji index zadnjeg slobodnog polja u kucici za tog igraca
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
				//pomakni figuru i promjeni podatke o njezino trenutnom polozaju
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

				//provjeri da li je neka figura vec na tom polju, ako je resetiraj ju
				sudar (igrac [trenutniIgrac].figure [pomakniFiguru]);

				//
				igrac [trenutniIgrac].figure [pomakniFiguru].centriranje (put [igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup].transform);
				yield return StartCoroutine (cekanjeReda ());
			}
		}



		//resetiranje podataka prije ulaska u novi krug
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

		//kraj funkcije, povratak u gameloop
	}

	//bacanje kocke
	private IEnumerator bacanjeKocke(){
		//pomakni kameru na kocku, odradi random, zavrti kocku
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

		//posalji kocki podatak o dobivenom broju
		kocka.naBroj (brojMjesta);
		kocka.rotiraj (0);
		yield return waitNakonPomaka;

		//postavljanje kamere iznad startne pozicije igraca
		kamera.changeTarget(GameObject.Find("FokusKamere"+igrac[trenutniIgrac].ime));
	}

	//
	private IEnumerator cekanjeKamere(){
		yield return waitKamera;
	}

	//
	private IEnumerator cekanjePomaka(){
		yield return waitNakonPomaka;
	}

	//
	private IEnumerator cekanjeReda(){
		yield return waitNakonReda;
	}

	//detektira pritisak gumba za bacanje kocke
	public void baci(){
		gumbZaBacanje = true;
	}

	//detektira koja figura je odabrana za pomicanje
	public void odaberiFiguru(){
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
	//dozvoljeno je ako:
	//					figura je na startu i dobiven je 6, a niti jedna druga igraceva figura nije na ulaznom stupu
	//					figura nije na startu niti cilju i niti jedna druga igraceva figura nije na zadnjem stupu na koji bi figura pomakom dosla
	//					figura nije na startu niti cilju i pomakom nece preskociti zadnje slobodno polje u kucici
	private bool eligable(Figura figura, int index){
		if (figura.naStartu && brojMjesta == 6 && provjeraPozicije (figura, index))
			return true;
		else if (!figura.naStartu && !figura.naCilju && (figura.trenutniStup + brojMjesta <= igrac [trenutniIgrac].izlazniStup || figura.trenutniStup > igrac [trenutniIgrac].izlazniStup) && provjeraPozicije (figura, index))
			return true;
		else if (!figura.naStartu && !figura.naCilju && figura.trenutniStup + brojMjesta - igrac [trenutniIgrac].izlazniStup <= igrac [trenutniIgrac].zadnjiSlobodanIzlazni + 1)
			return true;
		else if (figura.uKucici && brojMjesta+figura.trenutniIzlazniStup<=igrac[trenutniIgrac].zadnjiSlobodanIzlazni && provjeraKucice(figura))
			return true;
		else
			return false;
	}

	//provjerava da li igrac vec ima figuru na stupu na koji bi figura dosla pomakom
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

	//provjerava da li igrac vec ima figuru na polju u kucici na koje bi figura dosla pomakom
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

	//pregledava da li neki od igraca ima ispunjen uvjet za pobjedu, broj slobodnih polja u cilju manji od 0 preko indeksa zadnjeg slobodnog
	private Igrac pobjeda(){
		for (int i = 0; i < 4; i++) {
			if (igrac [i].zadnjiSlobodanIzlazni < 0)
				return igrac [i];
		}
		return null;
	}

	//odabire 2 igraca
	public void brojIgraca2(){
		brojIgraca = 2;
		brojIgraca2Button.SetActive (false);
		brojIgraca3Button.SetActive (false);
		brojIgraca4Button.SetActive (false);

		imeIgraca1.SetActive (true);
		imeIgraca2.SetActive (true);
		textImeIgraca1.SetActive (true);
		textImeIgraca2.SetActive (true);
		pocni.SetActive (true);
		backButton.SetActive (true);
	}

	//odabire 3 igraca
	public void brojIgraca3(){
		brojIgraca = 3;
		brojIgraca2Button.SetActive (false);
		brojIgraca3Button.SetActive (false);
		brojIgraca4Button.SetActive (false);

		imeIgraca1.SetActive (true);
		imeIgraca2.SetActive (true);
		imeIgraca3.SetActive (true);
		textImeIgraca1.SetActive (true);
		textImeIgraca2.SetActive (true);
		textImeIgraca3.SetActive (true);
		pocni.SetActive (true);
		backButton.SetActive (true);
	}

	//odabire 4 igraca
	public void brojIgraca4(){
		brojIgraca = 4;
		brojIgraca2Button.SetActive (false);
		brojIgraca3Button.SetActive (false);
		brojIgraca4Button.SetActive (false);

		imeIgraca1.SetActive (true);
		imeIgraca2.SetActive (true);
		imeIgraca3.SetActive (true);
		imeIgraca4.SetActive (true);
		textImeIgraca1.SetActive (true);
		textImeIgraca2.SetActive (true);
		textImeIgraca3.SetActive (true);
		textImeIgraca4.SetActive (true);
		pocni.SetActive (true);
		backButton.SetActive (true);
	}

	//upisuje imena igraca u listu i započinje igru
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
		backButton.SetActive (false);
		kreni = true;
		buttonBaci.SetActive (true);
		dropdownListFigure.gameObject.SetActive (true);
	}

	//gasi aplikaciju
	public void quit(){
		Application.Quit ();
	}

	//resetira igru vracanjem u glavni izbornik
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
			igrac [i].brojBacanja = 1;
			igrac [i].prvoBacanje = true;
		}

		win.SetActive (true);
		resetButton.SetActive (true);
		backButton.SetActive (true);
		imeIgraca1.SetActive (true);
		imeIgraca2.SetActive (true);
		imeIgraca3.SetActive (true);
		imeIgraca4.SetActive (true);
		textImeIgraca1.SetActive (true);
		textImeIgraca2.SetActive (true);
		textImeIgraca3.SetActive (true);
		textImeIgraca4.SetActive (true);
		brojIgraca2Button.SetActive (true);
		brojIgraca3Button.SetActive (true);
		brojIgraca4Button.SetActive (true);
		pocni.SetActive (true);
		quitButton.SetActive (true);
		buttonBaci.SetActive (true);

		this.StopAllCoroutines ();

		this.Start ();
	}

	//vraca na prethodni dio izbornika
	public void back (){
		brojIgraca2Button.SetActive (true);
		brojIgraca3Button.SetActive (true);
		brojIgraca4Button.SetActive (true);

		if(imeIgraca1.activeSelf)imeIgraca1.SetActive (false);
		if(imeIgraca2.activeSelf)imeIgraca2.SetActive (false);
		if(imeIgraca3.activeSelf)imeIgraca3.SetActive (false);
		if(imeIgraca4.activeSelf)imeIgraca4.SetActive (false);
		if(textImeIgraca1.activeSelf)textImeIgraca1.SetActive (false);
		if(textImeIgraca2.activeSelf)textImeIgraca2.SetActive (false);
		if(textImeIgraca3.activeSelf)textImeIgraca3.SetActive (false);
		if(textImeIgraca4.activeSelf)textImeIgraca4.SetActive (false);
		if(pocni.activeSelf)pocni.SetActive (false);

		backButton.SetActive (false);
	}

	//naredni dio koda se ne koristi u igri, sluzi iskljucivo za salu
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