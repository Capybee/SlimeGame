using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarControler : MonoBehaviour
{
    [SerializeField] private GameObject HealthBarPrefab;
    [SerializeField] private Canvas CanvasInstance;

    private GameObject HealthBarInstance;
    private TMP_Text EntityName;
    private Slider EntityHealth;
    private TMP_Text EntityHealthNum;
    private Entity EntityInstance;
    private int MaxHealthPoint;
    private Dictionary<EntityTypes, string> EntityDictionary = new Dictionary<EntityTypes, string>()
    {
        {EntityTypes.FlyingTarget, "Цель"},
        {EntityTypes.TrainingTarget,"Маникен"},
        {EntityTypes.Missing, "Пропавший"},
        {EntityTypes.RangeMissing, "Пропавший"},
        {EntityTypes.Observer, "Наблюдатель"}
    };

    private void Awake() 
    {
        HealthBarInstance = Instantiate(HealthBarPrefab);
        HealthBarInstance.transform.SetParent(CanvasInstance.transform);

        EntityName = HealthBarInstance.GetComponentInChildren<TMP_Text>();
        EntityHealth = HealthBarInstance.GetComponentInChildren<Slider>();
        EntityHealthNum = EntityHealth.GetComponentInChildren<TMP_Text>();
        EntityInstance = gameObject.GetComponent<Entity>();

        foreach(var Item in EntityDictionary)
        {
            if(Item.Key == EntityInstance.GetEntityType())
            {
                MaxHealthPoint = EntityInstance.GetHealthPoint();
                EntityName.text = Item.Value;
                EntityHealth.maxValue = MaxHealthPoint;
                EntityHealth.minValue = 0;
                EntityHealth.value = MaxHealthPoint;
                EntityHealthNum.text = $"{EntityInstance.GetHealthPoint()}/{MaxHealthPoint}";
            }
        }

        UpdatePosition();
    }

    private void Update() 
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 WorldPosition = transform.position + Vector3.up * 2f;
        Vector3 ScreenPosition = Camera.main.WorldToScreenPoint(WorldPosition);

        HealthBarInstance.transform.position = ScreenPosition;
    }

    public void Delete()
    {
        Destroy(HealthBarInstance);
    }

    public void UpdateHealthBarValue(int Value)
    {
        EntityHealth.value -= Value;
        EntityHealthNum.text = $"{EntityInstance.GetHealthPoint()}/{MaxHealthPoint}";
    }
}
