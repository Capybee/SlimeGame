using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMissing : Entity
{
    [SerializeField] private float Speed;
    [SerializeField] private Vector3 RightBorder;
    [SerializeField] private Vector3 LeftBorder;
    [SerializeField] private Vector3 BoxSize;
    [SerializeField] private GameObject LeftHomingMissile;
    [SerializeField] private GameObject RightHomingMissile;
    [SerializeField] private GameObject MidHomingMissile;
    [SerializeField] private GameObject MissilePrefab;

    private Rigidbody2D RB;
    private bool IsRight = true;
    private GameObject Target;
    private Vector3 LeftHomingMissilePosition;
    private Vector3 RightHomingMissilePosition;
    private Vector3 MidHomingMissilePosition;
    private int LeftMissileTimer;
    private int RightMissileTimer;
    private int MidMissileTimer;

    private const int TimerStartValue = 30;

    private void Start() 
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
        LeftHomingMissilePosition = LeftHomingMissile.transform.position;    
        RightHomingMissilePosition = RightHomingMissile.transform.position;    
        MidHomingMissilePosition = MidHomingMissile.transform.position;    
    }

    private void FixedUpdate() 
    {
        if(Target != null)
        {
            MoveOut();
        }
        Move();    
    }

    private void Update() 
    {
        SearchTargets();
        if (Target != null)
        {
            Attack();
        }
        Reload();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Missile")
        {
            Missile MissileInstance = other.gameObject.GetComponent<Missile>();

            TakingDamage(MissileInstance.GetDamage());
        }
    }

    private void Move()
    {
        if(IsRight)
        {
            if(Vector3.Distance(transform.position, RightBorder) > 0.2f)
            {
                RB.velocity = new Vector3(Speed, RB.velocity.y);
            }
            else
            {
                RB.velocity = new Vector3(-Speed, RB.velocity.y);
                IsRight = false;
            }
        }
        else
        {
            if(Vector3.Distance(transform.position, LeftBorder) > 0.2f)
            {
                RB.velocity = new Vector3(-Speed, RB.velocity.y);
            }
            else
            {
                RB.velocity = new Vector3(Speed, RB.velocity.y);
                IsRight = true;
            }
        }
    }

    protected override void SearchTargets()
    {
        var Colliders = Physics2D.OverlapBoxAll(transform.position, BoxSize, 2f);

        foreach (var collider in Colliders)
        {
            if(collider.gameObject.tag == "Player")
            {
                Target = collider.gameObject;
                return;
            }
        }
        Target = null;
    }

    public void MoveOut()
    {
        Player PlayerInstance = Target.gameObject.GetComponent<Player>();
        if(PlayerInstance.GetDirection())
        {
            IsRight = true;
        }
        else
        {
            IsRight = false;
        }

    }

    private void Reload()
    {
        if(LeftHomingMissile == null)
        {
            if(LeftMissileTimer == 0)
            {
                LeftHomingMissile = Instantiate(MissilePrefab);
                LeftHomingMissile.transform.position = LeftHomingMissilePosition;
            }
            else
            {
                LeftMissileTimer -= 1;
            }
        }
        else if(RightHomingMissile == null)
        {
            if(RightMissileTimer == 0)
            {
                RightHomingMissile = Instantiate(MissilePrefab);
                RightHomingMissile.transform.position = RightHomingMissilePosition;
            }
            else
            {
                RightMissileTimer -= 1;
            }
        }
        else if(MidHomingMissile == null)
        {
            if(MidMissileTimer == 0)
            {
                MidHomingMissile = Instantiate(MissilePrefab);
                MidHomingMissile.transform.position = MidHomingMissilePosition;
            }
            else
            {
                MidMissileTimer -= 1;
            }
        }

    }

    protected override void Attack()
    {
        if(LeftHomingMissile != null)
        {
            HomingMissile HomingMissileInstance = LeftHomingMissile.gameObject.GetComponent<HomingMissile>();
            HomingMissileInstance.Fire(Damage, 0.1f, Target.transform.position, EntityType);
            LeftMissileTimer = TimerStartValue;
        }
        else if(RightHomingMissile != null)
        {
            HomingMissile HomingMissileInstance = RightHomingMissile.gameObject.GetComponent<HomingMissile>();
            HomingMissileInstance.Fire(Damage, 0.1f, Target.transform.position, EntityType);
            RightMissileTimer = TimerStartValue;
        }
        else if (MidHomingMissile != null)
        {
            HomingMissile HomingMissileInstance = MidHomingMissile.gameObject.GetComponent<HomingMissile>();
            HomingMissileInstance.Fire(Damage, 0.1f, Target.transform.position, EntityType);
            MidMissileTimer = TimerStartValue;
        }
    }

    public override void TakingDamage(int TakeDamage)
    {
        if((HealthPoint -= TakeDamage) > 0)
        {
            HealthPoint -= TakeDamage;
            Debug.Log($"Получено урона: {TakeDamage}");
            Debug.Log($"Здоровья осталось: {HealthPoint}");
        }
        else
        {
            Death();
        }
    }

    protected override void Death()
    {
        Destroy(gameObject);
    }
}
