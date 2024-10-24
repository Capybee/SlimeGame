using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missing : Entity
{
    [SerializeField] private int Speed;
    [SerializeField] private Vector3 RightPunchPoint; //Локальная точка в пространстве, является центром окружности в которой проверяется попадание по противнику
    [SerializeField] private Vector3 LeftPunchPoint; //Локальная точка в пространстве, является центром окружности в которой проверяется попадание по противнику
    [SerializeField] private Vector3 RightBorder; //Граница области передвижения
    [SerializeField] private Vector3 LeftBorder; //Граница области передвижения
    [SerializeField] private Vector3 BoxSize; //Размер зоны обнаружения цели
    [SerializeField] private float StopDistacne; //Дистанция до цели при достижении которой Потерявшийся останавливается
    [SerializeField] private float PunchRadius; //Радиус в котором проверяется попадание по цели
    [SerializeField] private float PunchDistance; //Дистанция с которой возможен удар
    [SerializeField] private DropControler DropControlerInstance;

    private Rigidbody2D RB;
    private bool IsRight = true; //Движется ли объект вправо
    private GameObject Target;
    private bool Punch = false; //Совершен ли удар, при значении true удар невозможен
    private int Timer;
    private const int TimerStartValue = 30;

    private void Start() 
    {
        RB = gameObject.GetComponent<Rigidbody2D>();  
        Timer = TimerStartValue;  
    }

    private void FixedUpdate() 
    {
        Move();
        if(Target != null)
        {
            FollowPlayer();
        }
        if(Punch)
        {
            Timer += 1;
            Debug.Log($"Timer = {Timer}");
            if(Timer == TimerStartValue)
            {
                Punch = false;
            }
        }
    }

    private void Update() 
    {
        SearchTargets();
        if(Target != null)
        {
            if(Vector3.Distance(transform.position, Target.gameObject.transform.position) < PunchDistance && Timer == TimerStartValue) //Если дистанция до цели меньше дистанции атаки, а также удар разрешен вызывает метод удара
            {
                MelleAttack();
                Punch = true;
                Timer = 0;
            }
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

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, BoxSize);    
    }

    private void Move()
    {
        if(IsRight)
        {
            if(Vector3.Distance(transform.position, RightBorder) > 0.1f)
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
            if(Vector3.Distance(transform.position, LeftBorder) > 0.1f)
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
    /// Заставляет двигаться за игроком
    /// </summary>
    private void FollowPlayer()
    {
        if (Vector3.Distance(transform.position, Target.gameObject.transform.position) > StopDistacne)
        {
            Vector3 MoveVector = Target.gameObject.transform.position - transform.position;
            MoveVector.y = 0;

            RB.velocity = MoveVector;
        }
        else
        {
            RB.velocity = Vector3.zero;
        }
    }

    protected override void MelleAttack()
    {
        if(IsRight)
        {
            Vector3 PunchMediantPoint = transform.TransformPoint(RightPunchPoint); //Переводит локальные координаты в глобальные

            var Colliders = Physics2D.OverlapCircleAll(PunchMediantPoint, PunchRadius); //Получает массив коллайдеров попавших в радиус атаки

            foreach (var collider in Colliders)
            {
                if(collider.gameObject.tag == "Player")
                {
                    Player PlayerInstance = collider.gameObject.GetComponent<Player>();
                    PlayerInstance.TakingDamage(MeleeDamage);
                }
            }
        }
        else
        {
            Vector3 PunchMediantPoint = transform.TransformPoint(LeftPunchPoint); //Переводит локальные координаты в глобальные

            var Colliders = Physics2D.OverlapCircleAll(PunchMediantPoint, PunchRadius); //Получает массив коллайдеров попавших в радиус атаки

            foreach (var collider in Colliders)
            {
                if(collider.gameObject.tag == "Player")
                {
                    Player PlayerInstance = collider.gameObject.GetComponent<Player>();
                    PlayerInstance.TakingDamage(MeleeDamage);
                }
            }
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
        DropControlerInstance.Drop(EntityType, transform.position);
        Destroy(gameObject);
    }
}
