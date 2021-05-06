using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class WackSpawner : MonoBehaviour {


	private Transform closestBush;

	[SerializeField] private Vector2 initXZPosOffsetMinLimits;
	[SerializeField] private Vector2 initXZPosOffsetMaxLimits;

	//[SerializeField] private float initPosRandomOffsetMinLimits;
	//[SerializeField] private float initPosRandomOffsetMaxLimits;

	[SerializeField] private float speed;
	[SerializeField] private float distanceBushOffset;
	[SerializeField] private float timeUntilOneLessBerry;

	private Transform playerTransform;

	private Transform rootMoveTransform;
	private Animator thisAnimator;

    [Header("Navigation_Controlls")]
    [SerializeField] bool isUseNavCont = false;
    private NavMeshAgent thisNavCont;
   

    [SerializeField] private Transform _NC_parentSpawnLocations;
    private List<Transform> _NC_spawnlocations;

	
	[SerializeField]private Vector3 initPos;
	[SerializeField]private bool isUseParentRootTransform;

	[Tooltip("For FLying Creatures that use the y coordinate system")]
	[SerializeField]private bool isAllowYVector;
	[SerializeField]private float YInitStart;


	//[SerializeField]private bool isStable;


	void Awake(){

		if(isUseParentRootTransform)
			rootMoveTransform = transform.parent;
		else
			rootMoveTransform = transform;

		thisAnimator = GetComponentInChildren<Animator> ();
		playerTransform = GameObject.FindWithTag ("Player").transform;

        if(isUseNavCont)
            thisNavCont = GetComponentInChildren<NavMeshAgent>(true);

    }
	//10/28/17 changed from onenable to prevent errors
	void Start(){

		initPos = rootMoveTransform.position;

		SetRandomPos ();



        if (isUseNavCont)
        {
            _NC_spawnlocations = new List<Transform>();

            foreach (Transform loc in _NC_parentSpawnLocations)
                _NC_spawnlocations.Add(loc);

            //if (!thisNavCont.isOnNavMesh) {
            //    thisNavCont.nav
            //        }
        }
            

	}

	/*public void ChooseAndSeekClosestBush(){

		
		float closestBushDistance = Mathf.Infinity;

		foreach (Transform bs in WackGameManager.Instance.totalBranches) {

			if (Vector3.Distance (thisTransform.position, bs.position) < closestBushDistance) {

				if (!WackGameManager.Instance.BranchHasBerries (bs))
					continue;

				closestBush = bs;
				closestBushDistance = Vector3.Distance (thisTransform.position, bs.position);

			}

		}
		StartCoroutine (SeekBush ());
	
	}*/

	Vector3 dir;
	IEnumerator SeekBush(){

		while(true){

            if (!closestBush)
                closestBush = WackGameManager.Instance.GetRandomBush();//.totalBranches [0];
				
			if(thisAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Idle")){
				yield return null;
				continue;
			}

			yield return null;


            if (!isUseNavCont)
            {
                dir = closestBush.position - rootMoveTransform.position;

                if (!isAllowYVector)
                    dir.y = 0;

                rootMoveTransform.position += dir.normalized * Time.deltaTime * speed;
                rootMoveTransform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }
            else
            {
                thisNavCont.SetDestination(closestBush.position);
            }

				if (Vector3.Distance (rootMoveTransform.position, closestBush.position) < distanceBushOffset) {
					StartCoroutine (EatFruit ());
					thisAnimator.SetTrigger("IsEating");
					break;
				}

			

		}
	
	}
	private float timer;
	IEnumerator EatFruit(){

		//Transform oldBush;
		timer = 0;


	//	if (thisAnimator.GetBool ("IsDead"))
	//		StopAllCoroutines ();


		while(true){

			yield return null;
			timer += Time.deltaTime;

			if (timer > timeUntilOneLessBerry) {
				
				WackGameManager.Instance.ReduceBerry (closestBush);
			//	oldBush = closestBush;
         //   if()
				closestBush = WackGameManager.Instance.GetClosestBush (rootMoveTransform);

			/*	if (oldBush.GetInstanceID() != closestBush.GetInstanceID())
				if(thisAnimator.HasParameter ("IsWalk"))
					thisAnimator.SetTrigger("IsWalk");*/
		        if(gameObject.activeInHierarchy)
				StartCoroutine (SeekBush());
				break;
			}

		}

	}
	public void SetRandomPos(){

		//thisCollider.enabled = true;

		StopAllCoroutines ();

		float randomX = Random.Range(initXZPosOffsetMinLimits.x, initXZPosOffsetMaxLimits.x);
		float randomZ = Random.Range(initXZPosOffsetMinLimits.y, initXZPosOffsetMaxLimits.y);

		Vector3 initTo = Vector3.zero;
		if (!isAllowYVector) {
			initTo = WackGameManager.Instance.centerPos.position + new Vector3 (randomX, 0, randomZ);
			initTo.y = transform.position.y;
		} else {
			initTo = WackGameManager.Instance.centerPos.position + new Vector3 (randomX, 0, randomZ);
			initTo.y = YInitStart;
		
		}

		rootMoveTransform.position = initTo;

        if (gameObject.activeInHierarchy)
            StartCoroutine (SeekBush ());

	}

}
