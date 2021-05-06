using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;



public class WackGameManager : MonoBehaviour {

	//[SerializeField]public WaveManager waveController;
//	[SerializeField]public WackLookClick wacklookClick;

	public Transform centerPos;
	//[SerializeField]private WackBerryController wackBerryController;
	public GameObject[] totalMoles;
	public List<GameObject> activeMoles;

	public Transform[]totalBranches;
	private List<Transform> totalBerries;

    [Space]
    [Header("References")]
   // [SerializeField]private Points
	[SerializeField]private int berriesLeft;
	[SerializeField]private Text berriesText;
	[SerializeField]private GameTimer gameTimer;


	public static WackGameManager Instance
	{ get { return instance; } }

	private static WackGameManager instance = null;

	void Awake()
	{

		if (instance) {
			Debug.LogError ("Two instances of singleton (WackGameManager)");

			return;
		}
		instance = this; 


		totalMoles = WaveManager.Instance.GetAllGOInAllWaves ().ToArray();
			//waveController.GetAllGOInAllWaves ().ToArray();


	}

    public Transform mainObjectToDerive;
    //public Dictionary<Transform, GameObject> branchToNotificationGOList;
    public List<GameObject> branchToNotificationGOList;
    public void Start()
    {

        totalBerries = new List<Transform>();
        ////branchToNotificationGOList = new Dictionary<Transform, GameObject>();//new List<GameObject>();
        //branchToNotificationGOList = new List<GameObject>();

        //for (int i = 0; i < mainObjectToDerive.childCount; i++)
        //{
        //    //Each Set
        //    foreach (Transform set in mainObjectToDerive.GetChild(i))
        //    {

        //        //each_table
        //        foreach (Transform table in set.GetChild(0))
        //        {
        //            //each item
        //            foreach (Transform item in table)
        //            {
        //                totalBerries.Add(item);
        //            }

        //        }
        //        //each notification
        //        foreach (Transform not in set.GetChild(1))
        //        {
        //            //each item
        //            branchToNotificationGOList.Add(not.gameObject);

        //        }

        //    }


		foreach (Transform be in totalBranches) {

			foreach (Transform b in be)
				totalBerries.Add(b);

		}

berriesLeft = totalBerries.Count;
		berriesText.text = ": " + berriesLeft; 
	
	
	}
	public bool BranchHasBerries(Transform branch){


		foreach (Transform be in branch) {

			if (be.gameObject.activeInHierarchy)
				return true;

		}

		return false;
	}

    public IEnumerator TurnOnTurnOff(GameObject obj)
    {

        obj.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        obj.SetActive(true);
       // obj.transform.
    }
	public void ReduceBerry(Transform branch){


		foreach (Transform be in branch) {

			if (be.gameObject.activeInHierarchy) {

				be.gameObject.SetActive (false);
				totalBerries.Remove (be);
				berriesLeft -= 1;

				if (berriesLeft == 0) {
					GameController.Instance.isGameOver = true;
					gameTimer.SetGameOver ("No More Berries :(");
					StartCoroutine (GetComponent<WackLookClick>().TurnOffAll (Mathf.Infinity));
					GetComponent<WackLookClick> ().isAllowPopUps = false;
					WaveManager.Instance.EndWavesAndDisableAllObjects ();

				}

               // StartCoroutine(TurnOnTurnOff(branchToNotificationGOList[totalBerries.IndexOf(branch)]));

				berriesText.text = ": " + berriesLeft; 

				break;
			}

		}
	}

    public Transform GetRandomBush()
    {
        List<Transform> availableBushes = new List<Transform>(); //= new Transform[totalBranches];
        foreach (Transform bs in WackGameManager.Instance.totalBranches)
        {

                if (!BranchHasBerries(bs))
                    continue;

            availableBushes.Add(bs);


        }
        
        return availableBushes[Random.Range(0, availableBushes.Count)];
    }

	public Transform GetClosestBush(Transform moleTrans){
		Transform closestBush = null;

		float closestBushDistance = Mathf.Infinity;

		foreach (Transform bs in WackGameManager.Instance.totalBranches) {

			if (Vector3.Distance (moleTrans.position, bs.position) < closestBushDistance) {

				if (!BranchHasBerries (bs))
					continue;

				closestBush = bs;
				closestBushDistance = Vector3.Distance (moleTrans.position, bs.position);

			}

		}
		return closestBush;
	//	StartCoroutine (SeekBush ());

	}

    //private void Update()
    //{
    //   // UpdateMoleActiveList();
    //}

    public void UpdateMoleActiveList(){

		if (totalMoles.Length == 0)
			return;
		
		for (int i = 0; i < totalMoles.Length; i++) {

			if (totalMoles [i].activeInHierarchy) {
				if (activeMoles.Contains (totalMoles [i]))
					continue;
				else
				activeMoles.Add (totalMoles [i]);
				   }
	
		}
		Debug.Log ("Number of active moles: " + WackGameManager.Instance.activeMoles.Count);
	}

	//public void Sto
}