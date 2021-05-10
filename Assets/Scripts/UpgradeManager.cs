using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] ItemSelection itemSelection = default;
    [SerializeField] PlayerBehaviour playerBehaviour = default;
    [SerializeField] HealthBarManager healthBar = default;
    [SerializeField] SphereCollider playerRange = default;

    void Initialize()
    {
        itemSelection.Upgrade1Selected += RangeUp;
        itemSelection.Upgrade2Selected += HealthRestore;
        itemSelection.Upgrade3Selected += MoreMoney;
        itemSelection.Upgrade4Selected += GottaGoFast;
        itemSelection.Upgrade5Selected += ProjectileSpeedUp;
        itemSelection.Upgrade6Selected += TurnToAnItem;
    }

    private void Awake()
    {
        Initialize();
    }

    void RangeUp()
    {
        playerRange.radius += 0.25f;
    }

    void HealthRestore()
    {
        healthBar.HealthBarChanged(100);
        healthBar.IsHealthFull();
    }

    void MoreMoney()
    {
        playerRange.GetComponent<MoneyMagnet>().extraMoney++;
    }

    void GottaGoFast()
    {
        playerBehaviour.speed += 2;
    }

    void ProjectileSpeedUp()
    {
        playerBehaviour.throwSpeed += 10;
    }

    void TurnToAnItem()
    {

    }
}
