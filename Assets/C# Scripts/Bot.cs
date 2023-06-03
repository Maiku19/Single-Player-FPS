using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

// TODO: Cache all WaitForFixedUpdate, WaitForSeconds, etc. in Coroutines (creates garbage)
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
    [SerializeField] Player _bot;

    #endregion

    #region Private

    Coroutine _aim;
    GameObject _currentTarget;
    Vector3 _lastKnownTargetPosition;
    GameObject[] _targets = new GameObject[0];
    Vector3 _destination;
    bool _searchForTargets = true;
    bool _useDynamicDifficulty = true;

    #endregion

    #endregion


    // -------------


    void Start()
    {
        // TODO: Put PlayerPrefs in a separate class
        _useDynamicDifficulty = 1 == PlayerPrefs.GetInt($"Use Dynamic Difficulty {tag}", _useDynamicDifficulty ? 1 : 0);
        difficulty = PlayerPrefs.GetFloat($"Difficulty {tag}", _useDynamicDifficulty ? 1 : 0);

        InitializeMovement();
        GetTargetPool();
    }

    void Update()
    {
        // Change destination if not targeting any player
        if (_searchForTargets) ChangeRegionWhenAtDestination();

        // Select the best target in LOS
        SelectBestTargetIfPossible(1 / (targetSelectRefreshRateOffset + targetSelectRefreshRateMultiplier * difficulty));
        ShootIfTargetInLOS(shootRefreshRateOffset + shootRefreshRateMultiplier * difficulty);

        // Update Difficulty based on player skill
        UpdateDifficulty(1);
    }


    // ------------------------------------

    #region PrivateMethods
    void GetTargetPool()
    {
        // TODO: There is a better way of doing this
        // Set targets to players of the oposite team of player
        if (transform.CompareTag("Team1")) _targets = GameObject.FindGameObjectsWithTag("Team2");
        else if (transform.CompareTag("Team2")) _targets = GameObject.FindGameObjectsWithTag("Team1");
    }

    void InitializeMovement()
    {
        _searchForTargets = true;

        // Get movement points then set random destination
        SelectRandomDestination();
        agent.destination = _destination;
    }

    IEnumerator MoveAtTarget(Transform pursuitTarget)
    {
        #region Initialization

        _searchForTargets = false;
        float pursuitTime = 0;
        stopRange = 10; // TODO: remove hardcode variable

        MoveToDestination(pursuitTarget.position);

        #endregion

        #region Target Visible Logic

        while (_currentTarget != null)
        {
            StrafeIfAble();
            StopIfInStoppingRange();

            yield return null;
        }

        #endregion

        #region Target not visible Logic

        while (_currentTarget == null && pursuitTime < maxPersuitTimeOffset + maxPersuitTimeMultiplier * difficulty)
        {
            MoveToDestination(pursuitTarget.position);

            yield return null;

            pursuitTime += Time.deltaTime;
        }

        #endregion

        #region Logic After Breaking of persuit

        _searchForTargets = true;
        stopRange = 2;

        #endregion
    }

    void StrafeIfAble()
    {
        if (difficulty < 0 /*threshold*/) { return; }

        // TODO: Add strafing here
    }

    void StopIfInStoppingRange()
    {
        if (Vector3.Distance(_destination, transform.position) <= stopRange) // TODO: Vector3.Distance() is performance heavy use math.distancesq() instead
        {
            _destination = transform.position;
            agent.destination = _destination;
        }
    }

    void MoveToDestination(Vector3 newDestination)
    {
        _destination = newDestination;
        agent.destination = _destination;

        StopIfInStoppingRange();
    }

    Vector3 _targetOffset;
    float _accuracy = 0;
    WaitForSeconds _wait = new(.1f);
    IEnumerator Aim()
    {
        _accuracy = 0;

        while (_currentTarget != null)
        {
            IncreaseAccuracy();
            AimOffset(_accuracy);
            yield return AimAtTarget();
            yield return _wait;
        }

        StopShooting();

        // TODO: Figure out WTF you've done here and fix it as this while loop looks sus
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

        _accuracy += UnityEngine.Random.Range(min, max);
        _accuracy = Mathf.Clamp(_accuracy, 0, 100);
    }

    IEnumerator AimAtTarget()
    {
        while (_currentTarget != null)
        {
            float speed = Time.deltaTime * aimSpeedMultiplier * difficulty;
            Quaternion rotation = Quaternion.LookRotation((transform.position - _currentTarget.transform.position + _targetOffset).normalized);
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
        // invert (lower = better)
        accuracy = Mathf.Clamp(accuracy, 0, 100);
        accuracy = 100 - accuracy;

        Vector3 v = V3Mul(UnityEngine.Random.insideUnitSphere, (maxInaccuracyOffset + maxInaccuracyMultiplier * difficulty));
        _targetOffset = accuracy * Vector3.Distance(_currentTarget.transform.position, transform.position) * v;


        static float GetRandomValue(float minMaxOff, float minMaxMul, float difficulty)
        {
            return UnityEngine.Random.Range
            (
                -minMaxOff - minMaxMul * difficulty,
                minMaxOff + minMaxMul * difficulty
            );
        }
        static Vector3 V3Mul(Vector3 a, Vector3 b) => new(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    float _difficultyDeltaTime = 0;
    void UpdateDifficulty(float refreshTime)
    {
        // TODO
    }

    float CalculateDifficultyDeltaBasedOnPlayerScore()
    {
        throw new NotImplementedException();
    }

    float _shootLOSCheckDeltaTime = 0;
    void ShootIfTargetInLOS(float refreshTime)
    {
        if (_currentTarget == null) { return; }
        if (_shootLOSCheckDeltaTime < refreshTime) { _shootLOSCheckDeltaTime += Time.deltaTime; return; }
        else { _shootLOSCheckDeltaTime = 0; }

        if (CheckIfInLOS(_currentTarget.transform))
        {
            StartShooting();
            StartCoroutine(MoveAtTarget(_currentTarget.transform));
        }
        else
        {
            StopShooting();
        }
    }

    float _searchDeltaTime = 0;
    void SelectBestTargetIfPossible(float searchRefreshTime)
    {
        if (_searchDeltaTime < searchRefreshTime) { _searchDeltaTime += Time.deltaTime; return; }
        else { _searchDeltaTime = 0; }

        // Loop through all targets in game
        _currentTarget = null;
        float bestDistanceSq = math.INFINITY;
        foreach (GameObject target in _targets)
        {
            // check if in LOS
            if (CheckIfInLOS(target.transform))
            {
                // Compare distance
                float distSq = math.distancesq(target.transform.position, transform.position);
                if (bestDistanceSq > distSq)
                {
                    _currentTarget = target;
                    bestDistanceSq = distSq;
                }
            }
        }

        if (_currentTarget != null) { _lastKnownTargetPosition = _currentTarget.transform.position; }
        if (_currentTarget == null) { StopShooting(); }
    }

    void ChangeRegionWhenAtDestination()
    {
        if (_destination != null && Vector3.Distance(_destination, transform.position) <= stopRange)
        {
            SelectRandomDestination();
            agent.destination = _destination;
        }
    }

    void SelectRandomDestination()
    {
        _destination = MovementPoints.Instance.GetRandomMovementPoint().position;
    }

    bool CheckIfInLOS(Transform target)
    {
        // TODO: there is a better way of doing this

        // Get angle towards target
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
        return Quaternion.Angle(rotation, cam.rotation) <= FOV * .5f;
    }

    void StartShooting()
    {
        if (gun.shoot) { return; }

        gun.shoot = true;
        _aim = StartCoroutine(Aim());
        stopRange = 10;
    }

    void StopShooting()
    {
        if (!gun.shoot) { return; }

        gun.shoot = false;
        stopRange = 2; // TODO: remove hardcoded variable
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
