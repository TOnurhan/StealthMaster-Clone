using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBehaviour : MonoBehaviour
{
    private void OnEnable()
    {
        transform.LeanMoveY(transform.position.y - 0.5f, 1.25f).setEaseInOutQuad().setLoopPingPong();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerBehaviour>().hasKey = true;
            gameObject.SetActive(false);
        }
    }
}
