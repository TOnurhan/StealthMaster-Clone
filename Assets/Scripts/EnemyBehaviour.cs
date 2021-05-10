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
    [SerializeField] float visionConeAngle = default;
    [SerializeField] float visionRange = default;
    [SerializeField] LayerMask layerMask = default;

    [Header("Waypoint")]
    [SerializeField] GameObject waypoints = default;
    List<GameObject> waypointList;
    int waypointIndex = 0;

    bool patrolling;
    Vector3 latestPosition;
    Vector3 playerPos;

    [SerializeField] float cooldownTime = default;
    private bool _isCooldown = false;
    [SerializeField] GameObject shuriken = default;
    [SerializeField] float throwSpeed = default;

    private LTDescr ChasePlayerLTD;
    private LTDescr MoveNextPointLTD;
    private LTDescr LookAroundLTD;

    RaycastHit hit;

    bool isDead = false;
    bool goruldu = false;
    int previousIndex;

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

        StartCoroutine(EnemyAI());
    }

    private void Update()
    {
        if (!isDead)
        {
            FieldOfViewHandle();
        }

        if (Vector3.Angle(transform.forward, (playerPos - transform.position).normalized) < fieldOfView.fov / 2f)
        {
            playerPos = PlayerBehaviour.Instance.GetPosition();
            if (Physics.Raycast(transform.position, playerPos - transform.position, out hit, fieldOfView.viewDistance, layerMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    PlayerSeen();
                    DisableTweens();
                }
            }

            else if (goruldu)
            {
                ChasePlayer();
                goruldu = false;
            }
        }
    }

    void FieldOfViewHandle()
    {
        fieldOfView.SetAimDirection(transform.forward);
        fieldOfView.SetOrigin(transform.position + new Vector3(0, 0.1f, 0));
    }

    void LookAround()
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

    void MoveToNextPoint()
    {
        while (waypointIndex == previousIndex)
        {
            waypointIndex = Random.Range(0, waypointList.Count);
        }
        transform.LookAt(waypointList[waypointIndex].transform.position);
        MoveNextPointLTD = transform.LeanMove(waypointList[waypointIndex].transform.position, 2f).setOnComplete(() =>
        {
            if (!isDead)
            {
                StartCoroutine(EnemyAI());
            }
            previousIndex = waypointIndex;
        });
    }

    IEnumerator EnemyAI()
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

    //Düşman görüldüyse yaptıklarını bırak ve düşmana saldır.
    void PlayerSeen()
    {
        //Karakterin y'si 0'dan büyükse sorun çıkıyor.
        transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
        Attack();
        goruldu = true;
    }

    // Düşman görüşünden çıktı ise takip et, belirli sürede bulamazsan işine geri dön.
    void ChasePlayer()
    {
        if (latestPosition == Vector3.zero)
        {
            latestPosition = transform.position;
        }
        ChasePlayerLTD = LeanTween.delayedCall(gameObject, 1f, () =>
        {
            transform.LookAt(playerPos);
            LeanTween.move(gameObject, playerPos, 3f).setOnComplete(() =>
            {

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
        fieldOfView.gameObject.SetActive(false);
        gameObject.SetActive(false);
        GameObject deadBody = Instantiate(dummyBody, transform.position, Quaternion.identity);
        deadBody.transform.SetParent(transform.parent);
        if(deadBody.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(-transform.forward * forceToDead);
        }
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

    //Düşmana saldır.
    void Attack()
    {
        if (!_isCooldown)
        {
            EnemyProjectile spawnedShuriken = Instantiate(shuriken, new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity).GetComponent<EnemyProjectile>();
            spawnedShuriken.player = PlayerBehaviour.Instance.transform;
            spawnedShuriken.speed = throwSpeed;
            StartCoroutine(AttackCooldown());
        }
    }

    //Saldırılar arası bekleme süresi.
    IEnumerator AttackCooldown()
    {
        _isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        _isCooldown = false;
    }

}