using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	/*---------------------------------------------Varijable--------------------------------------------------*/
	public Igrac[] igrac;								//lista igrača
	public GameObject[] put;							//lista stupova koji tvore put za figure
	public CameraController kamera;						//referenca na kameru
	public Kocka kocka;									//referenca na kocku
	public Dropdown dropdownListFigure;					//referenca na dropdown listu mogućih figura za pomicanje
	public Text pobjedaText;							//referenca na tekstualno polje u kojem se ispisuje poruka za pobjedu
	public Text trenutniIgracText;						//referenca na tekstualno polje u kojem se ispisuje koji igrač je trenutno na redu

	private int trenutniIgrac = 0;						//index igrača koji je trenutno na potezu
	private WaitForSeconds waitKocka;					//vrijeme čekanja između promjena brzine rotacije kocke
	private WaitForSeconds waitKamera;					//vrijeme čekanja da kamera stigne do sljedeće mete
	private WaitForSeconds waitNakonPomaka;				//vrijeme čekanja nakon pomaka
	private WaitForSeconds waitNakonReda;				//vrijeme čekanja nakon što je igraču red završio
	private int brojMjesta;								//broj dobiven na kocki
	private bool gumbZaBacanje = false;					//prekidač čekanja da netko stisne gumb i baci kocku
	private int pomakniFiguru = -1;						//index odabrane figure za kretanje, -1 ako se čeka odabir
	private Igrac pobjednik=null;						//referenca na pobjednika
	private int brojIgraca=0;							//broj igrača koji igraju igru
	private GameObject buttonBaci;						//referenca na gumb za bacanje kocke
	private bool bacanjeRed = true;						//oznaka za bacanje kocke za određivanje igrača koji prvi kreće
	private int bacanjeRedMax = 0;						//najveća vrijednost dobivena u krugu za određivanje prvog igrača
	private int prviIgracIndex = 0;						//index igrača koji je dobio najveću vrijednost u krugu određivanja prvog igrača
	private bool kreni=false;							//prekidač za pokretanje igre
	private string[] imenaIgraca = { "", "", "", "" };	//lista imena igrača

	//reference na input polja za imena igrača
	private GameObject imeIgraca1;
	private GameObject imeIgraca2;
	private GameObject imeIgraca3;
	private GameObject imeIgraca4;

	//reference na text polja sa oznakama igrača
	private GameObject textImeIgraca1;
	private GameObject textImeIgraca2;
	private GameObject textImeIgraca3;
	private GameObject textImeIgraca4;

	//reference na gumbe koji određuju broj igrača
	private GameObject brojIgraca2Button;
	private GameObject brojIgraca3Button;
	private GameObject brojIgraca4Button;

	//reference na gumbe za početak igre i izlazak iz igre
	private GameObject pocni;
	private GameObject quitButton;

	//referenca na šaljivu sliku
	private GameObject win;

	//reference na gumbe za resetiranje igre i povratak na prijašnji izbornik
	private GameObject resetButton;
	private GameObject backButton;

	//reference na tekstualna polja za naslov i imena autora
	private GameObject title1;
	private GameObject title2;
	private GameObject title3;
	private GameObject title4;
	private GameObject autori;

	/*----------------------------------------------Metode----------------------------------------*/
	//inicijalizacija igre
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
		title1 = GameObject.Find ("Title1");
		title2 = GameObject.Find ("Title2");
		title3 = GameObject.Find ("Title3");
		title4 = GameObject.Find ("Title4");
		autori = GameObject.Find ("Autori");

		//gašenje trenutno nepotrebnih objekata
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

		//postavljanje početnih pozicija figura
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

		//postavljanje vremena čekanja
		waitKocka = new WaitForSeconds (0.5f);
		waitKamera = new WaitForSeconds(1.2f);
		waitNakonPomaka = new WaitForSeconds (2f);
		waitNakonReda = new WaitForSeconds (1.2f);

		//postavljanje kamere na centar
		kamera.changeTarget (GameObject.Find ("Center"));

		//pokretanje igre
		StartCoroutine (gameloop ());
	}

	//upravljanje igrom
	private IEnumerator gameloop(){

		//čekaj odabir broja igraca i pritisak na Kreni!
		//nakon pokretanja prvog kruga ovaj dio se preskače
		while (brojIgraca == 0 || !kreni) {
			yield return null;
		}

		//pokreni krug i čekaj njegov kraj
		yield return StartCoroutine (turn ());

		//provjerava postoji li pobjednik i ispisuje ga ako postoji, inače nastavlja igru
		if ((pobjednik = pobjeda ()) != null) {
			pobjedaText.text = "Pobijedio je " + pobjednik.ime;
			quitButton.SetActive (true);
			resetButton.SetActive (true);
		} else {
			StartCoroutine (gameloop ());
		}
	}

	//upravlja krugom
	private IEnumerator turn(){
		//promjena boje igračevog imena u ispisu ovisno o boji igračevih figura
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

		//ispis trenutnog igrača na vrhu ekrana
		trenutniIgracText.text ="Trenutno je na potezu <color=#"+colorString+">"+ imenaIgraca[trenutniIgrac]+"</color>";

		//postavljanje kamere iznad startne pozicije igraca
		kamera.changeTarget(GameObject.Find("FokusKamere"+igrac[trenutniIgrac].ime));

		//čekanje pritiska na gumb za bacanje
		while (!gumbZaBacanje) {
			yield return null;
		}

		//provjerava je li igračev trenutni krug ujedno i njegov prvi krug te postavlja broj bacanja na 3 ako je
		if (!bacanjeRed && igrac [trenutniIgrac].prvoBacanje) {
			igrac [trenutniIgrac].brojBacanja = 3;
			igrac [trenutniIgrac].prvoBacanje = false;
		}

		//pokreće bacanje kocke i čeka da završi
		yield return StartCoroutine (bacanjeKocke ());

		//ako nije bacanje za određivanje reda, omogućuje se pokretanje figurama koje zadovoljavaju pravila
		if (!bacanjeRed) {
			//resetiranje liste figura koje se mogu pomaknuti
			dropdownListFigure.ClearOptions ();
			List<string> opcije = new List<string> ();

			//postavljanje standardne opcije
			opcije.Add ("Odaberi figuru");

			//određivanje koje figure se smiju pomaknuti i stavljanje tih figura u listu
			if (eligable (igrac [trenutniIgrac].figure [0], 0))
				opcije.Add (igrac [trenutniIgrac].figure [0].gameObject.name);
			if (eligable (igrac [trenutniIgrac].figure [1], 1))
				opcije.Add (igrac [trenutniIgrac].figure [1].gameObject.name);
			if (eligable (igrac [trenutniIgrac].figure [2], 2))
				opcije.Add (igrac [trenutniIgrac].figure [2].gameObject.name);
			if (eligable (igrac [trenutniIgrac].figure [3], 3))
				opcije.Add (igrac [trenutniIgrac].figure [3].gameObject.name);
			dropdownListFigure.AddOptions (opcije);

			//čekanje dok se ne odabere figura, ako nijednoj nije dozvoljeno pomicanje nastavlja se dalje
			while (pomakniFiguru == -1) {
				if (dropdownListFigure.options.Count == 1)
					break;
				yield return null;
			}

			//ako je odabrana figura pomakni, onda se ona pomiče, inače se kod preskače
			if (pomakniFiguru != -1) {
				
				//postavljanje kamere na figuru
				kamera.changeTarget (igrac [trenutniIgrac].figure [pomakniFiguru].gameObject);
				yield return StartCoroutine (cekanjeKamere ());

				//ako je odabrana figura na startu, postavlja se na ulazni stup
				if (igrac [trenutniIgrac].figure [pomakniFiguru].naStartu) {
					float rotacija = (float)((trenutniIgrac - 1) * 90);
					igrac [trenutniIgrac].figure [pomakniFiguru].move (put [igrac [trenutniIgrac].ulazniStup].transform, true);
					igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup = igrac [trenutniIgrac].ulazniStup;
					igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (rotacija);
					igrac [trenutniIgrac].figure [pomakniFiguru].naStartu = false;
					yield return StartCoroutine (cekanjePomaka ());
				}

				//ako figura može ući u kučicu, pomiče se i provjerava je li stigla u najdublje polje u kučici te smanjuje index zadnjeg slobodnog polja u kučici za tog igrača ako je
				else if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup + brojMjesta - igrac [trenutniIgrac].izlazniStup == igrac [trenutniIgrac].zadnjiSlobodanIzlazni + 1 &&
				         igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup <= igrac [trenutniIgrac].izlazniStup) {

					//broj mjesta za kretanje u kučici
					int uKucici=brojMjesta;

					//pomicanje figure dok ne dođe do zadnjeg stupa prije kučice
					for (int x = 1; x <= brojMjesta; x++) {

						//rotiranje figure ako je na specifičnom stupu
						for (int i = 0; i < 4; i++) {
							if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == 1 + i * 10) {
								igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
								yield return StartCoroutine (cekanjeReda ());
							} else if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == 5 + i * 10) {
								igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (-90.0f);
								yield return StartCoroutine (cekanjeReda ());
							}
						}

						//prekid pomicanja figure i rotiranje figure prema kučici ako je na zadnjem stupu prije kučice
						if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == igrac [trenutniIgrac].izlazniStup) {
							igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
							break;
						}

						//pomicanje figure za jedno mjesto naprijed
						igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup += 1;
						igrac [trenutniIgrac].figure [pomakniFiguru].move (put [igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup].transform, false);
						yield return StartCoroutine (cekanjePomaka ());

						//smanjivanje preostalog broja pokreta prema naprijed
						uKucici--;
					}

					//pomicanje figure unutar kučice za preostali broj mjesta
					for (int i = 0; i < uKucici; i++) {
						igrac [trenutniIgrac].figure [pomakniFiguru].move (igrac[trenutniIgrac].izlazniStupovi[i].transform, false);
						igrac [trenutniIgrac].figure [pomakniFiguru].trenutniIzlazniStup++;
					}

					//postavljanje indexa trenutnog stupa na -1 i označavanje figure da se nalazi u kučici
					igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup = -1;
					igrac [trenutniIgrac].figure [pomakniFiguru].uKucici = true;

					//provjera je li figura na najdubljem slobodnom stupu u kučici i označavanje figure da je na cilju ako je
					if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniIzlazniStup == igrac [trenutniIgrac].zadnjiSlobodanIzlazni) {
						igrac [trenutniIgrac].zadnjiSlobodanIzlazni--;
						igrac [trenutniIgrac].figure [pomakniFiguru].naCilju = true;
					}
				}

				//pomicanje figure i promjena podataka o njezinom trenutnom položaju
				else {

					//pomicanje figure za dobiveni broj i rotiranje figure po potrebi
					for (int x = 1; x <= brojMjesta; x++) {

						//postavljanje broja stupa figure na sljedeći stup
						igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup += 1;
						if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup >= 40)
							igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup = 0;

						//rotiranje figure ako je na specifičnom stupu
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

						//rotiranje figure ako je na zadnjem stupu prije kučice
						if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == igrac [trenutniIgrac].izlazniStup) {
							igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
							yield return StartCoroutine (cekanjeReda ());
						}

						//pomicanje figure za 1 mjesto naprijed
						igrac [trenutniIgrac].figure [pomakniFiguru].move (put [igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup].transform, false);
						yield return StartCoroutine (cekanjePomaka ());
					}
				}

				//provjera je li neka figura već na polju gdje je figura stala, ako je resetira ju
				sudar (igrac [trenutniIgrac].figure [pomakniFiguru]);

				//centriranje figure na sredinu stupa
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
		
		//pomicanje kameru na kocku i dobivanje nasumičnog broja
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

		//slanje podatka o dobivenom broju kocki
		kocka.naBroj (brojMjesta);
		kocka.rotiraj (0);
		yield return waitNakonPomaka;

		//postavljanje kamere iznad startne pozicije igraca
		kamera.changeTarget(GameObject.Find("FokusKamere"+igrac[trenutniIgrac].ime));
	}

	//izvršavanje čekanja kamere da se pozicionira
	private IEnumerator cekanjeKamere(){
		yield return waitKamera;
	}

	//izvršavanje čekanja kraja pomaka
	private IEnumerator cekanjePomaka(){
		yield return waitNakonPomaka;
	}

	//iyvršavanje čekanja nakon igračevog reda
	private IEnumerator cekanjeReda(){
		yield return waitNakonReda;
	}

	//detektiviranje mogućnosti pritiska na gumb za bacanje kocke
	public void baci(){
		gumbZaBacanje = true;
	}

	//detektiranje koja figura je odabrana za pomicanje iz dropdown liste
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

	//provjera je li figuri dozvoljeno kretanje
	//dozvoljeno je ako:
	//					figura je na startu i dobiven je 6, a niti jedna druga igračeva figura nije na ulaznom stupu
	//					figura nije na startu niti cilju i niti jedna druga igračeva figura nije na zadnjem stupu na koji bi figura pomakom dosla
	//					figura nije na startu niti cilju i pomakom neće preskociti zadnje slobodno polje u kučici
	//					figura je u kučici, neće preskočiti zadnje slobodno mjesto u kučici niti neće skočiti na polje u kučici koje je već zauzeto
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

	//provjera ima li igrač već neku drugu figuru na stupu na koji bi figura dosla pomakom
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

	//provjera ima li igrač već neku drugu figuru na polju u kucici na koje bi figura dosla pomakom
	private bool provjeraKucice(Figura figura){
		for (int i = 0; i < 4; i++) {
			if (figura.trenutniIzlazniStup + brojMjesta == igrac [trenutniIgrac].figure [i].trenutniIzlazniStup)
				return false;
		}
		return true;
	}

	//resetira figuru koja je na stupu ako je došla neka nova na taj stup
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

	//provjera zadovoljava li jedan od igrača uvjet za pobjedu
	private Igrac pobjeda(){
		for (int i = 0; i < 4; i++) {
			if (igrac [i].zadnjiSlobodanIzlazni < 0)
				return igrac [i];
		}
		return null;
	}

	//odabir 2 igrača i aktiviranje/deaktiviranje objekata ovisno o potrebi
	public void brojIgraca2(){
		brojIgraca = 2;
		brojIgraca2Button.SetActive (false);
		brojIgraca3Button.SetActive (false);
		brojIgraca4Button.SetActive (false);
		title1.SetActive (false);
		title2.SetActive (false);
		title3.SetActive (false);
		title4.SetActive (false);
		autori.SetActive (false);

		imeIgraca1.SetActive (true);
		imeIgraca2.SetActive (true);
		textImeIgraca1.SetActive (true);
		textImeIgraca2.SetActive (true);
		pocni.SetActive (true);
		backButton.SetActive (true);
	}

	//odabir 3 igrača i aktiviranje/deaktiviranje objekata ovisno o potrebi
	public void brojIgraca3(){
		brojIgraca = 3;
		brojIgraca2Button.SetActive (false);
		brojIgraca3Button.SetActive (false);
		brojIgraca4Button.SetActive (false);
		title1.SetActive (false);
		title2.SetActive (false);
		title3.SetActive (false);
		title4.SetActive (false);
		autori.SetActive (false);

		imeIgraca1.SetActive (true);
		imeIgraca2.SetActive (true);
		imeIgraca3.SetActive (true);
		textImeIgraca1.SetActive (true);
		textImeIgraca2.SetActive (true);
		textImeIgraca3.SetActive (true);
		pocni.SetActive (true);
		backButton.SetActive (true);
	}

	//odabir 4 igrača i aktiviranje/deaktiviranje objekata ovisno o potrebi
	public void brojIgraca4(){
		brojIgraca = 4;
		brojIgraca2Button.SetActive (false);
		brojIgraca3Button.SetActive (false);
		brojIgraca4Button.SetActive (false);
		title1.SetActive (false);
		title2.SetActive (false);
		title3.SetActive (false);
		title4.SetActive (false);
		autori.SetActive (false);

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

	//upisivanje imena igrača u listu, deaktiviranje svih nepotrebnih objekata i početak igre
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

	//gašenje aplikacije
	public void quit(){
		Application.Quit ();
	}

	//resetiranje igre vraćanjem u glavni izbornik
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
		title1.SetActive (true);
		title2.SetActive (true);
		title3.SetActive (true);
		title4.SetActive (true);
		autori.SetActive (true);

		this.StopAllCoroutines ();

		this.Start ();
	}

	//vraćanje na prethodni dio izbornika
	public void back (){
		brojIgraca2Button.SetActive (true);
		brojIgraca3Button.SetActive (true);
		brojIgraca4Button.SetActive (true);
		title1.SetActive (true);
		title2.SetActive (true);
		title3.SetActive (true);
		title4.SetActive (true);
		autori.SetActive (true);

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

	//poziva se svaki frame i provjerava je unose s tipkovnice
	//P.S.: CTRL+W poziva šaljivu sliku :)
	void Update (){
		if ((Input.GetKey (KeyCode.RightControl) || Input.GetKey (KeyCode.LeftControl)) && Input.GetKeyDown (KeyCode.W)) {
			if (win.activeSelf) {
				win.SetActive (false);
			} else {
				win.SetActive (true);
			}
		}

		if (Input.GetKey (KeyCode.Escape)) {
			if (quitButton.activeSelf)
				quitButton.SetActive (false);
			else
				quitButton.SetActive (true);
		}
	}
}