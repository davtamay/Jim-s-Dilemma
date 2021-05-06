// Gaze Input Module by Peter Koch <peterept@gmail.com>
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DirectionInputModule : PointerInputModule
{
    public enum Mode { Click = 0, OBJECTReference };
    public Mode mode;

    [Header("Direction")]
    public GameObject directionReference;



    public string ClickInputName = "Submit";
    [Header("Gaze Settings")]
    public static float GazeTimeInSeconds = 2f;

    private PointerEventData pointerEventData;
    private GameObject currentLookAtHandler;
    private float currentLookAtHandlerClickTime;

    public override void Process()
    {
        HandleLook();
        HandleSelection();
    }

    void HandleLook()
    {
        //if (!directionReference.activeInHierarchy)
        //    return;

        

        if (pointerEventData == null)
        {
            pointerEventData = new PointerEventData(eventSystem);
        }


        pointerEventData.position = directionReference.transform.position;
        

        pointerEventData.delta = Vector2.zero;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, raycastResults);
        pointerEventData.pointerCurrentRaycast = FindFirstRaycast(raycastResults);
        ProcessMove(pointerEventData);
    }

    void HandleSelection()
    {

        if (pointerEventData.pointerEnter != null)
        {

            // if the ui receiver has changed, reset the gaze delay timer
            GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerEventData.pointerEnter);
            if (currentLookAtHandler != handler)
            {

                currentLookAtHandler = handler;
                currentLookAtHandlerClickTime = Time.realtimeSinceStartup + GazeTimeInSeconds;
                eventSystem.SetSelectedGameObject(currentLookAtHandler, pointerEventData);
                //pointerEventData.selectedObject = currentLookAtHandler;
            }

            // if we have a handler and it's time to click, do it now
            if (currentLookAtHandler != null &&
                (mode == Mode.OBJECTReference && Time.realtimeSinceStartup > currentLookAtHandlerClickTime) ||
                (mode == Mode.Click && Input.GetButtonDown(ClickInputName)))
            {
                //	ExecuteEvents.Execute(currentLookAtHandler, pointerEventData, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.ExecuteHierarchy(currentLookAtHandler, pointerEventData, ExecuteEvents.pointerClickHandler);
                currentLookAtHandlerClickTime = float.MaxValue;
            }
        }
        else
        {
            currentLookAtHandler = null;
        }
    }
}