using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private float _Speed;
    private int _Damage;
    private GameObject _Target;
    private EntityTypes _EntityType;
    private bool IsFire = false; //Переменная указывающая на то, совершен ли выстрел
    private int LifeTimer = 0;
    private const int  LifeTimerValue =120;

    /// <summary>
    /// Запускает снаряд в указанную цель
    /// </summary>
    /// <param name="Damage">Урон снаряда</param>
    /// <param name="Speed">Скорость снаряда</param>
    /// <param name="Target">Цель</param>
    /// <param name="EntityType">Тип сущности запустившей снаряд</param>
    public void Fire(int Damage, float Speed, GameObject Target, EntityTypes EntityType)
    {
        _Speed = Speed;
        _Damage = Damage;
        _Target = Target;
        _EntityType = EntityType;
        IsFire = true;
        LifeTimer = LifeTimerValue;
    }

    public int GetDamage()
    {
        return _Damage;
    }

    private void FixedUpdate() 
    {
        if(IsFire)
        {
            transform.position = Vector3.MoveTowards(transform.position, _Target.transform.position, _Speed);

            if (Vector3.Distance(transform.position, _Target.transform.position) < 0.1f)
            {
                Destroy(gameObject, 0.2f);
            }

            LifeCheck();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        switch (_EntityType)
        {
            case EntityTypes.Player:
                Entity EntityInstance = other.gameObject.GetComponent<Entity>();

                switch(EntityInstance.GetEntityType())
                {
                    case EntityTypes.FlyingTarget:
                        FlyingTarget FlyingTargetInstance = other.gameObject.GetComponent<FlyingTarget>();
                        FlyingTargetInstance.TakingDamage(_Damage);
                        IsFire = false;
                        Destroy(gameObject);
                        break;
                    case EntityTypes.RangeMissing:
                        RangeMissing RangeMissingInstacne = other.gameObject.GetComponent<RangeMissing>();
                        RangeMissingInstacne.TakingDamage(_Damage);
                        IsFire = false;
                        Destroy(gameObject);
                        break;
                    case EntityTypes.TrainingTarget:
                        TrainingTarget TrainingTargetInstance = other.gameObject.GetComponent<TrainingTarget>();
                        TrainingTargetInstance.TakingDamage(_Damage);
                        IsFire = false;
                        Destroy(gameObject);
                        break;
                    case EntityTypes.Missing:
                        Missing MissingInstance = other.gameObject.GetComponent<Missing>();
                        MissingInstance.TakingDamage(_Damage);
                        IsFire = false;
                        Destroy(gameObject);
                        break;
                    case EntityTypes.Stalactitl:
                        Stalactitl StalactitlInstance = other.gameObject.GetComponent<Stalactitl>();
                        StalactitlInstance.TakingDamage(_Damage);
                        IsFire = false;
                        Destroy(gameObject);
                        break;
                    case EntityTypes.Observer:
                        Observer ObserverInstance = other.gameObject.GetComponent<Observer>();
                        ObserverInstance.TakingDamage(_Damage);
                        IsFire = false;
                        Destroy(gameObject);
                        break;
                }
            break;
            case EntityTypes.RangeMissing:
                Entity EntityInstanceCase2 = other.gameObject.GetComponent<Entity>();
                if(EntityInstanceCase2.GetEntityType() == EntityTypes.Player)
                {
                    Player PlayerInstance = other.gameObject.GetComponent<Player>();
                    PlayerInstance.TakingDamage(_Damage);
                    IsFire = false;
                    Destroy(gameObject);
                }
            break;
        }
    }
    
    private void LifeCheck()
    {
        if(LifeTimer == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            LifeTimer -= 1;
        }
    }
}
