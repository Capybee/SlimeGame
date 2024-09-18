using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private KeyCode AttackKey;
    [SerializeField] private Vector3 RightFirePoint; //Точка в локальной системе координат из которой вылетает снаряд
    [SerializeField] private Vector3 LeftFirePoint; //Точка в локальной системе координат из которой вылетает снаряд
    [SerializeField] private  Vector3 UpFirePoint; //Точка в локальной системе координат из которой вылетает снаряд
    [SerializeField] private GameObject Missile; //Префаб снаряда
    [SerializeField] private GameObject HomingMissile; //Префаб самонаводящегося снаряда
    [SerializeField] private UIControler UIControlerInstance;
    [SerializeField] private KeyCode SearchKey;
    [SerializeField] private KeyCode StopTargetSearchKey;
    [SerializeField] private KeyCode HommingFireKey;
    [SerializeField] private Vector3 RightPunchMediantPoint; //Локальная точка в пространстве, является центром окружности в которой проверяется попадание по противнику
    [SerializeField] private Vector3 LeftPunchMediantPoint; //Локальная точка в пространстве, является центром окружности в которой проверяется попадание по противнику
    [SerializeField] private float PunchRadius; //Радиус в котором проверяется попадание по цели
    [SerializeField] private GameObject VisualizationPunch; //Префаб объекта для визуализации области удара
    [SerializeField] private KeyCode MeleeAttackKey;

    private bool IsRight;
    private Rigidbody2D RB;
    private List<Collider2D> Targets;
    private Collider2D Target;
    private int TargetsCounter = 0;
    private bool TargetIsActive = false;

    private void Start() 
    {
        UIControlerInstance.SetHP(HealthPoint);
    }

    private void Awake() 
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Update() 
    {
        if(Input.GetKeyDown(AttackKey))
        {
            Attack();
        }

        if(RB.velocity.x > 0 )
        {
            IsRight = true;
        }
        else if (RB.velocity.x < 0)
        {
            IsRight = false;
        }

        SearchTargets();

        if(Targets.Count <= 0 || Input.GetKeyDown(StopTargetSearchKey)) 
        {
            UIControlerInstance.StopDrawningSight();
        }

        if(TargetIsActive)
        {
            if(Target == null)
            {
                if(Targets.Count > 0)
                {
                    SelectTarget();
                }
                else
                {
                    UIControlerInstance.StopDrawningSight();
                }
            }
        }

        if(Input.GetKeyDown(SearchKey))
        {
            SelectTarget();
        }

        if(Input.GetKeyDown(HommingFireKey))
        {
            AttackOnFlyingTargets();
        }

        if(Input.GetKeyDown(MeleeAttackKey))
        {
            MelleAttack();
        }
    }

    protected override void Attack()
    {
        GameObject MissileObject = Instantiate(Missile); //Создание снаряда из префаба
        if(IsRight)
        {
            MissileObject.transform.position = transform.TransformPoint(RightFirePoint);  //Перевод локальных координат в глобальные
            MissileObject.GetComponent<Missile>().Fire(Damage, 0.3f, transform.position + new Vector3(15f, 0)); 
        }
        else
        {
            MissileObject.transform.position = transform.TransformPoint(LeftFirePoint); //Перевод локальных координат в глобальные
            MissileObject.GetComponent<Missile>().Fire(Damage, 0.3f, transform.position + new Vector3(-15f, 0));
        }
        
    }

    public override void TakingDamage(int TakeDamage)
    {
        if (HealthPoint - TakeDamage <= 0)
        {
            Death();
            UIControlerInstance.SetHP(0);
        }
        else
        {
            HealthPoint -= TakeDamage;
            UIControlerInstance.SetHP(HealthPoint);
        }
    }

    protected override void Death()
    {
        UIControlerInstance.ActivateDeathNotification();
        Destroy(gameObject);
    }

    protected override void AttackOnFlyingTargets()
    {
        GameObject HomingMissileObject = Instantiate(HomingMissile); //Создание снаряда из префаба
        HomingMissileObject.transform.position = transform.TransformPoint(UpFirePoint); //Перевод локальных координат в глобальные
        HomingMissileObject.GetComponent<HomingMissile>().Fire(Damage, 0.2f, Target.gameObject, EntityTypes.Player);
    }

    protected override void MelleAttack()
    {
        if(IsRight)
        {
            Vector3 PunchMediantPoint = transform.TransformPoint(RightPunchMediantPoint); //Перевод локальных координат в глобальные

            var Colliders = Physics2D.OverlapCircleAll(PunchMediantPoint, PunchRadius); //Получение коллайдеров папавших в радиус атаки

            GameObject PunchRadiusInstance = Instantiate(VisualizationPunch); //Создание объекта для визуализации
            PunchRadiusInstance.transform.position = transform.TransformPoint(RightPunchMediantPoint);

            foreach(var ObjectCollider in Colliders)
            {
                if(ObjectCollider.gameObject.tag == "Enemy" || ObjectCollider.gameObject.tag == "FlyingEnemy") //Если объект имеет тег Eneny или FlyingEnemy, он является противником и необходимо нанести урон
                {
                    Entity EntityInstance = ObjectCollider.gameObject.GetComponent<Entity>(); //Получение экземпляра класса Entity с целью получения информации о типе сущности

                    switch(EntityInstance.GetEntityType())
                    {
                        case EntityTypes.TrainingTarget:
                            TrainingTarget TrainingTargetInstance = ObjectCollider.gameObject.GetComponent<TrainingTarget>();
                            TrainingTargetInstance.TakingDamage(MeleeDamage);
                            Destroy(PunchRadiusInstance);
                            break;
                        case EntityTypes.FlyingTarget:
                            FlyingTarget FlyingTargetInstance = ObjectCollider.gameObject.GetComponent<FlyingTarget>();
                            FlyingTargetInstance.TakingDamage(MeleeDamage);
                            Destroy(PunchRadiusInstance);
                            break;
                        case EntityTypes.Missing:
                            Missing MissingInstance = ObjectCollider.gameObject.GetComponent<Missing>();
                            MissingInstance.TakingDamage(MeleeDamage);
                            Destroy(PunchRadiusInstance);
                            break;
                        case EntityTypes.RangeMissing:
                            RangeMissing RangeMissingInstance = ObjectCollider.gameObject.GetComponent<RangeMissing>();
                            RangeMissingInstance.TakingDamage(MeleeDamage);
                            Destroy(PunchRadiusInstance);
                            break;
                    }
                }
            }
        }
        else
        {
            Vector3 PunchMediantPoint = transform.TransformPoint(LeftPunchMediantPoint); //Перевод локальных координат в глобальные

            var Colliders = Physics2D.OverlapCircleAll(PunchMediantPoint, PunchRadius); //Получение коллайдеров папавших в радиус атаки

            GameObject PunchRadiusInstance = Instantiate(VisualizationPunch); //Создание объекта для визуализации
            PunchRadiusInstance.transform.position = transform.TransformPoint(LeftPunchMediantPoint);

            foreach(var ObjectCollider in Colliders)
            {
                if(ObjectCollider.gameObject.tag == "Enemy" || ObjectCollider.gameObject.tag == "FlyingEnemy") //Если объект имеет тег Eneny или FlyingEnemy, он является противником и необходимо нанести урон
                {
                    Entity EntityInstance = ObjectCollider.gameObject.GetComponent<Entity>(); //Получение экземпляра класса Entity с целью получения информации о типе сущности

                    switch(EntityInstance.GetEntityType())
                    {
                        case EntityTypes.TrainingTarget:
                            TrainingTarget TrainingTargetInstance = ObjectCollider.gameObject.GetComponent<TrainingTarget>();
                            TrainingTargetInstance.TakingDamage(MeleeDamage);
                            Destroy(PunchRadiusInstance);
                            break;
                        case EntityTypes.FlyingTarget:
                            FlyingTarget FlyingTargetInstance = ObjectCollider.gameObject.GetComponent<FlyingTarget>();
                            FlyingTargetInstance.TakingDamage(MeleeDamage);
                            Destroy(PunchRadiusInstance);
                            break;
                        case EntityTypes.Missing:
                            Missing MissingInstance = ObjectCollider.gameObject.GetComponent<Missing>();
                            MissingInstance.TakingDamage(MeleeDamage);
                            Destroy(PunchRadiusInstance);
                            break;
                        case EntityTypes.RangeMissing:
                            RangeMissing RangeMissingInstance = ObjectCollider.gameObject.GetComponent<RangeMissing>();
                            RangeMissingInstance.TakingDamage(MeleeDamage);
                            Destroy(PunchRadiusInstance);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Обнаруживает летающих противников в радиусе
    /// </summary>
    protected override void SearchTargets()
    {
        var Colliders = Physics2D.OverlapCircleAll(transform.position, 100f).ToList();
        Targets = (from c in Colliders where c.gameObject.tag == "FlyingEnemy" select c).ToList();
    }

    /// <summary>
    /// Изменяет выбранную цель
    /// </summary>
    private void SelectTarget()
    {
        Debug.Log($"Количество целей: {Targets.Count}\n TargetsCounter = {TargetsCounter}");

        if (TargetsCounter < Targets.Count)
        {
            Target = Targets[TargetsCounter];
            TargetIsActive = true;
            
            UIControlerInstance.DrawSight(Target);

            TargetsCounter++;
        }
        else
        {
            TargetsCounter = 0;

            Target = Targets[TargetsCounter];
            TargetIsActive = true;

            UIControlerInstance.DrawSight(Target);
        }
    }

    /// <summary>
    /// Вызвращает информацию о направлении движения игрока
    /// </summary>
    /// <returns>true - движение вправо; fasle - движение влево</returns>
    public bool GetDirection()
    {
        return IsRight;
    }
}
