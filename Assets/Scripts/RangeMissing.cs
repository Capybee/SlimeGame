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
    [SerializeField] private UIControler UIControlerInstance;

    private Rigidbody2D RB;
    private bool IsRight = true;
    private GameObject Target;
    private Vector3 LeftHomingMissilePosition; //Стартовая позиция левого снаряда
    private Vector3 RightHomingMissilePosition; //Стартовая позиция правого снаряда
    private Vector3 MidHomingMissilePosition; //Стартовая позиция центрального снаряда
    private int LeftMissileTimer; //Таймер "перезарядки" левого снаряда
    private int RightMissileTimer; //Таймер "перезарядки" правого снаряда
    private int MidMissileTimer; //Таймер "перезарядки" центрального снаряда
    private bool IsLeftFired = false;
    private bool IsRightFired = false;
    private bool IsCentrFired = false;
    private bool IsReload = false;
    private bool IsWait = false;
    private int WaitTimer;

    private const int TIMERSTARTVALUE = 370;
    private const int WAITSTARTVALUE = 120;

    private void Start() 
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
        //Сохранение начальных позиций снарядов
        LeftHomingMissilePosition = transform.InverseTransformPoint(LeftHomingMissile.transform.position);    
        RightHomingMissilePosition = transform.InverseTransformPoint(RightHomingMissile.transform.position);    
        MidHomingMissilePosition = transform.InverseTransformPoint(MidHomingMissile.transform.position);   
        UIControlerInstance.SetLeftMissileStatusContent("Готов"); 
        UIControlerInstance.SetRightMissileStatusContent("Готов");
        UIControlerInstance.SetMidMissileStatusContent("Готов");
    }

    private void FixedUpdate() 
    {
        if(Target != null)
        {
            MoveOut();
        }
        Move(); 
        if(IsReload)
        {
            Reload();
        }   
        if(IsWait)
        {
            Wait();
        }
    }

    private void Update() 
    {
        SearchTargets();
        if (Target != null)
        {
            Attack();
        }
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
        if(IsLeftFired)
        {
            UIControlerInstance.SetLeftMissileStatusContent($"Таймер левого снаряда: {LeftMissileTimer}");
            if(LeftMissileTimer == 0)
            {
                IsLeftFired = false;
                LeftHomingMissile.SetActive(true);
                UIControlerInstance.SetLeftMissileStatusContent("Готов");
            }
            else
            {
                LeftMissileTimer--;
            }
        }
        if(IsRightFired)
        {
            UIControlerInstance.SetRightMissileStatusContent($"Таймер правого снаряда: {RightMissileTimer}");
            if(RightMissileTimer == 0)
            {
                IsRightFired = false;
                RightHomingMissile.SetActive(true);
                UIControlerInstance.SetRightMissileStatusContent("Готов");
            }
            else
            {
                RightMissileTimer--;
            }
        }
        if(IsCentrFired)
        {
            UIControlerInstance.SetMidMissileStatusContent($"Таймер центрального снаряда: {MidMissileTimer}");
            if(MidMissileTimer == 0)
            {
                IsCentrFired = false;
                MidHomingMissile.SetActive(true);
                UIControlerInstance.SetMidMissileStatusContent("Готов");
            }
            else
            {
                MidMissileTimer--;
            }
        }
    }

    private void Wait()
    {
        if(WaitTimer == 0)
        {
            IsWait = false;
        }
        else
        {
            WaitTimer--;
        }
    }

    protected override void Attack()
    {
        if(!IsLeftFired && !IsWait)
        {
            GameObject HomingMissileObject = Instantiate(MissilePrefab);
            HomingMissileObject.transform.position = transform.TransformPoint(LeftHomingMissilePosition);
            HomingMissileObject.GetComponent<HomingMissile>().Fire(Damage, 0.1f, Target.gameObject, EntityType);
            LeftHomingMissile.SetActive(false);
            IsLeftFired = true;
            LeftMissileTimer = TIMERSTARTVALUE;
            IsReload = true;
            IsWait = true;
            WaitTimer = WAITSTARTVALUE;
        }
        else if(!IsCentrFired && !IsWait)
        {
            GameObject HomingMissileObject = Instantiate(MissilePrefab);
            HomingMissileObject.transform.position = transform.TransformPoint(MidHomingMissilePosition);
            HomingMissileObject.GetComponent<HomingMissile>().Fire(Damage, 0.1f, Target.gameObject, EntityType);
            MidHomingMissile.SetActive(false);
            IsCentrFired = true;
            MidMissileTimer = TIMERSTARTVALUE;
            IsReload = true;
            IsWait = true;
            WaitTimer = WAITSTARTVALUE;
        }
        else if (!IsRightFired && !IsWait)  
        {
            GameObject HomingMissileObject = Instantiate(MissilePrefab);
            HomingMissileObject.transform.position = transform.TransformPoint(RightHomingMissilePosition);
            HomingMissileObject.GetComponent<HomingMissile>().Fire(Damage, 0.1f, Target.gameObject, EntityType);
            RightHomingMissile.SetActive(false);
            IsRightFired = true;
            RightMissileTimer = TIMERSTARTVALUE;
            IsReload = true;
            IsWait = true;
            WaitTimer = WAITSTARTVALUE;
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
