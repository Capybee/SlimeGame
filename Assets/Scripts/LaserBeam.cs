using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private float _Speed;
    private int _Damage;
    private Vector3 _Target;

    /// <summary>
    /// Запускает снаряд в цель которой является абстрактная точка перед сущностью
    /// </summary>
    /// <param name="Damage">Урон снаряда</param>
    /// <param name="Speed">Скорость снаряда</param>
    /// <param name="Target">Точка перед сущностью</param>
    public void Fire(int Damage, float Speed, Vector3 Target)
    {
        _Speed = Speed;
        _Damage = Damage;
        _Target = Target;

        Vector2 Direction = _Target - transform.position;

        float Angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0,0, Angle - 90f));
    }

    private void FixedUpdate() 
    {
        transform.position = Vector3.MoveTowards(transform.position, _Target, _Speed);

        if (Vector3.Distance(transform.position, _Target) < 0.01f)
        {
            Destroy(gameObject, 0.2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        Entity EntityInstanceCase2 = other.gameObject.GetComponent<Entity>();
            if(EntityInstanceCase2.GetEntityType() == EntityTypes.Player)
            {
                Player PlayerInstance = other.gameObject.GetComponent<Player>();
                PlayerInstance.TakingDamage(_Damage);
                Destroy(gameObject);
            }
    }
}
