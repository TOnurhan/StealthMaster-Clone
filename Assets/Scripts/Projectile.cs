using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float speed = default;
    public Transform targetEnemy;

    void Update()
    {
        transform.Rotate(0, speed, 0);
        transform.position = Vector3.MoveTowards(transform.position, targetEnemy.position + Vector3.up, Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == targetEnemy)
        {
            targetEnemy.GetComponent<EnemyBehaviour>().Died();
            Destroy(gameObject);
        }
    }
}
