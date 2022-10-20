using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovingPlatformHandler : MonoBehaviour
{
    public Vector3 center;
    public float movingSpeed;
    public float angle;
    private float currentAngle;
    private Vector3 startLocation;
    private float radius;
    private Transform playerTrans;
    
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log(transform.position);
        startLocation = transform.position;
        float diffX = startLocation.x - center.x;
        float diffY = startLocation.y - center.y;
        radius = Mathf.Sqrt(diffX * diffX + diffY * diffY);
        playerTrans = GameObject.Find("Vajra").transform;
        // Debug.Log(radius);
    }
    // optimize using Rigidbody.MovePosition
    void FixedUpdate(){
        float positionX = center.x + Mathf.Cos(currentAngle) * radius;
        float positionY = center.y + Mathf.Sin(currentAngle) * radius;
        
        currentAngle += Time.fixedDeltaTime * movingSpeed;
        if (currentAngle >= 360f){
            currentAngle = 0f;
        }
        transform.position = new Vector3(positionX, positionY, 0f);
    }
    // void OnBecameVisible(){
    //     enabled = true;
    // }
}
