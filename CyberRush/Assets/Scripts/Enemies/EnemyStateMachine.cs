using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStateMachine : MonoBehaviour
{
    //Serialized
    [Header("Shooting")]
    [SerializeField] float aimSpeed = .25f;
    [SerializeField] float shootingMoveSpeed = 3;
    [SerializeField] float moveBackSpeed = 1;
    [SerializeField] float shootStoppingDistance;
    [SerializeField] float moveBackThreshold;

    [Header("Stupidness")]
    [SerializeField] float accuracyRange = 3;
    [SerializeField] float roamRadius = 5;

    [Header("Animators")]
    [SerializeField] Animator droneAnim;

    [Header("Gun")]
    [SerializeField] Transform gunPos;
    [SerializeField] float secondsPerShot;
    [SerializeField] float laserPremonitionStartTime = 3;
    [SerializeField] float laserSecondsPerDamage = .5f;
    [SerializeField] float laserShootTime = 3;
    [SerializeField] int bulletsPerShot = 1;
    [SerializeField] float timeBetweenBullets = 0.25f;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject lineRendererObj;
    [Header("References")]
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] AudioClip laserFire;
    [SerializeField] AudioClip laserPremo;
    //Public 
    public enum enemyState
    {
        reactiveShooting,       //Rare enemy. Will seek you out more actively
        stillShooting,      //Common type. Will just move a little in place and shoot in your *general* direction
        turret,
        death
    }
    public enemyState enemyType = enemyState.reactiveShooting;
    public bool isDead;

    //Private
    NavMeshAgent enemy;
    GameObject player;
    float nextTimeToFire;
    float nextTimeToFireP;
    Vector3 randRot;
    Vector3 startPos;
    LineRenderer lineRenderer;
    AudioSource source;

    private void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemy.avoidancePriority = Random.Range(70, 99);

        randRot = new Vector3(Random.Range(-5f, 5f), 0, 0);
        startPos = transform.position;
        nextTimeToFire = Time.time + secondsPerShot;
        nextTimeToFireP = Time.time + laserPremonitionStartTime;

        TryGetComponent<AudioSource>(out source);

        if (enemyType == enemyState.turret)
        {
            lineRenderer = Instantiate(lineRendererObj, Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
            GetComponent<DamageSystem>().lineRenderer = lineRenderer.gameObject;
        }
    }

    private void Update()
    {
        if (isDead) { return; }
        switch (enemyType)
        {
            case enemyState.reactiveShooting:
                //agent changes
                enemy.speed = shootingMoveSpeed;
                enemy.updateRotation = false;
                ReactiveShooting();
                break;
            case enemyState.stillShooting:
                //agent changes
                enemy.speed = shootingMoveSpeed;
                enemy.updateRotation = false;
                StillShooting();
                break;
            case enemyState.turret:
                Turret();
                break;
            default:
                Debug.LogError("shit i thought this was not possible");
                break;
        }
    }

    void ReactiveShooting()
    {
        enemy.stoppingDistance = shootStoppingDistance;

        droneAnim.SetFloat("Horizontal", enemy.velocity.x);
        droneAnim.SetFloat("Vertical", enemy.velocity.z);

        Vector3 destination;

        // Vector3 directionToRotate = player.transform.position - gunPos.transform.position;     //Rotate toward player from the point of view of the gun
        // Quaternion rotate = Quaternion.LookRotation(directionToRotate);
        // transform.rotation = Quaternion.Lerp(transform.rotation, rotate, aimSpeed * Time.deltaTime);
        // transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        RotateTowardPlayer(Vector3.zero);

        destination = player.transform.position;

        //movebackwhentooclose
        if (Vector3.Distance(player.transform.position, transform.position) < moveBackThreshold)
        {
            destination = transform.position + -transform.forward * 2 + transform.right;
            enemy.stoppingDistance = 0;
            enemy.speed = moveBackSpeed;
        }

        Vector3 dirToTarget = (player.transform.position - gunPos.transform.position).normalized;
        if (!Physics.Raycast(gunPos.position, dirToTarget, out RaycastHit hit, 100, obstacleMask) && Vector3.Angle(transform.forward, dirToTarget) < 10)
        {
            Shoot();
        }
        else
        {
            //check right distance
            Physics.Raycast(transform.position, transform.right, out RaycastHit rightHit);
            //check left distance
            Physics.Raycast(transform.position, -transform.right, out RaycastHit leftHit);

            if (Vector3.Distance(rightHit.point, transform.position) <= Vector3.Distance(leftHit.point, transform.position))
            {
                destination = transform.position + transform.right * 2;
            }
            else
            {
                destination = transform.position + -transform.right * 2;
            }
            enemy.stoppingDistance = 0;
        }

        enemy.SetDestination(destination);
    }

    void StillShooting()
    {
        // Vector3 directionToRotate = player.transform.position + randRot - gunPos.transform.position;     //Rotate toward player from the point of view of the gun with randomness
        // Quaternion rotate = Quaternion.LookRotation(directionToRotate);
        // transform.rotation = Quaternion.Lerp(transform.rotation, rotate, aimSpeed * Time.deltaTime);
        // transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        RotateTowardPlayer(randRot);

        //Random Movement
        if (enemy.remainingDistance < 1f)
        {
            float Roffset = Random.Range(roamRadius / 2, -roamRadius / 2);
            float Loffset = Random.Range(roamRadius / 2, -roamRadius / 2);
            Vector3 movement = startPos + new Vector3(Roffset, 0, Loffset);
            enemy.SetDestination(movement);
        }

        Shoot();
    }

    bool laserAbleToDamage = false;     //When laser is firing
    bool isDamaging = false;    //When laser adds damage. Set true when player steps in the laser and false after set amount of time
    bool laserAiming = false;   //When laser is aiming
    float currentLaserWidth;
    void Turret()
    {
        Vector3 dirToTarget = (player.transform.position - gunPos.transform.position).normalized;
        if (Physics.Raycast(gunPos.position, dirToTarget, 100, obstacleMask) && !laserAbleToDamage && !laserAiming)
        {
            nextTimeToFireP = Time.time + laserPremonitionStartTime;
            nextTimeToFire = Time.time + secondsPerShot;
            return;
        }

        if (Physics.Raycast(gunPos.transform.position, gunPos.transform.forward, out RaycastHit playerHit) && laserAbleToDamage && !isDamaging)
        {
            if (playerHit.transform.TryGetComponent<DamageSystem>(out DamageSystem dms))
            {
                StartCoroutine(LaserDamage(dms));
            }
        }
        lineRenderer.endWidth = Mathf.Lerp(lineRenderer.endWidth, currentLaserWidth, Time.deltaTime * 2);
        lineRenderer.startWidth = Mathf.Lerp(lineRenderer.startWidth, currentLaserWidth, Time.deltaTime * 2);
        // lineRenderer.endWidth = currentLaserWidth;
        // lineRenderer.startWidth = currentLaserWidth;

        if (Time.time > nextTimeToFire)
        {
            currentLaserWidth = .5f;
            if (Physics.Raycast(gunPos.transform.position, gunPos.transform.forward, out RaycastHit hit, 100, obstacleMask))
            {
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, gunPos.transform.position + gunPos.transform.forward * 100);
            }
            lineRenderer.SetPosition(0, gunPos.transform.position);

            laserAiming = false;
            laserAbleToDamage = true;

            if (!disablingLaser)
            { StartCoroutine(DisableLaser()); }
        }
        else if (Time.time > nextTimeToFireP)
        {
            if (!source.isPlaying)
            {
                source.clip = laserPremo;
                source.Play();
            }

            lineRenderer.gameObject.SetActive(true);
            currentLaserWidth = .1f;
            laserAiming = true;

            if (Physics.Raycast(gunPos.transform.position, gunPos.transform.forward, out RaycastHit hit, 100, obstacleMask))
            {
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, gunPos.transform.position + gunPos.transform.forward * 100);
            }
            lineRenderer.SetPosition(0, gunPos.transform.position);
        }

        if (laserAbleToDamage) return;
        RotateTowardPlayer(Vector3.zero);
    }

    void RotateTowardPlayer(Vector3 randRotation)
    {
        Vector3 directionToRotate = player.transform.position + randRotation - gunPos.transform.position;     //Rotate toward player from the point of view of the gun with randomness
        Quaternion rotate = Quaternion.LookRotation(directionToRotate);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotate, aimSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    bool disablingLaser;
    IEnumerator DisableLaser()
    {
        disablingLaser = true;

        source.clip = laserFire;
        source.Play();

        yield return new WaitForSeconds(laserShootTime);

        currentLaserWidth = 0;
        lineRenderer.gameObject.SetActive(false);
        laserAbleToDamage = false;

        nextTimeToFireP = Time.time + laserPremonitionStartTime;
        nextTimeToFire = Time.time + secondsPerShot;

        disablingLaser = false;

        source.Stop();
    }

    IEnumerator LaserDamage(DamageSystem damageSystem)
    {
        isDamaging = true;
        damageSystem.TakeDamage();
        yield return new WaitForSeconds(laserSecondsPerDamage);
        isDamaging = false;
    }

    void Shoot()
    {
        if (Time.time > nextTimeToFire)
        {
            StartCoroutine(ShootBullet());

            nextTimeToFire = Time.time + secondsPerShot;
            randRot = new Vector3(Random.Range(-accuracyRange, accuracyRange), 0, 0); //Get a random rotation for some applications
        }
    }

    IEnumerator ShootBullet()
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            GameObject shotBullet = Instantiate(bullet, gunPos.position, gunPos.rotation);
            yield return new WaitForSeconds(timeBetweenBullets);
        }
    }
}
