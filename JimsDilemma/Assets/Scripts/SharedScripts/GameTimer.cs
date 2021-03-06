using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using RoboRyanTron.Unite2017.Events;


public class GameTimer : MonoBehaviour {

    private Text timerText;
    private Color originalColor;

    private bool isDone;

    [SerializeField] private bool isShowTextWhenDone = true;
    [SerializeField] private string textToShow;
    //[SerializeField] private bool isPauseGameWhenDone = false;

    [Header("References")]
    //[SerializeField] private bool triggerEvents;
    //[SerializeField] private GameEvent gameTimerDone;

    [SerializeField] private TimerClass TIMER_CLASS;
    [SerializeField] private LocalizationManager LOCALISATION_MANAGER;
    //private string timer;

    // Use this for initialization
    void Start() {


        timerText = GetComponent<Text>();


        //SetUpcomingTimerDoneTextToShow(textToShow);

        //timerText.text = LocalizationManager.GetLocalizedValue (textToShow);
        originalColor = timerText.color;

        SetTextToShow(textToShow);
        //SetTextToShow(textToShow);
        //    textToShow;//TIMER_CLASS.GetFormattedTime();//WaveManager.Instance.TimeToAdd (ref isDone);



        //ShowText(true);

        //StartCoroutine (OnUpdate ());
        //		InvokeRepeating("TimerUpdate", 0, 0.1f);
    }

    public void SetTextToShow(string key) {

        textToShow = LOCALISATION_MANAGER.GetLocalizedValue(key);
        //ShowText(true);
        //StartCoroutine(ShowTextInsteadOfTime(5));//key;
                                                                 //new
                                                                 //return textToShow;
    }
    public void ShowText(bool show)
    {
        if (show)
        {
            CancelInvoke("TimerUpdate");
            timerText.text = textToShow;
        }
        else
        {
          //  TimerUpdate();
            InvokeRepeating("TimerUpdate", 0, 0.06f);

        }
        //return show;

    }
    //private IEnumerator ShowTextInsteadOfTime(float time)
    //{
    //    CancelInvoke("TimerUpdate");
    //    timerText.text = textToShow;
    //    yield return new WaitForSecondsRealtime(time);
    //    InvokeRepeating("TimerUpdate", 0, 0.06f);
    //}
    public void SetGameOver(string text) {
        TIMER_CLASS.StopTimer();
        TIMER_CLASS.ResetTimer();
        //WaveManager.Instance.StopTimer ();
        //GameController.Instance.StopTimer ();

        timerText.color = originalColor;
        timerText.text = text;
        StopAllCoroutines();

    }

    void TimerUpdate() {

        
        timerText.text = TIMER_CLASS.GetFormattedTime();
        //		string seconds = timerText.text.Substring (6,2).Replace(timerText.text.Substring (6,2), "<size=5>" + timerText.text.Substring (6,2) + "</size>");
        //string seconds = timerText.text.Substring (6, 2);
        //timerText.text = timerText.text.Remove (6, 2);

        timerText.text = timerText.text.Remove(5, 3);
        // timerText.text = timerText.text.Insert (6, "<size=12>" + seconds + "</size>");
    }
   


	void OnEnable(){
		InvokeRepeating("TimerUpdate", 0, 0.06f);
	}
	void OnDisable(){
		CancelInvoke ("TimerUpdate");

	}
	IEnumerator OnUpdate(){

		timerText.text = textToShow;
		yield return new WaitForSeconds (0.3f);

//		TIMER_CLASS.StopTimer ();
		while (true) {
		
			timerText.text = TIMER_CLASS.GetFormattedTime ();//WaveManager.Instance.TimeToAdd(ref isDone);
			//timerText.text = GameController.Instance.TimeToAdd(ref isDone);
		
//
//			timerText.color = originalColor;
//			if (WaveManager.Instance.GetCurrentTime() <= 10f)
//				timerText.color = Color.red;
//
//			else if (WaveManager.Instance.GetCurrentTime () <= 25f)
//				timerText.color = Color.yellow;
//
//
//			if (WaveManager.Instance.GetCurrentTime () <= 0f){ //|| isDone) {
//
//				if (isShowTextWhenDone) {
//					timerText.color = originalColor;
//					timerText.text = textToShow;
//				}
//
//				if (isPauseGameWhenDone)
//					GameController.Instance.Paused = true;
//					
//				isDone = false;
//			}
//		
			yield return null;
	
		}

	
	
	}

}
