using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingTarget : Entity
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "HomingMissile")
        {
            HomingMissile MissileInstance = other.gameObject.GetComponent<HomingMissile>();

            TakingDamage(MissileInstance.GetDamage());
        }
        else if (other.gameObject.tag == "Missile")
        {
            Missile MissileInstance = other.gameObject.GetComponent<Missile>();

            TakingDamage(MissileInstance.GetDamage());
        }
    }

    public override void TakingDamage(int TakeDamage)
    {   
        Destroy(gameObject);
    }
}
