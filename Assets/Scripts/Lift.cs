using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] private Vector3 FinishPosition;
    [SerializeField] private float Speed;
    [SerializeField] private KeyCode StartKey;
    [SerializeField] private UIControler UIControlerInstance;

    private Vector3 StartPosition;
    private Rigidbody2D RB;
    private bool IsMooving;
    private bool PlayerStep; //Находится ли игрок на платформе
    private bool Up; //Движется ли лифт вверх

    private void Start() 
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
        StartPosition = transform.position;
    }

    private void Update() 
    {
        if(Input.GetKeyDown(StartKey) && PlayerStep)
        {
            if(transform.position == StartPosition)
            {
                IsMooving = true;
                Up = true;
                UIControlerInstance.HideHint();
            }
            else
            {
                IsMooving = true;
                Up = false;
                UIControlerInstance.HideHint();
            }
        }
    }

    private void FixedUpdate() 
    {
        if(IsMooving)
        {
            Move();
        }    
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.tag == "Player" && !IsMooving)
        {
            UIControlerInstance.ShowHint();
            PlayerStep = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
            if(other.gameObject.tag == "Player")
        {
            UIControlerInstance.HideHint();
            PlayerStep = false;
        }
    }

    private void Move()
    {
        if(Up)
        {
            if(Vector3.Distance(transform.position, FinishPosition) > 0.01f) 
            {
                RB.velocity = new Vector3(RB.velocity.x, Speed);
            }
            else
            {
                RB.velocity = Vector3.zero;
                IsMooving = false;
            }
        }
        else
        {
            if(Vector3.Distance(transform.position, StartPosition) > 0.01f) 
            {
                RB.velocity = new Vector3(RB.velocity.x, -Speed);
            }
            else
            {
                RB.velocity = Vector3.zero;
                IsMooving = false;
            }
        }
    }
}
