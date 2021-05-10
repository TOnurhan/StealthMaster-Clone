using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
    public int enemyCount;
    public Transform lastPosition;
    [SerializeField] private List<EnemyBehaviour> enemies;

    public void DisableAllEnemies()
    {
        gameObject.SetActive(false);
        enemies.ForEach(enemy => enemy.DisableTweens());
        Destroy(gameObject,1.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.Instance.LevelCompleted();
        }
    }
}