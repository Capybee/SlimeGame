using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Базовый класс для всех существ/сущностей в игре
public class Entity : MonoBehaviour
{
    [SerializeField] protected int HealthPoint;
    [SerializeField] protected int Damage;
    [SerializeField] protected int MeleeDamage;
    [SerializeField] protected EntityTypes EntityType;

    public virtual void TakingDamage(int TakeDamage) {}

    protected virtual void Attack() {}

    protected virtual void Death() {}

    protected virtual void AttackOnFlyingTargets() {}
    
    protected virtual void MelleAttack() {}

    public EntityTypes GetEntityType() 
    {
        return EntityType;
    }

    protected virtual void SearchTargets() {}

    public virtual void Healing(int RecoverableHealth) {}
}

//Перечисление всех типов существ/сущностей в игре
public enum EntityTypes
{
    Player = 0,
    TrainingTarget = 1,
    FlyingTarget = 2,
    Missing = 3,
    RangeMissing = 4,
    Stalactitl = 5,
    Observer = 6
}
