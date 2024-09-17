using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTarget : Entity
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Missile")
        {
            Missile MissileInstance = other.gameObject.GetComponent<Missile>();

            TakingDamage(MissileInstance.GetDamage());
        }
    }

    public override void TakingDamage(int TakeDamage)
    {
        Debug.Log($"Полученно {TakeDamage} урона");
    }
}
