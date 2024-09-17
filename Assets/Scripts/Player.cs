using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private KeyCode AttackKey;
    [SerializeField] private Vector3 RightFirePoint;
    [SerializeField] private Vector3 LeftFirePoint;
    [SerializeField] private  Vector3 UpFirePoint;
    [SerializeField] private GameObject Missile;
    [SerializeField] private GameObject HomingMissile;
    [SerializeField] private UIControler UIControlerInstance;
    [SerializeField] private KeyCode SearchKey;
    [SerializeField] private KeyCode StopTargetSearchKey;
    [SerializeField] private KeyCode HommingFireKey;
    [SerializeField] private Vector3 RightPunchMediantPoint;
    [SerializeField] private Vector3 LeftPunchMediantPoint;
    [SerializeField] private float PunchRadius;
    [SerializeField] private GameObject VisualizationPunch; 
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
        GameObject MissileObject = Instantiate(Missile);
        if(IsRight)
        {
            MissileObject.transform.position = transform.TransformPoint(RightFirePoint);
            MissileObject.GetComponent<Missile>().Fire(Damage, 0.3f, transform.position + new Vector3(15f, 0));
        }
        else
        {
            MissileObject.transform.position = transform.TransformPoint(LeftFirePoint);
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
        GameObject HomingMissileObject = Instantiate(HomingMissile);
        HomingMissileObject.transform.position = transform.TransformPoint(UpFirePoint);
        HomingMissileObject.GetComponent<HomingMissile>().Fire(Damage, 0.2f, Target.transform.position, EntityTypes.Player);
    }

    protected override void MelleAttack()
    {
        if(IsRight)
        {
            Vector3 PunchMediantPoint = transform.TransformPoint(RightPunchMediantPoint);

            var Colliders = Physics2D.OverlapCircleAll(PunchMediantPoint, PunchRadius);

            GameObject PunchRadiusInstance = Instantiate(VisualizationPunch);
            PunchRadiusInstance.transform.position = transform.TransformPoint(RightPunchMediantPoint);

            foreach(var ObjectCollider in Colliders)
            {
                if(ObjectCollider.gameObject.tag == "Enemy" || ObjectCollider.gameObject.tag == "FlyingEnemy")
                {
                    Entity EntityInstance = ObjectCollider.gameObject.GetComponent<Entity>();

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
            Vector3 PunchMediantPoint = transform.TransformPoint(LeftPunchMediantPoint);

            var Colliders = Physics2D.OverlapCircleAll(PunchMediantPoint, PunchRadius);

            GameObject PunchRadiusInstance = Instantiate(VisualizationPunch);
            PunchRadiusInstance.transform.position = transform.TransformPoint(LeftPunchMediantPoint);

            foreach(var ObjectCollider in Colliders)
            {
                if(ObjectCollider.gameObject.tag == "Enemy" || ObjectCollider.gameObject.tag == "FlyingEnemy")
                {
                    Entity EntityInstance = ObjectCollider.gameObject.GetComponent<Entity>();

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

    protected override void SearchTargets()
    {
        var Colliders = Physics2D.OverlapCircleAll(transform.position, 100f).ToList();
        Targets = (from c in Colliders where c.gameObject.tag == "FlyingEnemy" select c).ToList();
    }

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
