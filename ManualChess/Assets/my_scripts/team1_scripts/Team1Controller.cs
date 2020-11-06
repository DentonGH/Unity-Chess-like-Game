using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Team1Controller : MonoBehaviour {

	public GameObject[] team1Units;
	public GameObject[] team2Units;
	//int sayac = 0;
	public bool upgradeDone;
	public bool buyUnitsDone;
	int Gold = 200;
	//int upgradeScore = 0;
	//int buyUnitScore = 0;
	public GameObject knight;
	public GameObject archer;
	public GameObject mage;
	public GameObject swordsman;
	public GameObject spawnPoint;
	GameObject[] sunak;
	int random_no;
	public GameObject team2_object;

	[Task]
	bool TurBende()
	{
		for (int i = 0; i < team1Units.Length; i++) {
			if (team1Units[i].GetComponent<Stats>().turHakkı == true){
				return true;
			}
		}
		return false;
	}

	void Start () 
	{
		sunak = GameObject.FindGameObjectsWithTag ("sunak");
	}

	void Update () 
	{
		team1Units = GameObject.FindGameObjectsWithTag ("team1"); //tur bana her geldiğinde behaviour treeden çek bu 2sini
		team2Units = GameObject.FindGameObjectsWithTag ("team2");
		if (iznimVarAll()) {
			TurAttirici ();
		}
	}


	bool TurStillTeam1()
	{
		for (int i = 0; i < team1Units.Length; i++) {
			if (team1Units[i].GetComponent<Stats>().turHakkı == true){
				return true;
			}
		}
		return false;
	}
		
	void TurAttirici()
	{
		if (!TurStillTeam1 ()) {
			for (int i = 0; i < team2Units.Length; i++) {
				team2Units [i].GetComponent<Stats> ().turHakkı = true;
			}
			for (int i = 0; i < team1Units.Length; i++) {
				team1Units [i].GetComponent<Stats> ().iznimVar = false;
			}
			team2_object.GetComponent<Team2Controller> ().upgradeDone = false;
			team2_object.GetComponent<Team2Controller> ().buyUnitsDone = false;
		}
	}
		
	[Task]
	public void HangiUnitOynasınTeam()
	{
		GameObject oncelik;

		if (TurStillTeam1() && !iznimVarAll() && IsJobsDone()) {
			List<GameObject> tempTeam1Units = new List<GameObject>();
			for (int k = 0; k < team1Units.Length; k++) {
				if (team1Units [k].GetComponent<Stats> ().iznimVar == false) {
					tempTeam1Units.Add (team1Units [k]);
				}
			}
			oncelik = tempTeam1Units [0];											
			for (int i = 0; i < tempTeam1Units.Count - 1; i++) {
				if (tempTeam1Units [i+1].GetComponent<Stats>().hp > oncelik.GetComponent<Stats>().hp && tempTeam1Units [i+1].GetComponent<Stats>().iznimVar == false) {
					oncelik = tempTeam1Units [i+1];
				}
			}
			oncelik.GetComponent<Stats> ().iznimVar = true;
			GameObject temp = oncelik;
		}
	}

	bool iznimVarAll()
	{
		for (int i = 0; i < team1Units.Length; i++) {
			if (team1Units[i].GetComponent<Stats>().iznimVar == false){
				return false;
			}
		}
		return true;
	}

	public bool IsJobsDone()
	{
		if (upgradeDone || buyUnitsDone) {
			return true;
		} else {
			return false;
		}
	}




	[Task]
	bool WillUpgrade()
	{
		//her zaman false yapıyorum şimdilik for convenience.
		if (WillYouUpgrade() > 11) { 
			return true;
		} else
			return false;
	}
	[Task]
	bool TemporaryTask()
	{
		
		if (team1Units.Length < 11) { 
			return true;
		} else
			return false;
	}


	int WillYouUpgrade() //make a better scaling system later on.
	{
		if (team1Units.Length > 10) {
			return 10;
		} else
			return 0;
	}

	int WillYouBuyUnits() //make a better scaling system later on.
	{
		return (10 - (team1Units.Length - 1));
	}

	[Task]
	public bool UpgradeDone()
	{
		if (upgradeDone) {
			return true;
		} else {
			return false;
		}
	}

	[Task]
	public bool BuyUnitsDone()
	{
		if (buyUnitsDone) {
			return true;
		} else {
			return false;
		}
	}

	[Task]
	public void MakeUpgradeDone()
	{
		upgradeDone = true;
		Task.current.Succeed ();
	}

	[Task]
	public void MakeBuyUnitDone()
	{
		buyUnitsDone = true;
		Task.current.Succeed ();
	}
		
	GameObject PickUnitToUpgrade()
	{
		if (random_no == 0) {
			if (Gold > swordsman.GetComponent<Stats> ().upgradeCost) {
				return swordsman;
			}
		}

		else if (random_no == 1) {
			if (Gold > archer.GetComponent<Stats> ().upgradeCost) {
				return archer;
			}
		}

		else if (random_no == 2) {
			if (Gold > knight.GetComponent<Stats> ().upgradeCost) {
				return knight;
			}
		}
		return mage;
	} 

	[Task]
	void UpgradeTheUnit()
	{	
		//upgrade'i unite özel tasarla sonra.
		PickUnitToUpgrade ().GetComponent<Stats> ().hp += 2;
		PickUnitToUpgrade ().GetComponent<Stats> ().atk += 1;
		Gold -= PickUnitToUpgrade().GetComponent<Stats>().upgradeCost;
		Task.current.Succeed ();
	}

	[Task]
	bool CanIBuyUnits() //bunu int yap. hangi random nodan çıktıysa onu göndersin. hiçbirinden çıkamazsa return 10 dersin. sonra pickunittobuyda random_no 4ten küçükse random_nonun karşılık geldiği adamı basabilirsin.
	{
		random_no = Random.Range (0, 3);
		if (random_no == 0) {
			if (Gold > swordsman.GetComponent<Stats> ().buyCost) {
				return true;
			}
		}

		if (random_no == 1) {
			if (Gold > archer.GetComponent<Stats> ().buyCost) {
				return true;
			}
		}

		if (random_no == 2) {
			if (Gold > knight.GetComponent<Stats> ().buyCost) {
				return true;
			}
		}

		if (random_no == 3) {
			if (Gold > mage.GetComponent<Stats> ().buyCost) {
				return true;
			}
		}
		buyUnitsDone = true;
		return false;
	}

	GameObject PickUnitToBuy()
	{
		if (random_no == 0) {
			if (Gold > swordsman.GetComponent<Stats> ().buyCost) {
				return swordsman;
			}
		}

		else if (random_no == 1) {
			if (Gold > archer.GetComponent<Stats> ().buyCost) {
				return archer;
			}
		}

		else if (random_no == 2) {
			if (Gold > knight.GetComponent<Stats> ().buyCost) {
				return knight;
			}
		}
		return mage;
	} 

	[Task]
	void BuyTheUnit()
	{
		if (team1Units.Length < 10) {
			GameObject unit = PickUnitToBuy ();
			var newUnit = Instantiate (unit, spawnPoint.transform.position, spawnPoint.transform.rotation);
			newUnit.transform.parent = gameObject.transform;
			Gold -= unit.GetComponent<Stats> ().buyCost;

		}
		Task.current.Succeed ();
	}

	[Task]
	bool CanIUpgrade()
	{
		random_no = Random.Range (0, 3);
		if (random_no == 0) {
			if (Gold > swordsman.GetComponent<Stats> ().upgradeCost) {
				return true;
			}
		}

		if (random_no == 1) {
			if (Gold > archer.GetComponent<Stats> ().upgradeCost) {
				return true;
			}
		}

		if (random_no == 2) {
			if (Gold > knight.GetComponent<Stats> ().upgradeCost) {
				return true;
			}
		}

		if (random_no == 3) {
			if (Gold > mage.GetComponent<Stats> ().upgradeCost) {
				return true;
			}
		}
		return false;
	}

	void ChildCount() //EKLENMEDi KODA!!!
	{
		if (this.transform.childCount == 0)
			Debug.Log ("Game Ended");
			Debug.Log("Team2 Wins");
		// oyun bittiğinde olacak şeyleri ekle ve retry main menu butonları getir vs.
	}

	[Task]
	void GatherGold() //sunağın tam üstünde olmayacak. onun ayarlamasını yap.
	{
		for (int i = 0; i < team1Units.Length; i++) {
			for (int k = 0; k < sunak.Length; k++) {
				if (team1Units [i].transform.position == sunak [k].transform.position) {
					Gold += 100;
				}
			}
		}
		Gold += 50;
		Task.current.Succeed ();
	}
}