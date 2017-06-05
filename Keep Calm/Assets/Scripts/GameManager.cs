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

	//inicijalizacija
	void Start () {

		dropdownListFigure.gameObject.SetActive (false);
		buttonBaci = GameObject.Find ("Button");
		buttonBaci.SetActive (false);

		//postavljanje pocetnih pozicija figura
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				igrac [i].figure [j].startPozicija = igrac [i].figure [j].transform.position;
				igrac [i].figure [j].zeljenaPozicija = igrac [i].figure [j].transform.position;
			}
		}

		//postavljanje osnovnih stupova (ulazni, izlazni, broj slobodnih u cilju) - razlog zašto se tu rikta broj slobodnih: NEŠTO GA JE JEBALO PA JE ON JEBAL MENE SAT VREMENA, ovak radi normalno
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
		waitKocka = new WaitForSeconds (2);
		waitKamera = new WaitForSeconds(1.2f);
		waitNakonPomaka = new WaitForSeconds (2f);
		waitNakonReda = new WaitForSeconds (1.5f);

		kamera.changeTarget (GameObject.Find ("Center"));

		//pokretanje igre
		StartCoroutine (gameloop ());
	}

	//main funkcija
	private IEnumerator gameloop(){

		while (brojIgraca == 0) {
			yield return null;
		}
			
		yield return StartCoroutine (turn ());

		//pogledaj da li postoji pobjednik, ako da ipisi ga, ako ne idemo dalje
		if ((pobjednik = pobjeda ()) != null) {
			pobjedaText.text = "Pobijedio je " + pobjednik.ime;
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
		trenutniIgracText.text ="Trenutno je na potezu <color=#"+colorString+">"+ igrac [trenutniIgrac].ime+"</color>";

		kamera.changeTarget(GameObject.Find("FokusKamere"+igrac[trenutniIgrac].ime));

		//cekaj da se stisne gumb za bacanje
		while (!gumbZaBacanje) {
			yield return null;
		}

		//pokreni bacanje "kocke" i cekaj da zavrsi
		yield return StartCoroutine (bacanjeKocke ());

		//resetiraj listu figura koje se mogu pomaknut
		dropdownListFigure.ClearOptions ();
		List<string> opcije= new List<string>();
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
		while (pomakniFiguru==-1) {
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
				igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj(rotacija);
				igrac [trenutniIgrac].figure [pomakniFiguru].naStartu = false;
				yield return StartCoroutine (cekanjePomaka ());
			}
			//ako moze uc u cilj, teleportira ju tamo, da se napravit da ide korak po korak ako ce radit probleme
			else if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup + brojMjesta - igrac [trenutniIgrac].izlazniStup == igrac [trenutniIgrac].zadnjiSlobodanIzlazni+1 &&
				igrac[trenutniIgrac].figure[pomakniFiguru].trenutniStup<=igrac[trenutniIgrac].izlazniStup) {

				igrac [trenutniIgrac].figure [pomakniFiguru].move (igrac [trenutniIgrac].izlazniStupovi [igrac [trenutniIgrac].zadnjiSlobodanIzlazni].transform, false);
				igrac [trenutniIgrac].zadnjiSlobodanIzlazni--;
				igrac [trenutniIgrac].figure [pomakniFiguru].naCilju = true;
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
						}
						else if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == 9 + i * 10) {
							igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
							yield return StartCoroutine (cekanjeReda ());
						}
						else if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == 5 + i * 10) {
							igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (-90.0f);
							yield return StartCoroutine (cekanjeReda ());
						}
					}
					if (igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup == igrac [trenutniIgrac].izlazniStup) {
						igrac [trenutniIgrac].figure [pomakniFiguru].rotiraj (90.0f);
						yield return StartCoroutine (cekanjeReda ());
					}
					igrac [trenutniIgrac].figure [pomakniFiguru].move (put [igrac [trenutniIgrac].figure[pomakniFiguru].trenutniStup].transform, false);
					yield return StartCoroutine (cekanjePomaka ());
				}
			}

			//vidi jel doslo do sudara
			sudar (igrac [trenutniIgrac].figure [pomakniFiguru]);

			igrac [trenutniIgrac].figure [pomakniFiguru].centriranje (put [igrac [trenutniIgrac].figure [pomakniFiguru].trenutniStup].transform);

			yield return StartCoroutine (cekanjeReda ());
		}



		//resetira listu figura, resetira index odabrane figure, pomice kameru na centar, prebacuje na sljedeceg igraca, resetira gumb za bacanje
		dropdownListFigure.value = 0;
		dropdownListFigure.ClearOptions();
		pomakniFiguru = -1;
		kamera.changeTarget(GameObject.Find("FokusKamere"+igrac[trenutniIgrac].ime));
		yield return StartCoroutine (cekanjeKamere ());
		if(brojMjesta != 6) trenutniIgrac++;
		if (trenutniIgrac >= brojIgraca)
			trenutniIgrac = 0;
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
		kocka.rotiraj (0.5f);
		yield return waitKocka;
		kocka.rotiraj (0.25f);
		yield return waitKocka;
		kocka.naBroj (brojMjesta);
		kocka.rotiraj (0);
		yield return waitKocka;

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
		else if (!figura.naStartu && !figura.naCilju && (figura.trenutniStup + brojMjesta <= igrac [trenutniIgrac].izlazniStup || figura.trenutniStup > igrac [trenutniIgrac].izlazniStup))
			return true;
		else if (!figura.naStartu && !figura.naCilju && figura.trenutniStup + brojMjesta - igrac [trenutniIgrac].izlazniStup == igrac [trenutniIgrac].zadnjiSlobodanIzlazni + 1)
			return true;
		else if (!figura.naStartu && provjeraPozicije (figura, index))
			return true;
		else
			return false;
	}

	private bool provjeraPozicije(Figura figura, int index){
		for (int i = 0; i < 4; i++) {
			if (i != index) {
				if (!figura.naStartu && igrac [trenutniIgrac].figure [i].trenutniStup <= figura.trenutniStup + brojMjesta) {
					return false;
				} else if (figura.naStartu && igrac [trenutniIgrac].figure [i].trenutniStup == igrac [trenutniIgrac].ulazniStup) {
					return false;
				}
			}
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
		buttonBaci.SetActive (true);
		dropdownListFigure.gameObject.SetActive (true);
	}

	public void brojIgraca3(){
		brojIgraca = 3;
		GameObject.Find ("BrojIgraca2").SetActive (false);
		GameObject.Find ("BrojIgraca3").SetActive (false);
		GameObject.Find ("BrojIgraca4").SetActive (false);
		buttonBaci.SetActive (true);
		dropdownListFigure.gameObject.SetActive (true);
	}

	public void brojIgraca4(){
		brojIgraca = 4;
		GameObject.Find ("BrojIgraca2").SetActive (false);
		GameObject.Find ("BrojIgraca3").SetActive (false);
		GameObject.Find ("BrojIgraca4").SetActive (false);
		buttonBaci.SetActive (true);
		dropdownListFigure.gameObject.SetActive (true);
	}
}
