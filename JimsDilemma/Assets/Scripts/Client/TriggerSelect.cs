using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;



[System.Serializable]
public class Output_Collided_Enter_Information : UnityEvent<Collider> { }
public class Output_Collided_Exit_Information : UnityEvent<Collider> { }

public class TriggerSelect : MonoBehaviour
{
    private bool isOverButton;
    private Button currenButton;
 

    private InputSelection inputSelection;
    private string parentFlagTagMask_ForButtonUI;
    private bool isFloorDetector;
    public bool isSelectOnDisable = true;

    public Output_Collided_Enter_Information ouput_onTriggerEnter_infoevent;
    public Output_Collided_Enter_Information ouput_onTriggerExit_infoevent;
    public void Start()
    {
        inputSelection = transform.parent.GetComponent<InputSelection>();
        isFloorDetector = inputSelection.isFloorDetector;

        if (!isFloorDetector)
            parentFlagTagMask_ForButtonUI = inputSelection.tagFlagMask_ForUIButton;

    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Got");
        //if (!isFloorDetector)
        //{
        if (other.CompareTag(parentFlagTagMask_ForButtonUI))
        {
            

            ouput_onTriggerEnter_infoevent.Invoke(other);

            if (isSelectOnDisable)
            {
                currenButton = other.GetComponent<Button>();
                currenButton.Select();

                isOverButton = true;
            }
        

               

            }
       // }
    }
 
    public void OnTriggerExit(Collider other)
    {
        ouput_onTriggerExit_infoevent.Invoke(other);

        if (!isFloorDetector)
        {

            isOverButton = false;


            if (currenButton)
            {

               

                EventSystem.current.SetSelectedGameObject(null);// = null;
                
              
                currenButton = null;

            }
        }
    }

    //THIS IS WHERE FUNCTIONS ARE INVOKED (ON RELEASE OF TRIGGER BUTTON WHICH DEACTIVATES PARENT OBJECT
    public void OnDisable()
    {
        if (isSelectOnDisable)
        {
            if (!isFloorDetector)
            {

                if (isOverButton && currenButton)
                {
                    currenButton.onClick.Invoke();

                }

                isOverButton = false;
                currenButton = null;

            }
        }
    }



}
