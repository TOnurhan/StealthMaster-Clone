using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [HideInInspector] public float speed = default;
    public Transform player;
    public static event Action GotHit;
    void Update()
    {
        transform.Rotate(0, speed, 0);
        transform.position = Vector3.MoveTowards(transform.position, player.position + Vector3.up, Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GotHit?.Invoke();
        }
        Destroy(gameObject);
    }
}