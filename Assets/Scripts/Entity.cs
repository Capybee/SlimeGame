using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Базовый класс для всех существ/сущностей в игре
public class Entity : MonoBehaviour
{
    [SerializeField] protected int HealthPoint;
    [SerializeField] protected int Damage;
    [SerializeField] protected int MeleeDamage;
    [SerializeField] protected EntityTypes EntityType;
    [SerializeField] protected GameObject HealthBarPrefab;

    protected GameObject HealthBarInstance;
    protected TMP_Text EntityName;
    protected Slider HealthBar;
    private bool NotPlayer;

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

    
    private void Awake() 
    {
        if(EntityType != EntityTypes.Player)
        {
            Canvas CanvasInstance = FindAnyObjectByType<Canvas>();

            HealthBarInstance = Instantiate(HealthBarPrefab);
            HealthBarInstance.SetActive(true);

            HealthBarInstance.transform.SetParent(CanvasInstance.transform,false);

            EntityName = HealthBarInstance.GetComponent<TMP_Text>();
            HealthBar = HealthBarInstance.GetComponentInChildren<Slider>();

            switch(EntityType)
            {
                case EntityTypes.TrainingTarget:
                    EntityName.text = "Маникен";
                    break;
                case EntityTypes.FlyingTarget:
                    EntityName.text = "Мишень";
                    break;
                case EntityTypes.Missing:
                case EntityTypes.RangeMissing:
                    EntityName.text = "Пропавший";
                    break;
                case EntityTypes.Stalactitl:
                    EntityName.text = "Сталактитль";
                    break;
                case EntityTypes.Observer:
                    EntityName.text = "Наблюдатель";
                    break;
            }    
            HealthBar.maxValue = HealthPoint;
            HealthBar.minValue = 0;

            NotPlayer = true;

            UpdateEntityNameAndHealthBarPosition();

            HealthBar.value = HealthPoint;
        }
        else
        {
            NotPlayer = false;
        }
    }

    private void Update() 
    {
        if (NotPlayer)
        {
            UpdateEntityNameAndHealthBarPosition();

            HealthBar.value = HealthPoint;
        }
    }

    private void UpdateEntityNameAndHealthBarPosition()
    {
        Vector3 WorldPosition = transform.position + Vector3.up * 2f;
        Vector3 ScreenPosition = Camera.main.WorldToScreenPoint(WorldPosition);
            
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)HealthBarInstance.transform.parent, 
            ScreenPosition, 
            null, 
            out Vector2 LocalPoint
        );

        HealthBarInstance.GetComponent<RectTransform>().localPosition = LocalPoint;
    }
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
