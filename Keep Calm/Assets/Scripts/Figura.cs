using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Figura :MonoBehaviour{

	/*------------------------------------------------------Varijable---------------------------------------------------*/
	//oznaka [HideInInspector] označava da se ne pokazuju u Unity inspektoru i ne mogu se tamo postavljati
	[HideInInspector]public Vector3 startPozicija;				//koordinate početne pozicije figure
	[HideInInspector]public Vector3 zeljenaPozicija;			//koordinate sljedeće pozicije figure
	[HideInInspector]public bool naStartu = true;				//označava je li figura na početnoj poziciji
	[HideInInspector]public bool naCilju=false;					//označava je li figura na završnoj poziciji
	[HideInInspector]public bool uKucici = false;				//označava je li figura ušla u kučicu (zadnjih 4 polja na putu figure)
	[HideInInspector]public int trenutniStup = -1;				//oznaka trenutnog stupa figure na putu, -1 ako je na startu ili u kučici
	[HideInInspector]public int trenutniIzlazniStup=-1;			//oznaka trenutnog stupa figure u kučici, -1 ako figura nije u kučici
	[HideInInspector]public float udaljenostX;					//udaljenost između trenutne i željene pozicije figure po X osi, računa se automatski u Update metodi
	[HideInInspector]public float udaljenostZ;					//udaljenost između trenutne i željene pozicije figure po Z osi, računa se automatski u Update metodi

	private Animator anim;										//referenca na Animator komponentu figure
	private bool ulaz = true;									//označava je li sljedeći mogući potez figure ulazak na ploču
	private Quaternion ciljnaRotacija;							//željena rotacija figure
	private Vector3 speed;										//brzina pomicanja figure sa početne pozicije na polje, postavlja i mijenja se automatski

	/*-------------------------------------------------------Metode-----------------------------------------------------*/
	//postavlja referencu na Animator i željenu rotaciju na trenutnu
	void Start(){
		anim = GetComponent<Animator> ();
		ciljnaRotacija = transform.rotation;
	}

	//prima novu poziciju i oznaku izlazi li figura na ploču te ih postavlja u odgovarajuće varijable
	public void move(Transform novaPozicija, bool ulaz){
		zeljenaPozicija=new Vector3 (novaPozicija.position.x, 6.01f, novaPozicija.position.z);
		this.ulaz = ulaz;
	}

	//funkcija se poziva svaki frame i izvršava kontrolu kretanja i rotacije figure
	void Update () {
		//petlja za određivanje vrste kretanja figure ovisno o varijabli ulaz
		//figura "leti" do početne pozicije ako tek ulazi na polje, a kreće se pomoću animacije ako već je na polju
		if (ulaz) {
			this.transform.position = Vector3.SmoothDamp (gameObject.transform.position, zeljenaPozicija, ref speed, 1f);
		}
		else {
			
			//računanje udaljenosti po osima X i Z
			udaljenostX = gameObject.transform.position.x - zeljenaPozicija.x;
			udaljenostZ = gameObject.transform.position.z - zeljenaPozicija.z;

			//petlja koja omogućava izvršavanje animacije ako je figura izvan određenih udaljenosti od željene pozicije po osima X i Z
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

		//glatka rotacija figure
		transform.rotation = Quaternion.Slerp (transform.rotation,ciljnaRotacija,0.15f);
	}

	//vraća figuru na početnu poziciju i resetira vrijednosti potrebnih varijabli na početne vrijednosti
	public void reset(){
		transform.position = startPozicija;
		zeljenaPozicija = startPozicija;
		transform.eulerAngles = new Vector3 (0, 0, 0);
		ciljnaRotacija = transform.rotation;
		naStartu = true;
		trenutniStup = -1;
	}

	//postavlja figuru na centar trenutnog stupa
	//koristi se radi ispravljanja nesavršenosti animacije trčanja zbog koje je dolazilo do odstupanja od središta stupa
	public void centriranje(Transform stup){
		if(!ulaz)
			this.transform.SetPositionAndRotation (new Vector3 (stup.position.x, 6.01f, stup.position.z), this.transform.rotation);
	}

	//zaustavlja kretanje i rotira figuru za primljeni broj stupnjeva
	public void rotiraj(float stupnjevi){
		anim.SetBool ("Move", false);
		ciljnaRotacija.eulerAngles = new Vector3 (0, transform.eulerAngles.y+stupnjevi, 0);
	}
}
