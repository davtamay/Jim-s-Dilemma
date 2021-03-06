using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[ExecuteInEditMode]
public class WaveManager : MonoBehaviour {
	
#region PROPERTIES AND FIELDS
	private Transform thisTransform;

	[Header("Main Game Events")]
	[SerializeField]UnityEvent onGameStart;
	[SerializeField]UnityEvent onGameCompleted;

	[Header("Wave Transition Events")]
	[SerializeField]UnityEvent onWaveEnd;
	[SerializeField]private float newWaveIntermissionTime = 3;
	[SerializeField]UnityEvent onNewWaveStart;
	[SerializeField]UnityEvent onNewWaveObjectsEnabled;

	//[SerializeField]private GameObject newWaveIntermissionIndicator;

	//[SerializeField]private bool isDeactivateIndicatorAfterIntermission;

	[Tooltip("Do you want to leave wave on for the next wave?")]
	[SerializeField]private bool leaveWavesActiveWhenDone;

	[Header("Timer Settings")]
	[SerializeField]private BoolVariable isStartUIActive;
	[SerializeField]TimerClass TIMER;
	[SerializeField]float timerSpeed = 1.2f;
    [SerializeField] UnityEvent onTimerReach0;

    [Header("Waves (Based on children)")]
    [Tooltip("Create waves by parenting children with objects to spawn")]
    private int currentWave;
    
    
	public List<WaveObject> waveEvents;

	private Dictionary<int,WaveObject>waveID;
	private List<GameObject> totalGOs;
	private List<int> myIndices;

	private static WaveManager instance;
	public static WaveManager Instance
	{ get { return instance; } }

#endregion

//THIS IS WHERE WE SET UP THE EDITOR TO UPDATE BASED ON CHILDREN PLACED IN THIS COMPONENT'S TRANSFORM
#region EDITOR SETUP
#if UNITY_EDITOR
//INSPECTOR SET UP TO SHOW WAVE OBJECTS DEPENDING ON PRESENT CHILDREN IN PARENT 

	void Update(){

	if(!Application.isPlaying)
		if (transform.hasChanged) {
			
			waveID = new Dictionary<int, WaveObject> ();

				int count = 0;
				int waveNum = 0;
				List<int> tempArray = new List<int> ();
				foreach (Transform t in transform) {

					if (t.parent == transform) {
						
						tempArray.Add (t.GetInstanceID());

					if (count >= waveEvents.Count) {
						waveID.Add (t.GetInstanceID (), new WaveObject ());
						waveEvents.Add (waveID [t.GetInstanceID ()]);

						waveID [t.GetInstanceID ()].InstanceID = t.GetInstanceID();
						waveID [t.GetInstanceID ()].waveTransformParent = t;
						waveID [t.GetInstanceID ()].waveNumber = waveNum;
						++waveNum;
							
					} else {
						waveID.Add (t.GetInstanceID (), waveEvents [count]);

						waveID [t.GetInstanceID ()].InstanceID = t.GetInstanceID();
						waveID [t.GetInstanceID ()].waveTransformParent = t;
						waveID [t.GetInstanceID ()].waveNumber = waveNum;
						++waveNum;
					}
						++count;
					}

				}

				while(waveEvents.Count > tempArray.Count){

					if (tempArray.Count == 0 || waveEvents.Count == 1){
						waveEvents.Clear ();
						break;
					}

				waveEvents.Remove(waveEvents.Last ());

				}
						
			}
	}
//THIS IS THE CUSTOM DATA BLOCK FOR EACH WAVE
#endif
#endregion
//THIS IS WHAT THE INSPECTOR SHOWS FOR EACH CHILD PARENTED TO THIS COMPONENT
#region CustomClassForWaves
	[System.Serializable]
	public class WaveObject{

		[HideInInspector]	
		public int InstanceID;
		public int waveNumber;
		[Tooltip("This is your parent holding the spawned objects.")]
		public Transform waveTransformParent;
		public UnityEvent onIndividualWaveStart;
		public int numberToSpawn;
		public int timeUntilNextWave;
		[Header("Change Wave ADDITIONAL Settings")]
		public bool isChangeWaveWhenAllDisabled;
		public bool isResetTime;

	}
#endregion

#region WAVEINITIATION AND UPDATER
	void Awake(){
		
		thisTransform = transform;

		totalGOs = new List<GameObject> ();

		foreach (Transform gOP in transform) {
			foreach (Transform gOC in gOP) {
				totalGOs.Add (gOC.gameObject);
				gOC.gameObject.SetActive (false);
			}
		}

		if (Application.isPlaying) {
			
			if (instance) {
				Debug.LogWarning ("There are two WaveManagers in scene - deleting late instance.");
				DestroyImmediate (this.gameObject);
				return;
			}
			instance = this; 
		}
	}


	void Start(){

		transform.hasChanged = true;

		if (Application.isPlaying) {
			//CHECK FOR PRESENCE OF WAVE INDICATOR IN SCENE
//			if (GameObject.FindWithTag ("NewWave")) {
//				newWaveIntermissionIndicator = GameObject.FindWithTag ("NewWave");
//				//OBTAIN INDICATORS OWN TIME UNTIL DISAPEAR - USEFUL FOR INDICATORS THAT USE FADING EFFECTS
//				if(newWaveIntermissionIndicator.transform.GetComponent<NewWaveTransition> () != null)
//					newWaveIntermissionTime = newWaveIntermissionIndicator.transform.GetComponent<NewWaveTransition> ().timeUntilDisapear;
//				newWaveIntermissionIndicator.gameObject.SetActive (false);
//			}

			if (waveEvents.Count == 0)
				return;
		

			StartCoroutine (OnUpdate ());
		}
	}



	private bool isDone;
	private bool isByPassIsDoneCheck = false;

	private float timeLeft;
	private float timeElapsed;
    private bool isDisableEveryObject;
    IEnumerator OnUpdate() {
        onGameStart.Invoke();

        //ADD TIME FOR FIRST WAVE
        //	TimeToAdd (ref isDone, waveEvents [0].timeUntilNextWave);
        timeLeft = waveEvents[0].timeUntilNextWave;

       // yield return new WaitUntil(() => !isStartUIActive.isOn);
        //GET RANDOMIZED INDEX LIST OF CHILDREN
        RandomizeGOToEnable(waveEvents[0].numberToSpawn, waveEvents[0].waveTransformParent);

        //new 12/31/18
      //  yield return new WaitUntil(() => !isStartUIActive.isOn);

        //ACTIVATE 
        foreach (int gO in myIndices)
            waveEvents[0].waveTransformParent.GetChild(gO).gameObject.SetActive(true);

        onNewWaveObjectsEnabled.Invoke();
        waveEvents[0].onIndividualWaveStart.Invoke();
        //onNewWaveStart.Invoke();

        TIMER.ResetTimer();
        TIMER.SetTimeToDisplay(timeLeft);
        TIMER.StopTimer();

       

        TIMER.StartTimer();

        while (true) {
            yield return null;

            timeElapsed = TIMER.GetTime();
      

            if (!isByPassIsDoneCheck)
            {

                yield return null;


            }
            else
                isByPassIsDoneCheck = false;

            //ALLOW FOR WAVE END ONCE NO MORE OBJECTS PRESSENT
            if (waveEvents[currentWave].isChangeWaveWhenAllDisabled) {

                //   waveEvents[currentWave].onIndividualWaveStart.Invoke();

                int EnabledCount = 0;
                int childCount = waveEvents[currentWave].waveTransformParent.childCount;

                for (int i = 0; i < childCount; i++) {

                    if (waveEvents[currentWave].waveTransformParent.GetChild(i).gameObject.activeInHierarchy)
                        EnabledCount += 1;

                }
                if (EnabledCount == 0) {
                   
                    //TimeToAdd (ref isDone);
                    //CHECK WHETER TO RESET TIMER
                    //if (waveEvents[currentWave].isResetTime)
                    //{
                    //    TIMER.ResetTimer();

                    isDisableEveryObject = true;
                    //timeElapsed = Mathf.Infinity;

                    // }
                    //CompleteTimer ();

                }

            }

            if (timeElapsed > timeLeft || isDisableEveryObject)
            {


                isDisableEveryObject = false;
                TIMER.StopTimer();
                onTimerReach0.Invoke();
                Debug.Log(timeLeft);


          	

				if (!leaveWavesActiveWhenDone)
					transform.GetChild (currentWave).gameObject.SetActive (false);

				if (currentWave + 1 == transform.childCount) {
					onGameCompleted.Invoke ();
					Debug.Log ("GAME ENDED!");
					EndWavesAndDisableAllObjects ();
					yield break;
				}

				onWaveEnd.Invoke ();
				//yield return StartCoroutine (NewWaveIntermission ());
				yield return new WaitForSecondsRealtime (newWaveIntermissionTime);
				//StartCoroutine (NewWaveIntermission ());newWaveIntermissionTime
				onNewWaveStart.Invoke ();

				++currentWave;

                
				waveEvents[currentWave].onIndividualWaveStart.Invoke ();



                if (!waveEvents[currentWave].isResetTime)
                {
                    //Debug.Log("TIMELEFT:   " + timeLeft + "TIMEUNTILNEXT" + waveEvents[currentWave].timeUntilNextWave);
                    timeLeft += waveEvents[currentWave].timeUntilNextWave;
                    //Debug.Log("TIMELEFT:   " + timeLeft + "TIMEUNTILNEXT" + waveEvents[currentWave].timeUntilNextWave);

                }
                if (waveEvents[currentWave].isResetTime)
                {
                    TIMER.ResetTimer();
                    timeLeft = waveEvents[currentWave].timeUntilNextWave;


                }


                // - waveEvents[currentWave - 1].timeUntilNextWave;
                /* timeLeft += waveEvents[currentWave].timeUntilNextWave - timeElapsed*///REMOVED 5/15/2019 TO FIX TIMING ISSUE (GOING THROUGH LEVELS) IN HIT- timeElapsed; BUT ADDS THE PREVIOUS ONE..


                //TimeToAdd (ref isDone, waveEvents [currentWave].timeUntilNextWave);

                RandomizeGOToEnable(waveEvents [currentWave].numberToSpawn, waveEvents [currentWave].waveTransformParent);

				foreach (int gO in myIndices) 
					waveEvents [currentWave].waveTransformParent.GetChild (gO).gameObject.SetActive (true);

				onNewWaveObjectsEnabled.Invoke ();
			

				//ResumeTimer();
				TIMER.SetTimeToDisplay (timeLeft);
				TIMER.StartTimer ();
			}
	}
		
	}

#endregion

#region WAVEOBJECTS METHODS
	public List<GameObject> GetAllGOInAllWaves(){

		return totalGOs;

	}

	/// <summary>
	/// Stop all waves and deactivate all active GameObjects in waves.
	/// </summary>
	public void EndWavesAndDisableAllObjects(){



		StopAllCoroutines ();

		foreach (Transform gOP in transform) {
			foreach (Transform gOC in gOP) {
				totalGOs.Add (gOC.gameObject);
				gOC.gameObject.SetActive (false);
			}
		}

	}
	/// <summary>
	/// Randomizes children indixes of provided parent by setting up myIndices List.
	/// </summary>
	/// <param name="numToRespawn">Number that you want to respawn.</param>
	/// <param name="parent">parent that you want to randomize children for..</param>
	private void RandomizeGOToEnable(int numToRespawn, Transform parent){

		int ChildCount = parent.childCount;

		if (numToRespawn > ChildCount) {
			Debug.LogWarning ("numToRespawn/GOToRespond can't be bigger than available childCount");
			return;
		}

		myIndices = new List<int> (ChildCount);

		for (int i = 0; i < numToRespawn; i++) {

		int myIndexPull = Random.Range(0, ChildCount);


		while (myIndices.Contains (myIndexPull)) {

			myIndexPull = Random.Range (0, ChildCount);

		}

		myIndices.Add (myIndexPull);

	}
}
#endregion

#region TIMER & METHODS
	private float timer;
	bool isTimerOn = false;
	float timerTimeScale = 1;
	Coroutine timerCoroutine;
	/// <summary>
	/// Check or AddTime to WaveTimer
	/// </summary>
	/// <returns>Current Time in string 00:00</returns>
	/// <param name="isDone">Add Ref bool local variable to have reference to current timer completion flag.</param>
	/// <param name="time">Add time to current wave timer, it could be left out if only checking isDone.</param>
	public string TimeToAdd(ref bool isDone, float time = 0f){

		timer += time;

        if (timer > 0f)
            isTimerOn = true;

		//if (time > 0f) {

		//	isTimerOn = true;
				
		//	//PREVENT RUNNING TWO COROUTINES WHEN ADDING TIME
		//	if (timerCoroutine != null)
		//		StopCoroutine (timerCoroutine);
			
		//		timerCoroutine = StartCoroutine (StartTimer ());

		//}

		string minutes = Mathf.Floor(timer /60).ToString("00");
		string seconds = Mathf.Floor (timer % 60).ToString ("00");

		if (timer <= 0f) {
			timer = 0f;
			isDone = true;
			isTimerOn = false;
		}

		return minutes + ":" + seconds;

	}

	private IEnumerator StartTimer(){

		while (isTimerOn) {

			timer -= Time.deltaTime * timerSpeed * timerTimeScale;

			yield return null;
		}
			
	}

	public void ResumeTimer(){

		timerTimeScale = 1;
        isTimerOn = true;
        TIMER.isTimerRunning = true;
       // isTimerOn = true;
	}

	public void StopTimer(){

		timerTimeScale = 0;
        isTimerOn = false;
        TIMER.isTimerRunning = false;
      //  isTimerOn = false;

	}
	public void CompleteTimer(){

		timer = 0;
	}
	public void ChangeTONextWaveAndAddTime(){

		isByPassIsDoneCheck = true;
		isDone = true;
	}
		
	public float GetCurrentTime(){

		return timer;
	}
#endregion

#region WAVEINDICATOR METHODS
//
//	bool isWaveIndicatorOn;
//	public IEnumerator NewWaveIntermission(){
//		isWaveIndicatorOn = true;
//
//		if (newWaveIntermissionIndicator != null) {
//			newWaveIntermissionIndicator.SetActive (true);
//			//WAVEINDICATOR CONTINUES TO SHOW WHEN TIMESCALE IS 0 BECAUSE OF THIS.
//			yield return new WaitForSecondsRealtime (newWaveIntermissionTime);
//			if (isDeactivateIndicatorAfterIntermission)
//				newWaveIntermissionIndicator.SetActive (false);
//		}else
//			yield return new WaitForSeconds (newWaveIntermissionTime);
//
//		isWaveIndicatorOn = false;
//	}
//
//	public float GetNewWaveTime{
//
//		get{return newWaveIntermissionTime;}	
//	}
//
//	public bool GetNewWaveTransitionState(){
//
//		if (isWaveIndicatorOn)
//			return true;
//
//		return false;
//
//	}
//
#endregion



}




