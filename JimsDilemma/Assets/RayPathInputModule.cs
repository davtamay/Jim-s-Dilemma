using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IOnRayPathTarget : IEventSystemHandler
{
    void OnPath();
}

public class RayPathInputModule : BaseInputModule
{
    [Tooltip("Starting Direction is Z+ on the object")]
    public Transform directionToStartPath;

    public LineRenderer lineRenderer;

    public Camera controllerCameraRay;

    public override void ActivateModule()
    {
        base.ActivateModule();

        //lineRenderer = directionToStartPath.GetComponent<LineRenderer>();
    }

    public override void Process()
    {
        //lineRenderer.SetPosition(0, directionToStartPath.position);
        // lineRenderer.SetPosition(1, directionToStartPath.position + Vector3.forward * 5);

       

        PointerEventData pointer = new PointerEventData(eventSystem);
        pointer.position = new Vector2( controllerCameraRay.pixelWidth / 2,
        controllerCameraRay.scaledPixelHeight / 2);//Screen.width / 2, Screen.height / 2);// Input.mousePosition;
        


        List<RaycastResult> raycastResult = new List<RaycastResult> ();

        eventSystem.RaycastAll(pointer, raycastResult);

        //var ray = new Ray(directionToStartPath.position, directionToStartPath.position + (directionToStartPath.forward * 10));



        pointer.pointerCurrentRaycast = FindFirstRaycast(raycastResult);

        //var hitPoint = ray.GetPoint(pointer.pointerCurrentRaycast.distance);

        // lineRenderer.SetPosition(1, hitPoint);

      Debug.Log(raycastResult.Count);

        var pathHandle = ExecuteEvents.GetEventHandler<IOnRayPathTarget>(pointer.pointerCurrentRaycast.gameObject);
        var enterHandle = ExecuteEvents.GetEventHandler<IPointerEnterHandler>(pointer.pointerCurrentRaycast.gameObject);
        //    
       // pointer.pointerEnter = enterHandle;
       // Debug.Log(pointer.pointerEnter);



        if (pathHandle != null)
        {
            
          //  HandlePointerExitAndEnter(pointer, enterHandle);
            //Debug.Log(pointer.used);
            //ExecuteEvents.Execute<IPointerEnterHandler>(enterHandle, GetBaseEventData(), ExecuteEvents.pointerEnterHandler);

            pointer.Use();
          //  Debug.Log(pointer.used);

            ExecuteEvents.Execute<IOnRayPathTarget>(pathHandle, GetBaseEventData(), (x, y) => { x.OnPath(); });
        }
        //foreach (var result in raycastResult)
        //{
        //    lineRenderer.SetPosition(1, result.worldPosition);
        //    var pathHandle = ExecuteEvents.GetEventHandler<IOnRayPathTarget>(result.gameObject);
        //    eventSystem.SetSelectedGameObject(result.gameObject, pointer);

        //    ExecuteEvents.Execute<IOnRayPathTarget>(pathHandle, GetBaseEventData(), (x, y) => { x.OnPath(); });

        //}


    }
}
