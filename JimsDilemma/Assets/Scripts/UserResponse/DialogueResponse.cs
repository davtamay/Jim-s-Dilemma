using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DialogueResponse : MonoBehaviour
{

    [Header("Initial Interaction References")]
    [SerializeField] private LookInteraction initialSpeakInteraction;
    [SerializeField] private bool isUnscaledTime = true;


    [Header("Global Set Settings")]
    //[SerializeField] private bool hasTextOptions = true;
    [SerializeField] private float responseDelay;
    [Space]
    [SerializeField] private bool isWrong = false;
    [SerializeField] private string wrongMessage;
    [SerializeField] private UnityEvent onInCompleteDialogue;
    [Space]
    [SerializeField] private bool isCompleted = false;
    [SerializeField] private bool isResetCompletedAfterDialogue = false;
    [SerializeField] private string correctMessage;
    [SerializeField] private UnityEvent onCompleteDialogue;
    [Space]

    [Space]
    public Dialogue_Data[] currentDialougeMessageSet;
    [Space]

    [Header("Main Dialogue References")]
    [SerializeField] private Dialogue_Change mainDialogueToShow;
    [SerializeField] private GameObject mainInteraction;
    [SerializeField] private Text mainText;
   // [Multiline] [SerializeField] private string[] mainDiscussion;

    [Space]

    [Header("Dialogue Options References")]
   // [SerializeField] private bool hasOptions = true;

    [Space]

    [SerializeField] private LookInteraction continueInteraction_Option;
    [SerializeField] private LookInteraction lookInteraction_Option1;
    private Text textOption1;
   // [Multiline] [SerializeField] private string[] discussionOptions1;

    [Space]

    [SerializeField] private LookInteraction lookInteraction_Option2;
    private Text textOption2;
   // [Multiline] [SerializeField] private string[] discussionOptions2;

  //  [Header("Response Result")]
  
    //[SerializeField] private int[] answerKey;
   // [SerializeField] private bool[] ToggleAppearingOptionsInLevel;

    [Header("Stressed References")]
    [SerializeField] private int resilienceNeededForHelping;
    [SerializeField] private Dialogue_Change dialogueToShowOnStressed;
    [SerializeField] private bool isStressed;
    [SerializeField] private UnityEvent onStressedDialogue;

    [Space]
    [Header("Debug.Info")]
    public int level;
    [Space]

    [Header("References")]
    [SerializeField] private DataManager DATA_MANAGER;



    // public enum DialogueOptions {First_Option_Continue, Second_Option}
    //[SerializeField] private DialogueOptions[] answerKey2;

    private Animator mainInteractionAnimator;
    private int isActiveHash = Animator.StringToHash("IsActive");


    //[SerializeField] private LocalizationManager LOCALIZATION_MANAGER;

    public void Start()
    {

        mainInteractionAnimator = mainInteraction.GetComponent<Animator>();

        if (mainText == null)
            mainText = mainInteraction.GetComponentInChildren<Text>();

        //mainText.text = mainDiscussion[level];
        mainText.text = currentDialougeMessageSet[level].mainText;


        mainInteractionAnimator.SetBool(isActiveHash, true);
        mainInteractionAnimator.SetBool(isActiveHash, false);

        if (lookInteraction_Option1 != null)
        {
            textOption1 = lookInteraction_Option1.GetComponentInChildren<Text>(true);
            // textOption1.text = discussionOptions1[level];

            textOption1.text = currentDialougeMessageSet[level].discussion_text_Option1;
        }

        if (lookInteraction_Option2 != null)
        {
            textOption2 = lookInteraction_Option2.GetComponentInChildren<Text>(true);
            //textOption2.text = discussionOptions2[level];

            textOption2.text = currentDialougeMessageSet[level].discussion_text_Option2;
        }


    }

    public void TurnOnDialogue()
    {
        isStressed = DATA_MANAGER.playerData.playerResilienceHealth.resilienceHealth < resilienceNeededForHelping;

        if (isStressed)
            ChangeDisplayedText(dialogueToShowOnStressed);
        else
        {
            if (mainDialogueToShow != null)
                ChangeDisplayedText(mainDialogueToShow);

        }

        if (initialSpeakInteraction)
            initialSpeakInteraction.canAppear = false;


        if (isCompleted)
        {
            level = currentDialougeMessageSet.Length - 1;
            mainText.text = correctMessage;

            //mainInteractionAnimator.SetBool(isActiveHash, true);

            StartCoroutine(ChangeOptions(false));
           // mainInteractionAnimator.SetBool(isActiveHash, false);
            // TurnOffDialogue(true);
            return;
            // Invoke("TurnOffDialogue(true)", responseDelay);



        }
        else
        {
            mainText.text = currentDialougeMessageSet[level].mainText;


            if (lookInteraction_Option1 != null)
                textOption1.text = currentDialougeMessageSet[level].discussion_text_Option1;

            if (lookInteraction_Option2 != null)
                textOption2.text = currentDialougeMessageSet[level].discussion_text_Option2;


            if (currentDialougeMessageSet[level].isOptionChoiceAvailable)
            {

                lookInteraction_Option1.TriggerSelections();

                if (lookInteraction_Option2 != null)
                    lookInteraction_Option2.TriggerSelections();
            }
            else
            {
                if (continueInteraction_Option != null)
                    continueInteraction_Option.TriggerSelections();

            }
        }
         mainInteractionAnimator.SetBool(isActiveHash, true);
    }

    public void TurnOffDialogue(bool keepLevel = false)
    {
        if (initialSpeakInteraction)
        {
            initialSpeakInteraction.canAppear = true;
            initialSpeakInteraction.RemoveSelections();

        }
        if (!isCompleted && !keepLevel)
            level = 0;


        if (isCompleted && !isStressed)
            onCompleteDialogue.Invoke();



        if (!mainInteractionAnimator)
            mainInteractionAnimator = mainInteraction.GetComponent<Animator>();

        mainInteractionAnimator.SetBool(isActiveHash, false);

        if (lookInteraction_Option1 != null)
            lookInteraction_Option1.RemoveSelections();

        if (continueInteraction_Option != null)
            continueInteraction_Option.RemoveSelections();

        if (lookInteraction_Option2 != null)
            lookInteraction_Option2.RemoveSelections();

    }

    public void ChangeDisplayedText(Dialogue_Change dialogueChange)
    {
        // onCompleteDialogue.

        TurnOffDialogue();
        level = 0;

        // var newLength = dialogueChange.discusion_Main_Text.Length;
        var newLength = dialogueChange.dialogueDataMessages.Length;

        currentDialougeMessageSet = new Dialogue_Data[newLength];


        //mainDiscussion = new string[newLength];
        //discussionOptions1 = new string[newLength];
        //discussionOptions2 = new string[newLength];
        //ToggleAppearingOptionsInLevel = new bool[newLength];
      //  answerKey = new int[newLength];
        //answerKey2 = new DialogueOptions[newLength];

        // for (int i = 0; i < dialogueChange.discusion_Main_Text.Length; i++)
        for (int i = 0; i < newLength; i++)
        {
            // mainDiscussion[i] = dialogueChange.discusion_Main_Text[i];
            currentDialougeMessageSet[i].mainText = dialogueChange.dialogueDataMessages[i].mainText;
         
            currentDialougeMessageSet[i].discussion_text_Option1 = dialogueChange.dialogueDataMessages[i].discussion_text_Option1;
           // discussionOptions1[i] = dialogueChange.dialogueDataMessages[i].discussion_text_Option1;

            if (lookInteraction_Option2 != null)
                // discussionOptions2[i] = dialogueChange.discussion_text_Option_2[i];
               currentDialougeMessageSet[i].discussion_text_Option2 = dialogueChange.dialogueDataMessages[i].discussion_text_Option2;


            // ToggleAppearingOptionsInLevel[i] = dialogueChange.isOptionsAvailable[i];
           // ToggleAppearingOptionsInLevel[i]
             currentDialougeMessageSet[i].isOptionChoiceAvailable = dialogueChange.dialogueDataMessages[i].isOptionChoiceAvailable;

            //answerKey[i] = dialogueChange.answer_key[i];
         //   answerKey[i] = dialogueChange.dialogueDataMessages[i].answer_Key;

        }
      
        correctMessage = dialogueChange.correctMessage;
        wrongMessage = dialogueChange.inCorrectMessage;
        this.isCompleted = dialogueChange.isComplete;


    }

    public IEnumerator ChangeOptions(bool isLastDialogueDelay = true)
    {
        #region TURN_EVERYTHING_OFF
        mainInteractionAnimator.SetBool(isActiveHash, false);

        if (continueInteraction_Option != null)
        {
            continueInteraction_Option.image.raycastTarget = false;
            continueInteraction_Option.RemoveSelections();
        }
        if (lookInteraction_Option1 != null)
        {
            lookInteraction_Option1.image.raycastTarget = false;
            lookInteraction_Option1.RemoveSelections();
        }

        if (lookInteraction_Option2 != null)
        {
            lookInteraction_Option2.image.raycastTarget = false;
            lookInteraction_Option2.RemoveSelections();
        }

        #endregion

        #region SETUP_COMPLETED_OR_WRONG_OR_STRESSED_MESSAGE


        if (isCompleted || isWrong || isStressed)
        {
            if (isLastDialogueDelay)
            {
                if (isUnscaledTime)
                    yield return new WaitForSecondsRealtime(responseDelay);
                else
                    yield return new WaitForSeconds(responseDelay);

            }

            if (isCompleted)
            {
                mainText.text = correctMessage;
              //  mainInteractionAnimator.SetBool(isActiveHash, true);
                // onCompleteDialogue.Invoke();
            }
            else if (isWrong)
            {
                mainText.text = wrongMessage;
                isWrong = false;

            }
            else if (isStressed)
            {

                onStressedDialogue.Invoke();
                level = 0;

                if (initialSpeakInteraction)
                    initialSpeakInteraction.canAppear = true;

               /// continueInteraction_Option.image.raycastTarget = true;


                //yield break;

            }

            mainInteractionAnimator.SetBool(isActiveHash, true);

            if (isUnscaledTime)
                yield return new WaitForSecondsRealtime(responseDelay);
            else;
               yield return new WaitForSeconds(responseDelay);

            mainInteractionAnimator.SetBool(isActiveHash, false);


            //if (isCompleted)
            //    onCompleteDialogue.Invoke();

            if (initialSpeakInteraction)
                initialSpeakInteraction.canAppear = true;

            if (isResetCompletedAfterDialogue)
            {
                level = 0;
                isCompleted = false;
            }

            if(lookInteraction_Option1 != null)
                lookInteraction_Option1.image.raycastTarget = true;
            if (lookInteraction_Option2 != null)
                lookInteraction_Option2.image.raycastTarget = true;
            if(continueInteraction_Option != null)
                continueInteraction_Option.image.raycastTarget = true;

            yield break;


        }
        #endregion

        #region TURN_OPTIONS_ON

        // Debug.Log("THIS IS HAPPENING::::::::::::::::::::::::::");
        if (isUnscaledTime)
            yield return new WaitForSecondsRealtime(responseDelay);
        else
            yield return new WaitForSeconds(responseDelay);


        if (currentDialougeMessageSet[level].isOptionChoiceAvailable)
        {

            lookInteraction_Option1.image.raycastTarget = true;
            textOption1.text = currentDialougeMessageSet[level].discussion_text_Option1;
            lookInteraction_Option1.TriggerSelections();


            if (lookInteraction_Option2 != null)
            {
                lookInteraction_Option2.image.raycastTarget = true;

                textOption2.text = currentDialougeMessageSet[level].discussion_text_Option2;

                lookInteraction_Option2.TriggerSelections();
            }

        }
        else
        {

            if (continueInteraction_Option != null)
            {
                continueInteraction_Option.image.raycastTarget = true;
                continueInteraction_Option.TriggerSelections();

            }
        }

        #endregion

        #region SETUP_TURNON_MAININTERACTION

        mainInteractionAnimator.SetBool(isActiveHash, true);

        mainText.text = currentDialougeMessageSet[level].mainText;


        #endregion

    }
    //public void CheckSelected(DialogueOptions dO) { }
    //1 is true //0 is false
    public void CheckSelectedOptions(int value)
    {

        currentDialougeMessageSet[level].onSelectOption.Invoke();
        
      if(currentDialougeMessageSet[level].CorrectOption == value)
        {
            ++level;

            // if (answerKey.Length == level && !isStressed)
            if (currentDialougeMessageSet.Length == level && !isStressed)
            {
                isCompleted = true;

            }

        }
        else
        {

            level = 0;

            isWrong = true;

            onInCompleteDialogue.Invoke();
        }

        StartCoroutine(ChangeOptions());

    }

    public void AddLevel(int level)
    {
        currentDialougeMessageSet[this.level].onSelectOption.Invoke();

        this.level += level;
      //  currentDialougeMessageSet[level].onSelectOption.Invoke();

        if (currentDialougeMessageSet.Length == this.level)
                isCompleted = true;


      //  TurnOnDialogue();
    }


}
