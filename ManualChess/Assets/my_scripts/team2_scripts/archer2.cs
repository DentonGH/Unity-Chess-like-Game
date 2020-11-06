using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class archer2 : MonoBehaviour {


	Stats stats;
	bool is_other_jobs_done;
	float tile = 1.24f;
	float[] tileX = new float[8];
	float[] tileZ = new float[8];
	float[,] matrix = new float[8, 8];
	//GameObject[] Terrain;
	GameObject[] Sunak;
	GameObject[] Team2Units;
	GameObject[] Team1Units;
	Vector3 RealDestination;

	void Start () 
	{
		stats = this.GetComponent<Stats> ();
	}

	void Update () 
	{
		is_other_jobs_done = GameObject.Find ("Team1").GetComponent<Team1Controller> ().IsJobsDone();
	}

	[Task]
	public void TileScoreMaker() //method type'ın ne olacağını düşünmedim, void olduğuna bakma. Canın fazla ve az olmasına göre, karakterinin gücüne bakarak tile puanlaması çıkaracak.
	{
		tileX [0] = -4.619f;
		tileZ [0] = 4.34f;
		for (int a = 0; a < (tileX.Length-1); a++) { //x koordinatı.
			tileX [a + 1] = (tileX [a] + tile);
		}
		for (int a = 0; a < tileZ.Length-1; a++) { //z koordinatı
			tileZ [a + 1] = (tileZ [a] - tile);
		}

		for (int m = 0; m < tileX.Length; m++) {
			for (int k = 0; k < tileZ.Length; k++) {
				matrix [m, k] = MatrixScore (tileX[m], tileZ[k]);
			}
		}
		Task.current.Succeed ();
	}

	public bool ThereIsNoOneAtThisPoint(float m, float k)
	{
		int sayac1 = 0;
		int sayac2 = 0;
		Vector3 currentVector = new Vector3 (m, 0.5675216f, k);
		//Vector3 Destination = new Vector3 (tileX [(int) m], 0.5675216f, tileZ [(int) k]); 

		for (int r = 0; r < Team1Units.Length; r++){
			if ((currentVector.x < Team1Units [r].transform.position.x - 0.5f || currentVector.x > Team1Units [r].transform.position.x + 0.5f) || (currentVector.z < Team1Units [r].transform.position.z - 0.5f || currentVector.z > Team1Units [r].transform.position.z + 0.5f)) {
				sayac1++;
			}
		}
		for (int r = 0; r < Team2Units.Length; r++){
			if ((currentVector.x < Team2Units [r].transform.position.x - 0.5f || currentVector.x > Team2Units [r].transform.position.x + 0.5f) || (currentVector.z < Team2Units [r].transform.position.z - 0.5f || currentVector.z > Team2Units [r].transform.position.z + 0.5f)) {
				sayac2++;
			}
		}
		if (sayac1 + sayac2 == Team1Units.Length + Team2Units.Length) {
			return true;
		}
		return false;
	}

	public float MatrixScore(float m, float k) //tek bir noktada skor nasıl hesaplanır bunu yap sadece. Geliştirmeye en açık olan bölüm.
	{
		float score = 0;
		Team1Units = GameObject.FindGameObjectsWithTag("team1"); //updateten çıkarmam gerekebilir.
		Team2Units = GameObject.FindGameObjectsWithTag("team2");
		if (ThereIsNoOneAtThisPoint(m, k)) {


			float atkscore = 0; //saldırabileceği yakınlık
			float defscore = 0; //saldırı yemeyeceği uzaklık
			float sunakscore = 0;
			//float terrainscore = 0;
			Vector3 currentVector = new Vector3 (m, 0.5675216f, k);
			//Terrain = GameObject.FindGameObjectsWithTag ("terrain");
			Sunak = GameObject.FindGameObjectsWithTag ("sunak");

			//sunak
			//sunak alma gerekliliği ne kadar diye de eklenebilir sunakscore için
			for (int i = 0; i < Sunak.Length; i++) {
				if (currentVector.x < Sunak [i].transform.position.x + 0.3f && currentVector.x > Sunak [i].transform.position.x - 0.3f) {
					if (currentVector.z < Sunak [i].transform.position.z + 0.3f && currentVector.z > Sunak [i].transform.position.z - 0.3f) {
						sunakscore += 100;
						break;
					}
				}
			}
			//terrain
			//terrain sonra eklenecek.
			//		for (int i = 0; i < Sunak.Length; i++) {
			//			if (currentVector == Terrain [i].transform.position) {
			//				if (Mathf.Clamp (currentVector.x, Sunak [i].transform.position.x - 0.3f, Sunak [i].transform.position.x + 0.3f) == currentVector.x &&
			//					Mathf.Clamp (currentVector.z, Sunak [i].transform.position.z - 0.3f, Terrain [i].transform.position.z + 0.3f) == currentVector.y){
			//					sunakscore += 20;
			//					break;
			//				}	
			//			}
			//		}
			//attack
			for (int i = 0; i < Team1Units.Length; i++) { //if inrange and if the targetlerden birinin canının azlığına oranla artan bir atk değeri ve strenght karşılaştırması. Not:range işini burda karıştırmadım.
				int hpfarki = stats.hp - Team1Units [i].GetComponent<Stats> ().hp;
				atkscore += hpfarki;
				int strfarki = stats.strength - Team1Units [i].GetComponent<Stats> ().strength;
				atkscore += strfarki;
				float uzaklik = Vector3.Distance (currentVector, Team1Units [i].transform.position);
				int tileUzaklik = (int)(uzaklik / tile);
				atkscore /= tileUzaklik/2;
			}
			//defense
			for (int i = 0; i < Team1Units.Length; i++) {
				int hpfarki = stats.hp - Team1Units [i].GetComponent<Stats> ().hp;
				defscore -= hpfarki;
				int strfarki = stats.strength - Team1Units [i].GetComponent<Stats> ().strength;
				defscore -= strfarki;
				float uzaklik = Vector3.Distance (currentVector, Team1Units [i].transform.position);
				int tileUzaklik = (int)(uzaklik / tile);
				defscore *= tileUzaklik/2;

			}
			score += atkscore + defscore + sunakscore; //+ terrainscore

			//plane in childı olmalı terrain. plane'e script yaz instantiate terrain on start şeklinde randomlu. kaç tane yapacağın da random olsun.
			return score;
		} else {
			return score;
		}
	}

	[Task]
	public void PickDestination() //pick destination gidilebildiğini de check etmeli. gidilebiliteyi bir metod şeklinde yapabilirim belki. ya da class.
	{
		float max = matrix [0, 0];
		float prevMax;
		int em = 0;
		int key = 0;
		for (int m = 0; m < tileX.Length; m++)
		{
			for (int k = 0; k < tileZ.Length; k++)
			{
				prevMax = max;
				max = Mathf.Max (matrix [m, k], max);
				if(max != prevMax)
				{
					Vector3 Destination = new Vector3 (tileX [m], 0.5675216f, tileZ [k]); 
					if (!ThereIsNoOneAtThisPoint(Destination.x, Destination.z) || Vector3.Distance (Destination, this.transform.position) > stats.moveRange) {
						max = prevMax;
					}
					if(ThereIsNoOneAtThisPoint(Destination.x, Destination.z) && Vector3.Distance (Destination, this.transform.position) < stats.moveRange)
					{
						em = m;
						key = k;
					}
				}
			}
		}
		RealDestination = new Vector3 (tileX [em], 0.5675216f, tileZ [key]);
		if (ThereIsNoOneAtThisPoint(RealDestination.x, RealDestination.z)) { //yukardaki adam gibi çalışmıyor. bunu yazınca üstüste gelme durdu. algoritma yanlış yukardaki sanırım.

			//geliştirdiğin move algoritmasını yapıştıracaksın. Şimdilik ışınlıyorum.
			this.transform.position = RealDestination;
		}
		Task.current.Succeed ();
	}


	[Task]
	public bool MovetoDestination() //check if it has arrived. Çünkü hala karakter yürüyor, onu beklememiz lazım. Arrivedsa aşşağıdaki aksiyona geç. burayı adam et.
	{
		if (true) {
			return true;
		}
		else
		{
			return false;
		}
		//		if (this.transform.position == RealDestination) {
		//			return true;
		//		} else {
		//			return false;
		//		}
	}

	[Task]
	public void AttackTarget()
	{
		for (int i = 0; i < Team1Units.Length; i++) {
			if ((Team1Units [i].transform.position.x < this.transform.position.x + stats.atkRange) && (Team1Units [i].transform.position.x > this.transform.position.x - stats.atkRange)) {
				if ((Team1Units [i].transform.position.z < this.transform.position.z + 0.3f) && (Team1Units [i].transform.position.z > this.transform.position.z - 0.3f)) {
					Team1Units [i].GetComponent<Stats> ().hp -= stats.atk;
					break;
				}
			}
		}
		for (int i = 0; i < Team1Units.Length; i++) {
			if ((Team1Units [i].transform.position.z < this.transform.position.z + stats.atkRange) && (Team1Units [i].transform.position.z > this.transform.position.z - stats.atkRange)) {
				if ((Team1Units [i].transform.position.x < this.transform.position.x + 0.3f) && (Team1Units [i].transform.position.x > this.transform.position.x - 0.3f)) {
					Team1Units [i].GetComponent<Stats> ().hp -= stats.atk;
					break;
				}
			}
		}
		//saldırdıktan sonra, her aksiyon sonu olduğu gibi ve turHakkını false'la. Gerçi gerek kalmadı task olarak hallettim onu.
		//çevresine baksın en vurması avantajlı unite vursun. Şimdilik random yapsan da olur.
		//accuracyi kullanmayı unutma. random değer döndürerek yap. 0-100 arası değerden büyükse 90 ok. btyi salla direk burda onu yapabilirsin.

		Task.current.Succeed ();
	}

	[Task]
	public void Die()
	{
		Destroy (this.gameObject);
		//put the animation here.
		Task.current.Succeed ();
	}

	[Task]
	public bool IsHealthLessThan(float health)
	{
		return stats.hp < health;
	}

	[Task]
	public bool IsiznimVarPermissionGiven()
	{
		if (stats.iznimVar == true) {
			return true;
		} else {
			return false;
		}
	}

	[Task]
	public void GiveiznimVarPermission(bool permission)
	{
		if (permission == true) {
			stats.iznimVar = true;
		} else if (permission == false){
			stats.iznimVar = false;
		}
		Task.current.Succeed ();
	}

	[Task]
	public bool IsTurHakkıPermissionGiven()
	{
		if (stats.turHakkı == true) {
			return true;
		} else {
			return false;
		}
	}

	[Task]
	public void GiveTurHakkıPermission(bool permission)
	{
		if (permission == true) {
			stats.turHakkı = true;
		} else if (permission == false){
			stats.turHakkı = false;
		}
		Task.current.Succeed ();
	}

	[Task]
	public bool IsOtherJobsDone()
	{
		if (is_other_jobs_done == true) {
			return true;
		} else {
			return false;
		}
	}










}
