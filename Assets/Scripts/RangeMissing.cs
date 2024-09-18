using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMissing : Entity
{
    [SerializeField] private float Speed;
    [SerializeField] private Vector3 RightBorder; //Граница области передвижения
    [SerializeField] private Vector3 LeftBorder; //Граница области передвижения
    [SerializeField] private Vector3 BoxSize; //Размер области поиска цели
    [SerializeField] private GameObject LeftHomingMissile; //Объект снаряда
    [SerializeField] private GameObject RightHomingMissile; //Объект снаряда
    [SerializeField] private GameObject MidHomingMissile; //Объект снаряда
    [SerializeField] private GameObject MissilePrefab; //Префаб снаряда

    private Rigidbody2D RB;
    private bool IsRight = true;
    private GameObject Target;
    private Vector3 LeftHomingMissilePosition; //Стартовая позиция левого снаряда
    private Vector3 RightHomingMissilePosition; //Стартовая позиция правого снаряда
    private Vector3 MidHomingMissilePosition; //Стартовая позиция центрального снаряда
    private int LeftMissileTimer; //Таймер "перезарядки" левого снаряда
    private int RightMissileTimer; //Таймер "перезарядки" правого снаряда
    private int MidMissileTimer; //Таймер "перезарядки" центрального снаряда

    private const int TimerStartValue = 30;

    private void Start() 
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
        //Сохранение начальных позиций снарядов
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

    /// <summary>
    /// Движение сущности по горизонтали
    /// </summary>
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

    /// <summary>
    /// Ищет игрока в заданной области
    /// </summary>
    protected override void SearchTargets()
    {
        var Colliders = Physics2D.OverlapBoxAll(transform.position, BoxSize, 2f); //Получение массива коллайдеров находящихся в указанной области

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

    /// <summary>
    /// Заставляет сущность двигаться в противоположном от игрока направлении
    /// </summary>
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

    /// <summary>
    /// Проверяет значение таймера. Если отсчет завершился создаёт новй снаряд. В противном случае уменьшает значение таймера на 1
    /// </summary>
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
