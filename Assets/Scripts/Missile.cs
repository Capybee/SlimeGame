using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{   
    private float _Speed;
    private int _Damage;
    private Vector3 _Target;

    public void Fire(int Damage, float Speed, Vector3 Target)
    {
        _Speed = Speed;
        _Damage = Damage;
        _Target = Target;
    }

    public int GetDamage()
    {
        return _Damage;
    }

    private void FixedUpdate() 
    {
        transform.position = Vector3.MoveTowards(transform.position, _Target, _Speed);

        if (Vector3.Distance(transform.position, _Target) < 0.1f)
        {
            Destroy(gameObject, 0.2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        Entity EntityInstance = other.gameObject.GetComponent<Entity>();

        switch(EntityInstance.GetEntityType())
        {
            case EntityTypes.TrainingTarget:
            case EntityTypes.FlyingTarget:
            case EntityTypes.Missing:
            case EntityTypes.RangeMissing:
                Destroy(gameObject);
                break;
        }
    }
}
