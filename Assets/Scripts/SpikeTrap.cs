using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Trap
{
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.tag == "Player")
        {
            Player PlayerInstance = other.gameObject.GetComponent<Player>();

            PlayerInstance.TakingDamage(Damage);

            if(PlayerInstance.GetDirection())
            {
                other.transform.position = RightRespawnPoint;
            }
            else
            {
                other.transform.position = LeftRespawnPoint;
            }
        }    
    }
}
