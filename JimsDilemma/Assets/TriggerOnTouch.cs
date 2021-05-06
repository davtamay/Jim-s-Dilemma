using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TriggerOnTouch : MonoBehaviour
{
    public Button thisButton;

    public void Start()
    {
        thisButton = thisButton ?? GetComponent<Button>();
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggered");
        EventSystem.current.SetSelectedGameObject(thisButton.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        thisButton.OnSelect(new BaseEventData(EventSystem.current));
    }
}
