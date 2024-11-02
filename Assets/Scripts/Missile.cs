using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{   
    private float _Speed;
    private int _Damage;
    private Vector3 _Target;

    /// <summary>
    /// Запускает снаряд в цель которой является абстрактная точка перед сущностью
    /// </summary>
    /// <param name="Damage">Урон снаряда</param>
    /// <param name="Speed">Скорость снаряда</param>
    /// <param name="Target">Точка перед сущностью</param>
    public void Fire(int Damage, float Speed, Vector3 Target)
    {
        Debug.Log($"Урон переданый в снаряд: {Damage}");
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
                TrainingTarget TrainingTargetInstance = other.gameObject.GetComponent<TrainingTarget>();
                TrainingTargetInstance.TakingDamage(_Damage);
                Destroy(gameObject);
                break;
            case EntityTypes.FlyingTarget:
                FlyingTarget FlyingTargetInstance = other.gameObject.GetComponent<FlyingTarget>();
                FlyingTargetInstance.TakingDamage(_Damage);
                Destroy(gameObject);
                break;
            case EntityTypes.Missing:
                Missing MissingInstance = other.gameObject.GetComponent<Missing>();
                MissingInstance.TakingDamage(_Damage);
                Destroy(gameObject);
                break;
            case EntityTypes.RangeMissing:
                RangeMissing RangeMissingInstacne = other.gameObject.GetComponent<RangeMissing>();
                RangeMissingInstacne.TakingDamage(_Damage);
            Destroy(gameObject);
                break;
            case EntityTypes.Stalactitl:
                Stalactitl StalactitlInstance = other.gameObject.GetComponent<Stalactitl>();
                StalactitlInstance.TakingDamage(_Damage);
                Destroy(gameObject);
                break;
            case EntityTypes.Observer:
                Observer ObserverInstance = other.gameObject.GetComponent<Observer>();
                ObserverInstance.TakingDamage(_Damage);
                Destroy(gameObject);
                break;
        }
    }
}
