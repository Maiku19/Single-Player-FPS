using Mike;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [System.Serializable]
    public struct AnimationTrigger
    {
        public string name;
        public float chanceWeight;
    }


    [HideInInspector] public bool shoot = false;

    [SerializeField] private bool useMouseInput = false;
    [SerializeField] private bool automatic = false;
    [SerializeField] private float damage = 10;
    [SerializeField] private float shotDelay = .1f;
    [SerializeField] private float reloadTime = 2;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private float maxRange = 100;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private bool useAmmoCounter = false;

    [Header("MuzzleFlash")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private ParticleSystem[] muzzleFlashesToPlay;

    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationTrigger[] fireAnimations;
    [SerializeField] private AnimationTrigger[] reloadAnimations;

    [HideInInspector] public int currentAmmo;
    float shotDelta = 0;
    Transform viewPoint;

    void Start()
    {
        Initialize();

        
    }

    void Initialize()
    {
        currentAmmo = maxAmmo;
        if (useAmmoCounter) { AmmoCounter.SetMaxAmmo(maxAmmo); AmmoCounter.SetAmmo(currentAmmo); }

        viewPoint = transform;
        while (true)
        {
            if (viewPoint.parent != null) viewPoint = viewPoint.parent;
            else { break; }
            if (viewPoint.tag == "Camera" || viewPoint.tag == "MainCamera") break;
        }
        if (viewPoint == null) { Debug.LogError("Gun must find viewPoint, else it won't work"); }
    }

    void Update()
    {
        ShootIfAble();
    }

    // prevents reload coroutine from running multiple times
    [HideInInspector] public bool reloading = false;

    IEnumerator Reload()
    {
        reloading = true;
        PlayReloadAnimation();

        yield return new WaitForSeconds(reloadTime);

        RefillMag();
    }

    void ShootIfAble()
    {
        if (currentAmmo == 0 || reloading) { if (!reloading) { StartCoroutine(Reload()); } return; }
        if (shotDelta < shotDelay) { shotDelta += Time.deltaTime; return; }

        //used for bots
        if (shoot) { Shoot(); }

        //if has no ammo reload and return
        if (!useMouseInput) { return; }
        if (Input.GetKeyDown(KeyCode.R) && !reloading) { StartCoroutine(Reload()); }
        if (!automatic && !Input.GetMouseButtonDown(0)) { return; }
        else if (!Input.GetMouseButton(0)) { return; }

        Shoot();
    }

    void Shoot()
    {
        PlayFireAnimation();

        shotDelta = 0;
        currentAmmo--;
        if (useAmmoCounter) { AmmoCounter.SetAmmo(currentAmmo); }

        SetFirePointRotation();
        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        SetBulletVariables(bulletInstance.GetComponent<Bullet>());

        if (muzzleFlash != null)
        {
            GameObject muzzleFlashInstance = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
            muzzleFlashInstance.transform.parent = transform;
        }

        foreach (ParticleSystem particleSystem in muzzleFlashesToPlay)
        {
            particleSystem.Play();
        }
    }

    public void RefillMag()
    {
        currentAmmo = maxAmmo;
        if (useAmmoCounter) { AmmoCounter.SetAmmo(currentAmmo); }

        reloading = false;
    }

    void PlayFireAnimation()
    {
        if (animator == null) { return; }

        float[] weights = new float[0];
        foreach (var item in fireAnimations)
        {
            weights = Mike.MikeArray.Append(weights, item.chanceWeight);
        }

        animator.SetTrigger(fireAnimations[Mike.MikeRandom.RandomIntByWeights(weights)].name);
    }

    void PlayReloadAnimation()
    {
        float[] weights = new float[0];
        foreach (var item in reloadAnimations)
        {
            weights = MikeArray.Append(weights, item.chanceWeight);
        }
        animator.SetTrigger(reloadAnimations[Mike.MikeRandom.RandomIntByWeights(weights)].name);
    }

    void SetFirePointRotation()
    {
        if (Physics.Raycast(viewPoint.position, viewPoint.forward, out RaycastHit hit))
        {
            firePoint.LookAt(hit.point);
        }
        else
        {
            firePoint.LookAt(firePoint.position + viewPoint.forward);
        }
    }

    void SetBulletVariables(Bullet bulletInstance)
    {
        if (bulletInstance == null) { return; }

        if (!useMouseInput)
        {
            bulletInstance.hitIndicator = null;
            bulletInstance.killIndicator = null;
        }

        bulletInstance.speed = bulletSpeed;
        bulletInstance.shooter = MikeTransform.GetParentOfParents(transform).GetChild(0);
        bulletInstance.damage = damage;
        bulletInstance.range = maxRange;
        bulletInstance.tag = MikeTransform.GetParentOfParents(transform).tag;
    }

    //LOL

}
