using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidKit : MonoBehaviour
{
    [SerializeField] private int RecoverableHealth;
    
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.tag == "Player")
        {
            Player PlayerInstance = other.gameObject.GetComponent<Player>();
            PlayerInstance.Healing(RecoverableHealth);
            Destroy(gameObject);
        }
    }
}
