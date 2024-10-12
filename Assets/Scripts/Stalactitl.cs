using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalactitl : Entity
{
    [SerializeField] float Speed;
    [SerializeField] Vector3 BoxSize;
    
    private GameObject Target;
    private Rigidbody2D RB;

    private void Start() 
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
        RB.bodyType = RigidbodyType2D.Static;
    }

    private void Update() 
    {
        SearchTarget();
        if(Target != null)
        {
            Rotation();
        }
    }

    private void FixedUpdate() 
    {
        if(Target != null)
        {
            RB.bodyType = RigidbodyType2D.Dynamic;
            MoveTo();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        
        if(other.gameObject.tag != "Player" && other.gameObject.tag != "Enemy" && other.gameObject.tag != "FlyingEnemy")
        {
            Death();
        }
        else
        {
            Entity EntityInstance = other.gameObject.GetComponent<Entity>();

            switch(EntityInstance.GetEntityType())
            {
                 case EntityTypes.FlyingTarget:
                        FlyingTarget FlyingTargetInstance = other.gameObject.GetComponent<FlyingTarget>();
                        FlyingTargetInstance.TakingDamage(Damage);
                        Death();
                        break;
                    case EntityTypes.RangeMissing:
                        RangeMissing RangeMissingInstacne = other.gameObject.GetComponent<RangeMissing>();
                        RangeMissingInstacne.TakingDamage(Damage);
                        Death();
                        break;
                    case EntityTypes.TrainingTarget:
                        TrainingTarget TrainingTargetInstance = other.gameObject.GetComponent<TrainingTarget>();
                        TrainingTargetInstance.TakingDamage(Damage);
                        Death();
                        break;
                    case EntityTypes.Missing:
                        Missing MissingInstance = other.gameObject.GetComponent<Missing>();
                        MissingInstance.TakingDamage(Damage);
                        Death();
                        break;
                    case EntityTypes.Player:
                        Player PlayerInstance = other.gameObject.GetComponent<Player>();
                        PlayerInstance.TakingDamage(Damage);
                        Death();
                        break;
            }
        }    
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, BoxSize);    
    }

    private void SearchTarget()
    {
        var Colliders = Physics2D.OverlapBoxAll(transform.position, BoxSize, 2f);

        foreach (var c in Colliders)
        {
            if(c.gameObject.tag == "Player")
            {
                Target = c.gameObject;
                return;
            }
        }

        Target = null;
    }

    private void MoveTo()
    {
        Vector3 Direction = (Target.transform.position - transform.position).normalized;

        Vector2 Velocity = Direction * Speed;

        RB.velocity = Velocity;
    }

    private void Rotation()
    {
        Vector3 Direction = (Target.transform.position - transform.position).normalized;
        float Angle = Mathf.Atan2(Direction.y,Direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion CurrentRotation = transform.rotation;
        Quaternion TargetRotation = Quaternion.Euler(0,0,Angle); 
        transform.rotation = Quaternion.Lerp(CurrentRotation, TargetRotation, 1f);
    }

    protected override void Death()
    {
        Destroy(gameObject);
    }

}
