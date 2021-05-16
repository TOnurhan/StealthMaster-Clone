using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBehaviour : CharacterBaseScript
{
    [SerializeField] private FieldOfView fieldOfView;
    [Header("DetectionField")]
    [SerializeField] private float visionConeAngle = default;
    [SerializeField] private float visionRange = default;
    [SerializeField] private LayerMask layerMask = default;

    [Header("Waypoint")]
    [SerializeField] private GameObject waypoints = default;
    private List<GameObject> waypointList;
    private int waypointIndex = 0;

    private bool patrolling;
    private Vector3 latestPosition;
    private Vector3 playerPos;

    [SerializeField] private float cooldownTime = default;
    private bool _isCooldown = false;
    [SerializeField] private GameObject shuriken = default;
    [SerializeField] private float throwSpeed = default;

    private LTDescr ChasePlayerLTD;
    private LTDescr MoveNextPointLTD;
    private LTDescr LookAroundLTD;

    private RaycastHit hit;

    private bool isDead = false;
    private bool isSeen = false;
    private int previousIndex;

    [SerializeField] private Animation anim;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        waypointList = new List<GameObject>();
        if (waypoints.transform.childCount > 0)
        {
            patrolling = true;
            for (int i = 0; i < waypoints.transform.childCount; i++)
            {
                waypointList.Add(waypoints.transform.GetChild(i).gameObject);
            }
        }
        anim.Play("AttackIdle");
        StartCoroutine(EnemyAI());
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        FieldOfViewHandle();

        if (Vector3.Angle(transform.forward, (playerPos - transform.position).normalized) < fieldOfView.fov / 2f)
        {
            playerPos = PlayerBehaviour.Instance.transform.position;
            if (Physics.Raycast(transform.position, playerPos - transform.position, out hit, fieldOfView.viewDistance, layerMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    PlayerSeen();
                    DisableTweens();
                }
            }

            else if (isSeen)
            {
                ChasePlayer();
                isSeen = false;
            }
        }
    }

    private void FieldOfViewHandle()
    {
        fieldOfView.SetAimDirection(transform.forward);
        fieldOfView.SetOrigin(transform.position + new Vector3(0, 0.1f, 0));
    }

    private void LookAround()
    {
        float angle = Random.Range(0, 2) == 0 ? -45 : 45;
        LookAroundLTD = LeanTween.rotateAround(gameObject, Vector3.up, angle, 1f).setOnComplete(() => 
        {
            LeanTween.delayedCall(0.5f,() =>
            {
                LeanTween.rotateAround(gameObject, Vector3.up, -angle * 2, 1f).setOnComplete(() => 
                {
                    if (!isDead)
                    {
                        StartCoroutine(EnemyAI());
                    }
                });
            });
        });
    }

    private void MoveToNextPoint()
    {
        while (waypointIndex == previousIndex)
        {
            waypointIndex = Random.Range(0, waypointList.Count);
        }
        transform.LookAt(waypointList[waypointIndex].transform.position);
        anim.Play("Run");
        MoveNextPointLTD = transform.LeanMove(waypointList[waypointIndex].transform.position, 2f).setOnComplete(() =>
        {
            if (!isDead)
            {
                StartCoroutine(EnemyAI());
            }
            previousIndex = waypointIndex;
            anim.Play("AttackIdle");
        });
    }

    private IEnumerator EnemyAI()
    {
        yield return new WaitForSeconds(1f);
        float choosedAction = Random.Range(0, 2);

        if(choosedAction == 0 && patrolling && !isDead)
        {
            MoveToNextPoint();
        }

        else
        {
            LookAround();
        }
    }

    private void PlayerSeen()
    {
        transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
        Attack();
        isSeen = true;
    }

    private void ChasePlayer()
    {
        
        if (latestPosition == Vector3.zero)
        {
            latestPosition = transform.position;
        }

        ChasePlayerLTD = LeanTween.delayedCall(gameObject, 1f, () =>
        {
            transform.LookAt(playerPos);
            anim.Play("Run");
            LeanTween.move(gameObject, playerPos, 3f).setOnComplete(() =>
            {
                anim.Play("AttackIdle");
                LeanTween.delayedCall(gameObject, 1f, () =>
                {
                    transform.LookAt(latestPosition);
                    LeanTween.move(gameObject, latestPosition, 3f).setOnComplete(() =>
                    {
                        if (!isDead)
                        {
                            StartCoroutine(EnemyAI());
                        }
                    });
                });
            });
        });
    }

    public void Died()
    {
        isDead = true;
        anim.Play("Death");
        transform.tag = "Dead";
        fieldOfView.gameObject.SetActive(false);
    }

    public void DisableTweens()
    {
        if (LookAroundLTD != null)
        {
            LeanTween.cancel(LookAroundLTD.id);
        }

        if (patrolling)
        {
            StopCoroutine(EnemyAI());

            if (MoveNextPointLTD != null)
            {
                LeanTween.cancel(MoveNextPointLTD.id);
            }
        }

        if (ChasePlayerLTD != null)
        {
            LeanTween.cancel(ChasePlayerLTD.id);
        }
    }

    private void Attack()
    {
        if (!_isCooldown)
        {
            anim.Play("D_Attack");
            EnemyProjectile spawnedShuriken = Instantiate(shuriken, new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity).GetComponent<EnemyProjectile>();
            spawnedShuriken.player = PlayerBehaviour.Instance.transform;
            spawnedShuriken.speed = throwSpeed;
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        _isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        _isCooldown = false;
    }

}