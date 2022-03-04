using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    #region Variables

    #region Editor

    public float difficulty = 10;

    [Header("Difficulty Multipliers Options")]
    [SerializeField] float FOV;

    [Space(10)]
    [SerializeField] float shootRefreshRateMultiplier = .02f;
    [SerializeField] float shootRefreshRateOffset = .02f;

    [Space(10)]
    [SerializeField] float targetSelectRefreshRateMultiplier = .01f;
    [SerializeField] float targetSelectRefreshRateOffset = .01f;

    [Space(10)]
    [SerializeField] Vector3 maxInaccuracyMultiplier = Vector3.one;
    [SerializeField] Vector3 maxInaccuracyOffset = Vector3.one;

    [Space(10)]
    [SerializeField] float accuracyIncrementMultiplier = 1;
    [SerializeField] float accuracyIncrementOffset = 1;
    [SerializeField] float minAccuracyIncrementDifferenceMultiplier = 1;

    [Space(10)]
    [SerializeField] float aimSpeedMultiplier = 10;

    [Space(10)]
    [SerializeField] float maxPersuitTimeMultiplier = 1;
    [SerializeField] float maxPersuitTimeOffset = 0;



    [Header("Options")]
    public float stopRange;
    public float visionRange;

    [Header("References")]
    public Transform viewPoint;
    public Transform cam;
    public NavMeshAgent agent;
    public Gun gun;

    #endregion

    #region Private

    Coroutine aim;
    GameObject currentTarget;
    Vector3 lastKnownTargetPosition;
    GameObject[] targets = new GameObject[0];
    Vector3 destination;
    GameObject[] movementPoints = new GameObject[0];
    bool searchForTargets = true;

    #endregion

    #endregion


    // -------------


    void Start()
    {
        GetMovementPoints();
        InitializeMovement();
        GetTargetPool();
    }

    void Update()
    {
        // Change destination if not targeting any player
        if (searchForTargets) ChangeRegionWhenAtDestination();

        // Select the best target in LOS
        SelectBestTargetIfPosible(targetSelectRefreshRateOffset + targetSelectRefreshRateMultiplier * difficulty);
        ShootIfTargetInLOS(shootRefreshRateOffset + shootRefreshRateMultiplier * difficulty);

        // Update Difficulty based on player skill
        UpdateDifficulty(1);
    }


    // ------------------------------------

    #region PrivateMethods
    void GetTargetPool()
    {
        // Set targets to players of the oposit team of player
        if (transform.CompareTag("Team1")) targets = GameObject.FindGameObjectsWithTag("Team2");
        else if (transform.CompareTag("Team2")) targets = GameObject.FindGameObjectsWithTag("Team1");
    }

    void InitializeMovement()
    {
        searchForTargets = true;

        // Get movement points then set random destination
        GetMovementPoints();
        destination = movementPoints[Random.Range(0, movementPoints.Length)].transform.position;
        agent.destination = destination;
    }

    IEnumerator MoveAtTarget(Transform persuitTarget)
    {
        #region Initialization

        searchForTargets = false;
        float persuitTime = 0;
        stopRange = 10;

        MoveToDestination(persuitTarget.position);

        #endregion

        #region Target Visible Logic

        while (currentTarget != null)
        {
            StrafeIfAble();
            StopIfInStopingRange();

            yield return null;
        }

        #endregion

        #region Target not visible Logic

        while (currentTarget == null && persuitTime < maxPersuitTimeOffset + maxPersuitTimeMultiplier * difficulty)
        {
            MoveToDestination(persuitTarget.position);

            yield return null;

            persuitTime += Time.deltaTime;
        }

        #endregion

        #region Logic After Breaking of persuit

        searchForTargets = true;
        stopRange = 2;

        #endregion
    }

    void StrafeIfAble()
    {
        if (difficulty < 0 /*threshold*/) { return; }

        // TODO
    }

    void StopIfInStopingRange()
    {
        if (Vector3.Distance(destination, transform.position) <= stopRange)
        {
            destination = transform.position;
            agent.destination = destination;
        }
    }

    void MoveToDestination(Vector3 newDestination)
    {
        destination = newDestination;
        agent.destination = destination;

        StopIfInStopingRange();
    }

    Vector3 targetOffset;
    float accuracy = 0;
    IEnumerator Aim()
    {
        accuracy = 0;

        while (currentTarget != null)
        {
            IncreaseAccuracy();
            AimOffset(accuracy);
            yield return AimAtTarget();
            yield return new WaitForSeconds(0.1f);
        }

        StopShooting();

        while (cam.localRotation != Quaternion.Euler(0, 0, 0))
        {
            cam.localRotation = Quaternion.RotateTowards(cam.localRotation, Quaternion.Euler(0,0,0), 36);
        }
    }

    void IncreaseAccuracy()
    {
        // Increments accuracy randomly 

        float max = accuracyIncrementOffset + accuracyIncrementMultiplier * difficulty;
        float min = max - minAccuracyIncrementDifferenceMultiplier / difficulty;

        accuracy += Random.Range(min, max);
        accuracy = Mathf.Clamp(accuracy, 0, 100);
    }

    IEnumerator AimAtTarget()
    {
        while (currentTarget != null)
        {
            float speed = Time.deltaTime * aimSpeedMultiplier * difficulty;
            Quaternion rotation = Quaternion.LookRotation((transform.position - currentTarget.transform.position + targetOffset).normalized);
            // Y axis for up/down mouse movement, X axis rotates up/down (camera)

            Quaternion lastCamRot = cam.rotation;
            rotation.eulerAngles += new Vector3(0, 180, 0);
            rotation.eulerAngles = new Vector3(rotation.eulerAngles.x * -1, rotation.eulerAngles.y);
            Quaternion newRot = Quaternion.RotateTowards(cam.rotation, rotation, speed);
            cam.rotation = newRot;


            // Break loop if aiming at the same spot
            if (cam.rotation == lastCamRot) { break; }

            yield return null;
        }
    }

    void AimOffset(float accuracy)
    {
        // Conversion (lower = better)
        accuracy = Mathf.Clamp(accuracy, 0, 100);
        accuracy = 100 - accuracy;

        float xMin = -maxInaccuracyOffset.x + maxInaccuracyMultiplier.x * difficulty * -1;
        float xMax = maxInaccuracyOffset.x + maxInaccuracyMultiplier.x * difficulty;
        float x = Random.Range(xMin, xMax);

        float yMin = -maxInaccuracyOffset.y + maxInaccuracyMultiplier.y * difficulty * -1;
        float yMax = maxInaccuracyOffset.y + maxInaccuracyMultiplier.y * difficulty;
        float y = Random.Range(yMin, yMax);

        float zMin = -maxInaccuracyOffset.z + maxInaccuracyMultiplier.z * difficulty * -1;
        float zMax = maxInaccuracyOffset.z + maxInaccuracyMultiplier.z * difficulty;
        float z = Random.Range(zMin, zMax);

        targetOffset = new Vector3(x, y, z) * Vector3.Distance(currentTarget.transform.position, transform.position) * accuracy;
    }

    float difficultyDeltaTime = 0;
    void UpdateDifficulty(float refreshTime)
    {
        // TODO
    }

    float shootLOSCheckDeltaTime = 0;
    void ShootIfTargetInLOS(float refreshTime)
    {
        if (currentTarget == null) { return; }
        if (shootLOSCheckDeltaTime < refreshTime) { shootLOSCheckDeltaTime += Time.deltaTime; return; }
        else { shootLOSCheckDeltaTime = 0; }

        if (CheckIfInLOS(currentTarget.transform))
        {
            StartShooting();
            StartCoroutine(MoveAtTarget(currentTarget.transform));
        }
        else
        {
            StopShooting();
        }
    }

    float searchDeltaTime = 0;
    void SelectBestTargetIfPosible(float searchRefreshTime)
    {
        if (searchDeltaTime < searchRefreshTime) { searchDeltaTime += Time.deltaTime; return; }
        else { searchDeltaTime = 0; }

        // Loop through all targets in game
        currentTarget = null;
        float bestDist = 9999999;
        foreach (GameObject target in targets)
        {
            // check if in LOS
            if (CheckIfInLOS(target.transform))
            {
                // Compare distance
                float dist = Vector3.Distance(target.transform.position, transform.position);
                if (bestDist > dist)
                {
                    currentTarget = target;
                    bestDist = dist;
                }
            }
        }

        if (currentTarget != null) { lastKnownTargetPosition = currentTarget.transform.position; }
        if (currentTarget == null) { StopShooting(); }
    }

    void ChangeRegionWhenAtDestination()
    {
        if (destination != null && Vector3.Distance(destination, transform.position) <= stopRange)
        {
            SelectRandomDestination();
            agent.destination = destination;
        }
    }

    void SelectRandomDestination()
    {
        destination = movementPoints[Random.Range(0, movementPoints.Length)].transform.position;
    }

    void GetMovementPoints()
    {
        movementPoints = GameObject.FindGameObjectsWithTag("MovementPoint");
    }

    bool CheckIfInLOS(Transform target)
    {
        // Get angle twards target
        float angle = Mike.MikeRotation.Vector2ToAngle(transform.position.x - target.position.x, transform.position.z - target.position.z).eulerAngles.z;

        if (CheckIfInFOV(Quaternion.AngleAxis(angle, Vector2.up)))
        {
            // shoot ray 
            Vector3 origin = viewPoint.position;
            Vector3 direction = (target.position - origin).normalized;

            // Check if hit something
            if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, visionRange))
            {
                // Check if hit target
                if (hitInfo.transform == target.transform)
                {
                    return true;
                }
            }
        }

        // Return false if target was not hit
        return false;
    }

    bool CheckIfInFOV(Quaternion rotation)
    {
        return Quaternion.Angle(rotation, cam.rotation) <= FOV / 2;
    }

    void StartShooting()
    {
        if (gun.shoot) { return; }

        gun.shoot = true;
        aim = StartCoroutine(Aim());
        stopRange = 10;
    }

    void StopShooting()
    {
        if (!gun.shoot) { return; }

        gun.shoot = false;
        stopRange = 2;
    }
    #endregion

    #region Public methods

    public void OnDeath()
    {
        StopShooting();
        StopAllCoroutines();
    }
    
    public void OnRespawn()
    {
        InitializeMovement();
    }

    #endregion
}
