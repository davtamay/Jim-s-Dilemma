using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InputSelection : MonoBehaviour//, IUpdatable
{
 //TODO error on handL and handR collider emiting an error on webgl builds (matrix transformation collider issues)
 //TODO animation is what is causing the line renderer to flicker not keep position??

    private LineRenderer lineRenderer;
    private Collider colliderInput;


    public string tagFlagMask_ForUIButton = "UIInteractable";


    public Vector3? newPlayerPos;
    public Vector3? newPlayerEulerRot;

    public bool isFloorDetector;
    public GameObject floorObj;

    int layerMaskIgnore = ~(1 << 2);
    int layerMaskWater = 1 << 9;

    public void Awake()
    {

        lineRenderer = GetComponent<LineRenderer>();
        colliderInput = transform.parent.GetComponentInChildren<Collider>(true);

        if (floorObj)
            floorObj.SetActive(false);
    }

    public void OnEnable()
    {
   

        colliderInput.enabled = true;
        
    }
    public void OnDisable()
    {
        if (floorObj)
            floorObj.SetActive(false);

        colliderInput.enabled = false;

        //isKeepLocation = false;
        //locationToKeep = Vector3.zero;
    }
    bool isKeepLocation;
    Vector3 locationToKeep;

    public float distanceToSendCollider = 20f;
    public void Update()
    {
        if (distanceToSendCollider == 0)
            return;

        if (colliderInput.enabled == false)
            return;

        Vector3 pos = transform.position + (transform.forward * distanceToSendCollider);
        lineRenderer.SetPosition(0, transform.position);

        lineRenderer.SetPosition(1, pos);


        colliderInput.transform.position = pos;


        if (!isFloorDetector)
        {

           

            //PLACING THIS HERE AFFECTS WORLD COLLIDER INTERACTIONS WITH LINE RENDERER?
            if (Physics.Linecast(transform.position, pos, out RaycastHit hit, layerMaskIgnore))
            {
                pos = hit.point;


                if (hit.collider.CompareTag(tagFlagMask_ForUIButton))
                {
                    colliderInput.transform.position = hit.point;
                }

            }
            //else
            //    colliderInput.transform.position = pos;

            lineRenderer.SetPosition(1, pos);

        }
        else
        {

            //COLLIDERS ON HANDL AND HANDR ARE CAUSING ERROR IN WEBGL BUILD

            //if (floorObj.activeInHierarchy)
            //    floorObj.SetActive(false);

            //LayerMask: "Walkable"
            if (Physics.Linecast(transform.position, pos, out RaycastHit hit, layerMaskWater))//, LayerMask.GetMask("Walkable"), QueryTriggerInteraction.Collide))
            {

                pos = hit.point;


                if (!floorObj.activeInHierarchy)
                    floorObj.SetActive(true);


                floorObj.transform.position = pos;

                newPlayerPos = pos;
            //    newPlayerEulerRot =

                colliderInput.transform.position = pos;

                isKeepLocation = true;
                locationToKeep = pos;



            }

            //BREAK CURRENT TELEPORTATION
            if (lineRenderer.GetPosition(1).y > 1.8f)
            {
                pos = transform.position + (transform.forward * 20);
                colliderInput.transform.position = pos;
                newPlayerPos = null;
                floorObj.SetActive(false);

                isKeepLocation = false;
                // lineRenderer.SetPosition(1, Vector3.Lerp(transform.position, pos, 0.5f));
                //  lineRenderer.SetPosition(2, pos);
            }

            //for (int i = 1; i < 4; i++)
            //{
            //    lineRenderer.SetPosition(i, Vector3.Lerp(transform.position, pos, 0.25f * i));
            //}

            if (isKeepLocation)
            {
                //   lineRenderer.SetPosition(1, Vector3.Lerp(transform.position, locationToKeep, 0.5f));

                lineRenderer.SetPosition(2, locationToKeep);
            }
            else
            {
                //  lineRenderer.SetPosition(1, Vector3.Lerp(transform.position, pos, 0.5f));


                lineRenderer.SetPosition(2, pos);
            }
        }



    }
}
