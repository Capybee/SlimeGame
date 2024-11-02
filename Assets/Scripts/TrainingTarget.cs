using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTarget : Entity
{
    public override void TakingDamage(int TakeDamage)
    {
        if((HealthPoint -= TakeDamage) > 0)
        {
            HealthPoint -= TakeDamage;
            Debug.Log($"Получено урона: {TakeDamage} Здоровья осталось: {HealthPoint}");
            base.TakingDamage(TakeDamage);
        }
        else
        {
            Death();
        }
    }
}
