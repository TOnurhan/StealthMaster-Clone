using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerBehaviour : CharacterBaseScript
{
    public static PlayerBehaviour Instance { get; private set; }
    [SerializeField] LayerMask layerMask = default;
    [SerializeField] GameObject shuriken = default;
    public float throwSpeed;
    Vector3 enemyPos;
    [HideInInspector] public Vector3 direction;
    public FloatingJoystick variableJoystick;
    [SerializeField] HealthBarManager healthBar = default;
    float turnSmoothVelocity;
    [SerializeField] float turnSmoothTime = 0.125f;
    public bool hasKey = false;
    public static event Action OpenTheDoor;
    RaycastHit hit;


    //[SerializeField] GameObject item1;

    private void Awake()
    {
        Instance = this;
        EnemyProjectile.GotHit += GotShot;
        healthBar.HealthReachedZero += Died;
        rigidBody = GetComponent<Rigidbody>();

        healthBar.IsHealthFull();

        variableJoystick.Reset();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        Debug.DrawLine(transform.position + Vector3.up, hit.point, Color.green);
    }

    void HandleMovement()
    {
        direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            rigidBody.velocity = direction * speed;
        }
        else
        {
            rigidBody.velocity = Vector3.zero;
        }
    }

    void Attack()
    {
        if (Physics.Raycast(transform.position + Vector3.up, enemyPos - transform.position, out hit, Mathf.Infinity, layerMask))
        {
            
            //Shuriken'i önceden spawnla. Instantiate etme...
            if (hit.collider.CompareTag("Enemy"))
            {
                Projectile spawnedShuriken = Instantiate(shuriken, new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity).GetComponent<Projectile>();
                spawnedShuriken.targetEnemy = hit.transform;
                spawnedShuriken.speed = throwSpeed;
            }
        }
    }

    void GotShot()
    {
        healthBar.HealthBarChanged(-5);
    }

    void Died()
    {
        gameObject.SetActive(false);
        GameObject deadBody = Instantiate(dummyBody, transform.position, Quaternion.identity);
        deadBody.GetComponent<Rigidbody>().AddForce(-transform.forward * forceToDead);
        GameController.Instance.GameOver();
    }

    void TurnToItem()
    {
       // GameObject Item = Instantiate(item1, transform.position, Quaternion.identity);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyPos = other.transform.position;
            Attack();
        }

        if (other.CompareTag("Door"))
        {
            if (hasKey)
            {
                OpenTheDoor?.Invoke();
            }
        }
    }
}
