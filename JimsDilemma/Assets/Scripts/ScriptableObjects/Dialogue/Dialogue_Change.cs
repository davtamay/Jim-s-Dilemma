using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "Dialogue",  menuName = "CustomSO/Dialogue")]
public class Dialogue_Change : ScriptableObject {

    // [Multiline]
    //public string[] discusion_Main_Text;
    //public string[] discussion_text_Option_1;
    //public string[] discussion_text_Option_2;
    // public int[] answer_key;
    //public bool[] isOptionsAvailable;



    // [Space]
    [Header("Main Dialogue Set")]
   // public bool hasTextOptionsPresent;
    public Dialogue_Data[] dialogueDataMessages;
    [Space]
    [Header ("Set Response Event and Settings")]
    [Multiline]
    public string correctMessage;
    public UnityEvent onCompleteDialogue;
    [Multiline]
    public string inCorrectMessage;
    public UnityEvent onIncompleteDialogue;
    
    public bool isComplete;


   // public bool hasOptionsPresent;
    //public Dialogue_Data[] dialogueDataMessages;
}
public enum Response{CORRECT = 1, WRONG = 0, INDIFERENT = -1 }
[System.Serializable]
public class Dialogue_Data
{
    public string description;

    [Multiline]
    public string mainText;
    [Space]
    public bool isOptionChoiceAvailable;
    [Multiline]
    public string discussion_text_Option1;
    [Multiline]
    public string discussion_text_Option2;
    public int CorrectOption = 1;
    //0 for wrong //1 for right //2 for third option?
 //public Response selectionResponse = Response.CORRECT;
    //public int answer_Key = 1;

    [Space]
    public UnityEvent onSelectOption;
    [Space]

    public bool isFinishDialogue = false;

    //public Dialogue_Data()
    //{
        
    //    answer_Key = 1;// (int)selectionResponse;
    //}

    public bool isComplete;



}
