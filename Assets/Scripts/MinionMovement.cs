using System;
using UnityEngine;

public class MinionMovement : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] Transform pointATransform, pointBTransform;
    Vector3 pointA, pointB;

    private Vector3 direction;

    void Awake(){
        pointA = pointATransform.position;
        pointB = pointBTransform.position;
        transform.position = pointA;
    }
    void Start(){
        direction = GetDirection(pointB);
    }
    void Update()
    {
        MoveMinio();
        UpdateDirection();
    }

    private void MoveMinio()
    {
        transform.position = transform.position + direction.normalized * speed;    
    }

    private Vector3 GetDirection(Vector3 target)
    {
        return target - transform.position;
    }

    private void UpdateDirection(){
        if((transform.position - pointA).magnitude <= speed )
            direction = GetDirection(pointB);

        if((transform.position - pointB).magnitude <= speed )
            direction = GetDirection(pointA);
    }
}
