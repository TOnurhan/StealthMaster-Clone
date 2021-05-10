using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyMagnet : MonoBehaviour
{
    public int moneyCount;
    public int extraMoney;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Money"))
        {
            other.gameObject.LeanMove(transform.position, 0.1f).setOnComplete(() =>
            {
                moneyCount = moneyCount + 1 + extraMoney;
                Destroy(other.gameObject);
            });
        }
    }
}
