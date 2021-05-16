using System;
using UnityEngine;

public class PlayerBehaviour : CharacterBaseScript
{
    public static PlayerBehaviour Instance { get; private set; }
    public FloatingJoystick variableJoystick;
    [SerializeField] private LayerMask layerMask = default;
    [SerializeField] private GameObject shuriken = default;
    [SerializeField] private HealthBarManager healthBar = default;
    [SerializeField] private float turnSmoothTime = 0.125f;
    [SerializeField] private Animation anim;
    public float throwSpeed;
    [HideInInspector] public Vector3 direction;
    public bool hasKey = false;
    private bool died = false;
    public static event Action OpenTheDoor;
    private Vector3 enemyPos;
    private float turnSmoothVelocity;
    private RaycastHit hit;

    private void Awake()
    {
        Instance = this;
        EnemyProjectile.GotHit += GotShot;
        healthBar.HealthReachedZero += Died;
        rigidBody = GetComponent<Rigidbody>();
        healthBar.IsHealthFull();
        variableJoystick.Reset();
        anim.Play("AttackIdle");
    }

    private void FixedUpdate()
    {
        if (!died)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;

        if (direction.magnitude >= 0.1f)
        {
            anim.Play("Run");
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            rigidBody.velocity = direction * speed;
        }

        else
        {
            rigidBody.velocity = Vector3.zero;
            anim.Play("AttackIdle");
        }
    }

    private void Attack()
    {
        if (Physics.Raycast(transform.position + Vector3.up, enemyPos - transform.position, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                anim.Play("D_Attack");
                Projectile spawnedShuriken = Instantiate(shuriken, new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity).GetComponent<Projectile>();
                spawnedShuriken.targetEnemy = hit.transform;
                spawnedShuriken.speed = throwSpeed;
            }
        }
    }

    private void GotShot()
    {
        healthBar.HealthBarChanged(-5);
    }

    private void Died()
    {
        died = true;
        transform.tag = "Dead";
        anim.Play("Death");
        GameController.Instance.GameOver();
    }

    void TurnToItem()
    {
        //Turn to an item when not moving. Requires upgrade.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (died)
        {
            return;
        }
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
