using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class WackLookClick : MonoBehaviour{

	private Camera cam;
    //z
    public Transform gOToSetDirectionOfDetection;
	public float speedDifficulty = 1f;
	private float timer;


	private int isPopUpHash = Animator.StringToHash("IsPopUp");
	private int	isDeadHash = Animator.StringToHash ("IsDead");
	private int	isEatingHash = Animator.StringToHash ("IsEating");


	public int currentMoleIndex;

	public GameObject deadCloudActionGO;


	public GameObject currentMole = null;
	public bool isAllowPopUps = true;

	public float changeWaveTurnOffTime;


    [Header("References")]
    [SerializeField] DataManager DATA_MANAGER;
//	WackGameManager WACK_GAME_MANAGER;

	void Start () {
		cam = Camera.main;

        
        
	}
    
    public void StartSpawning()
    {
       // StartCoroutine(UpdateLookRaycast());
        StartCoroutine(TurnOnController());

    }

	//public void OnPointerEnter(PointerEventData eventData){


	//}

    public void OnMoleDetect(Collider other)
    {
        Debug.Log("I was clicked");
        Vector3 pos = other.transform.position;
        GameObject temp = Instantiate(deadCloudActionGO, pos, Quaternion.LookRotation(cam.transform.position - pos, Vector3.up));
        Destroy(temp, 5);

        AudioSource tempAS = AudioManager.Instance.GetAudioSourceReferance(AudioManager.AudioReferanceType._DIRECT, "Pop");
        tempAS.transform.position = pos;
        tempAS.Play();

        

        StartCoroutine(WaitForAction(other.gameObject, true));


    }
    public IEnumerator WaitForAction(GameObject gO, bool hit)
    {

        yield return StartCoroutine(TurnOffMole(gO, true));
        //currentMole = null;
    }


	IEnumerator UpdateLookRaycast(){
        do { yield return null; }
        while (DATA_MANAGER.isStartUIOn.isOn);

        yield return new WaitForEndOfFrame();
		currentMole = WackGameManager.Instance.activeMoles [0];
	
			while (true) {

				Ray ray = new Ray (cam.transform.position, cam.transform.rotation * Vector3.forward);

				RaycastHit hit;

			if (Physics.SphereCast (ray, 0.1f, out hit, 500, 1 << 13) ){
              //  if (Physics.CapsuleCast(ray.origin, ray.direction * 100, 0.1f, ray.direction, out hit, 100f, 1 << 13))
                

                    if (hit.collider.CompareTag ("Wacked")) {

					Debug.Log (hit.collider.gameObject.name);
			
						GameObject temp = Instantiate(deadCloudActionGO, hit.point, Quaternion.LookRotation(cam.transform.position - hit.point, Vector3.up));
						Destroy(temp, 5);

						AudioSource tempAS = AudioManager.Instance.GetAudioSourceReferance (AudioManager.AudioReferanceType._DIRECT, "Pop");
						tempAS.transform.position = hit.point;
						tempAS.Play ();

						//hit.collider.enabled = false;

						yield return StartCoroutine (TurnOffMole (hit.transform.gameObject, true));
						

					}
			

				} 
			yield return null;
			}
	}
		
	public IEnumerator TurnOnController(){
        //do { yield return null; }
        //while (DATA_MANAGER.isStartUIOn);
        // yield return new WaitWhile(() => DATA_MANAGER.isStartUIOn);
        yield return new WaitForEndOfFrame();
        currentMole = WackGameManager.Instance.activeMoles[0];


        while (true){
			
			yield return null;

			if (!isAllowPopUps) 
				continue;
			


			timer += Time.deltaTime;
			if (timer > speedDifficulty) {

				currentMoleIndex = GetComparedRandomMoleIndex (currentMole);
		
					while(WackGameManager.Instance.activeMoles[currentMoleIndex].activeInHierarchy && !WackGameManager.Instance.activeMoles [currentMoleIndex].GetComponentInChildren<Animator> ().GetCurrentAnimatorStateInfo (0).IsName("Idle"))
                    {

                        currentMole = WackGameManager.Instance.activeMoles [Random.Range (0, WackGameManager.Instance.activeMoles.Count)];
					    currentMoleIndex = GetComparedRandomMoleIndex (currentMole);
					    yield return null;
				}

				currentMole = WackGameManager.Instance.activeMoles [currentMoleIndex];
			
				yield return StartCoroutine (TurnOnMole (currentMole));
				timer = 0;

				yield return null;


			}
	
		}
	
	
	}
	public void DisableAllMoles(float time){

		StartCoroutine	(TurnOffAll (time));

	}
	//make this turn off all for a bit
	public void TurnOffAllMolesForWaveChange(){

//		isAllowPopUps = false;
//
//		foreach (GameObject mole in WackGameManager.Instance.activeMoles) 
//			StartCoroutine (TurnOffMole (mole));

		//new 12/05/17
		StartCoroutine	(TurnOffAll (changeWaveTurnOffTime));
		//FIXME How do you get new wave transitionTime
	//	float timeOff = GameController.Instance.GetNewWaveTime;
	//	StartCoroutine (TurnOnAllowPopUps (timeOff));//TurnOffAll(timeOff));

	}
	IEnumerator TurnOnAllowPopUps(float tOff){
		yield return new WaitForSeconds (tOff);
		isAllowPopUps = true;

	}

	public IEnumerator TurnOffAll(float offTime){
		
		isAllowPopUps = false;


		foreach (GameObject mole in WackGameManager.Instance.activeMoles) {
			StartCoroutine (TurnOffMole (mole));
		}
	
		//yield return null;
		yield return new WaitForSeconds (offTime);

			isAllowPopUps = true;
			
		}

	IEnumerator TurnOffMole (GameObject mole, bool hit = false){

        if (!mole.activeInHierarchy)
            yield break;

		mole.GetComponentInChildren<CapsuleCollider> ().enabled = false;

		Animator an = mole.GetComponentInChildren<Animator> ();

		an.SetTrigger (isDeadHash);

		an.ResetTrigger (isPopUpHash);

		if(an.HasParameter (isEatingHash))
		an.ResetTrigger (isEatingHash);




		if(hit)
			PlayerManager.Instance.points = mole.GetComponent<WackHitPoints> ().hitPoints;


		yield return null;
	
		//mole.GetComponent<WackSpawner> ().StopAllCoroutines ();
	}
	IEnumerator TurnOnMole(GameObject mole){
        //May affect appearance in other parts?
        if (!mole.activeInHierarchy)
            yield break;

        mole.GetComponentInChildren<CapsuleCollider> ().enabled = true;

		Animator an = mole.GetComponentInChildren<Animator> ();

		if(an.HasParameter (isEatingHash))
			an.ResetTrigger (isEatingHash);
		

		an.ResetTrigger (isDeadHash);
		an.SetTrigger (isPopUpHash);


		if (mole.GetComponent<WackSpawner> () != null)
		mole.GetComponent<WackSpawner> ().SetRandomPos ();
		yield return null;


	
	}


	/// <summary>
	/// Gets the index of the compared random mole.
	/// </summary>
	/// <returns>To avoid calling same mole again (has to have atleast two moles in active list or editor will crash!).</returns>
	/// <param name="gO">currentMoleActive.</param>
	private int GetComparedRandomMoleIndex(GameObject gO){
	
		int randomMole = Random.Range (0, WackGameManager.Instance.activeMoles.Count);

		while (gO.GetInstanceID () == WackGameManager.Instance.activeMoles [randomMole].GetInstanceID ()) {
		

			randomMole = Random.Range (0, WackGameManager.Instance.activeMoles.Count);
		

		}
		return randomMole;

	}


		
	}

