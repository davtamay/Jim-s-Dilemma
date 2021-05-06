using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Custom_Ovr_Input : MonoBehaviour
{
    public UnityEvent onTriggerPressed;
    public UnityEvent onTriggerRelease;

    public UnityEvent onDelayTriggerRelease;

    public OVRInput.Controller controllerChoice;
    void Update()
    {
      
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerChoice))
                onTriggerPressed.Invoke();

        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controllerChoice))
        {
            onTriggerRelease.Invoke();
            StartCoroutine(TriggerReleaseDelay());
        }
       
    }

    public IEnumerator TriggerReleaseDelay()
    {
        yield return null;
        onDelayTriggerRelease.Invoke();
                   
    }
}
