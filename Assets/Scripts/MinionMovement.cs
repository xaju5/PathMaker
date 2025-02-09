using System;
using UnityEngine;

public class MinionMovement : MonoBehaviour
{

    [SerializeField] float speed;
    Vector3 targetPoint;

    private Vector3 direction;

    void Start(){
        targetPoint = transform.position;
        direction = GetDirection();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            targetPoint = GetMouseWolrdPosition();
            Debug.Log(targetPoint);
        }

        direction = GetDirection();
        if(direction.magnitude < 0.01 ) return;
        
        MoveMinion();
    }

    private void MoveMinion()
    {
        transform.position = transform.position + direction.normalized * speed;    
    }

    private Vector3 GetDirection()
    {
        return targetPoint - transform.position;
    }

    private Vector3 GetMouseWolrdPosition(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, 1, 0));

        float distance;
        if (plane.Raycast(ray, out distance)) {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
}
