using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public float currentHealth;
    public float maximumHealth;
    [SerializeField] Image mask = default;
    public event Action HealthReachedZero;
    float fillAmount;
    [SerializeField] Transform cam = default;

    private void OnEnable()
    {
        HealthBarChanged(0);
    }

    private void Update()
    {
        //Debug.Log(followingTarget.position);
        transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
    }
    public void HealthBarChanged(float healthAmount)
    {
        if (currentHealth + healthAmount >= 100)
        {
            currentHealth = maximumHealth;
        }

        else 
        {
            currentHealth += healthAmount;
        }

        fillAmount = currentHealth / maximumHealth;
        mask.fillAmount = fillAmount;

        IsHealthFull();

        if(currentHealth == 0)
        {
            HealthReachedZero?.Invoke();
        }
    }

    public void IsHealthFull()
    {
        if (currentHealth == maximumHealth)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
