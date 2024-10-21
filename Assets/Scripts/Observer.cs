using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Observer : Entity
{
    [SerializeField] private float Speed;
    [SerializeField] private float SearchSphereRadius;
    [SerializeField] private GameObject MissilePref;
    [SerializeField] private Vector2 RightBounds;
    [SerializeField] private Vector2 LeftBounds;
    [SerializeField] private float SearchRadius;
    [SerializeField] private float SavedDistanceToPlayer;

    private Rigidbody2D RB;
    private GameObject Target;
    private Vector2 TargetPosition;
    private bool IsWaiting = false;
    private float WaitTime = 1.5f;

    private void Start() 
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
        SetRandomTargetPosition();    
    }

    private void Update() 
    {
        SearchPlayer();    
    }

    private void FixedUpdate() 
    {
        if(!IsWaiting)
        {
            Move();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 BottomLeft = new Vector2(RightBounds.x, RightBounds.y);
        Vector2 BottomRight = new Vector2(LeftBounds.x, RightBounds.y);
        Vector2 TopLeft = new Vector2(RightBounds.x, LeftBounds.y);
        Vector2 TopRight = new Vector2(LeftBounds.x, LeftBounds.y);

        Gizmos.DrawLine(BottomLeft, BottomRight);  // Нижняя граница
        Gizmos.DrawLine(BottomRight, TopRight);    // Правая граница
        Gizmos.DrawLine(TopRight, TopLeft);        // Верхняя граница
        Gizmos.DrawLine(TopLeft, BottomLeft);      // Левая граница

        Gizmos.DrawWireSphere(transform.position, SearchRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SavedDistanceToPlayer);
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, TargetPosition, Speed);

        if(Vector2.Distance(transform.position, TargetPosition) < 0.2f)
        {
            StartCoroutine(Wait());
        }
    }

    private void SetRandomTargetPosition()
    {
        float RandomX = UnityEngine.Random.Range(RightBounds.x, LeftBounds.x);
        float RandomY = UnityEngine.Random.Range(RightBounds.y, LeftBounds.y);

        TargetPosition = new Vector2(RandomX, RandomY);
    }

    public IEnumerator Wait()
    {
        IsWaiting = true;
        yield return new WaitForSeconds(WaitTime);
        if(Target != null)
        {
            EscapeFronPlayer();
        }
        else
        {
            SetRandomTargetPosition();
        }
        IsWaiting = false;
    }

    private void SearchPlayer()
    {
        var Colliders = Physics2D.OverlapCircleAll(transform.position, SearchRadius);

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

    private void EscapeFronPlayer()
    {
        float RandomX = UnityEngine.Random.Range(RightBounds.x, LeftBounds.x);
        float RandomY = UnityEngine.Random.Range(RightBounds.y, LeftBounds.y);

        if(Vector2.Distance(new Vector2(RandomX, RandomY), Target.transform.position) > SavedDistanceToPlayer)
        {
            TargetPosition = new Vector2(RandomX, RandomY);
        }
    }
}
