using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D RB;
    private SpriteRenderer SR;
    private bool OnGround = false;
    private bool IsRight = true;
    private Player PlayerInstance;
    private bool IsDash = false;
    private int DashTimer = 0;

    private const int DASHTIMERSTARTVALUE = 150;

    [SerializeField] private KeyCode LeftKey;
    [SerializeField] private KeyCode RightKey;
    [SerializeField] private KeyCode JumpKey;
    [SerializeField] private KeyCode DashKey;
    [SerializeField] private float Speed;
    [SerializeField] private float JumpForce;
    [SerializeField] private float DashForce;

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        PlayerInstance = gameObject.GetComponent<Player>();
    }

    void Update()
    {
        if(Input.GetKeyDown(JumpKey) && OnGround)
        {
            Jump();
        }
        if(Input.GetKeyDown(DashKey))
        {
            IsDash = true;
        }

    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.tag == "Ground" || other.gameObject.tag == "Lift")
        {
            OnGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if(other.gameObject.tag == "Ground" || other.gameObject.tag == "Lift")
        {
            OnGround = false;
        }
    }

    private void FixedUpdate() 
    {
        
        if(IsDash)
        {
            Dash();
            IsDash = false;
        }
        
        if(DashTimer != 0)
        {
            DashTimer--;
        }

        float Horizontal = 0; //Значение скорости по горизонтали

        if(Input.GetKey(LeftKey))
        {
            Horizontal = -Speed;
            SR.flipX = true;
            IsRight = false;
        }

        if(Input.GetKey(RightKey))
        {
            Horizontal = Speed;
            SR.flipX = false;
            IsRight = true;
        }

        RB.velocity = new Vector2(Horizontal, RB.velocity.y); 
    }

    private void Jump()
    {
        RB.AddForce(new Vector2(RB.velocity.x, JumpForce), ForceMode2D.Impulse);
    }

    private void Dash()
    {
        if(DashTimer == 0)
        {
            if(IsRight)
            {
                Vector2 NewPosition = new Vector2(transform.position.x + 10f, transform.position.y);
                if(!Physics2D.OverlapPoint(NewPosition)) //Проверка на наличие припятствий в финальной точке рывка
                {
                    transform.position = NewPosition;
                    DashTimer = DASHTIMERSTARTVALUE;
                }
                else
                {
                    Debug.Log("Попадается припятствие");
                    PlayerInstance.TakingDamage(10);
                }
            }
            else
            {
                Vector2 NewPosition = new Vector2(transform.position.x - 10f, transform.position.y);
                if(!Physics2D.OverlapPoint(NewPosition)) //Проверка на наличие припятствий в финальной точке рывка
                {
                    transform.position = NewPosition;
                    DashTimer = DASHTIMERSTARTVALUE;
                }
                else
                {
                    Debug.Log("Попадается припятствие");
                    PlayerInstance.TakingDamage(10);
                }
            }
        }
    }
}
