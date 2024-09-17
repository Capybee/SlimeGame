using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missing : Entity
{
    [SerializeField] private int Speed;
    [SerializeField] private Vector3 RightPunchPoint;
    [SerializeField] private Vector3 LeftPunchPoint;
    [SerializeField] private Vector3 RightBorder;
    [SerializeField] private Vector3 LeftBorder;
    [SerializeField] private Vector3 BoxSize;
    [SerializeField] private float StopDistacne;
    [SerializeField] private float PunchRadius;
    [SerializeField] private float PunchDistance;

    private Rigidbody2D RB;
    private bool IsRight = true;
    private GameObject Target;
    private bool Punch = false;
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
            if(Vector3.Distance(transform.position, Target.gameObject.transform.position) < PunchDistance && Timer == TimerStartValue)
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
            Debug.Log("Удар впараво");
            Vector3 PunchMediantPoint = transform.TransformPoint(RightPunchPoint);

            var Colliders = Physics2D.OverlapCircleAll(PunchMediantPoint, PunchRadius);

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
            Debug.Log("Удар влево");
            Vector3 PunchMediantPoint = transform.TransformPoint(LeftPunchPoint);

            var Colliders = Physics2D.OverlapCircleAll(PunchMediantPoint, PunchRadius);

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
        Destroy(gameObject);
    }
}
