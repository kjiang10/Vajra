using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovingPlatformHandler : MonoBehaviour
{
    public Vector3 targetLocation;
    public float movingSpeed;
    private Vector3 startLocation;
    private Transform playerTrans;
    private int step;
    private int currentStep;
    private float deltaDistanceX;
    private float deltaDistanceY;
    private float positionX;
    private float positionY;


    // Start is called before the first frame update
    void Start()
    {
        startLocation = transform.position;
        float diffX = targetLocation.x - startLocation.x;
        float diffY = targetLocation.y - startLocation.y;
        float distance = Mathf.Sqrt(diffX * diffX + diffY * diffY);
        
        float deltaDistance = Time.fixedDeltaTime * movingSpeed;
        // how many FixedUpdate() will be made to get to the destination
        step = Mathf.FloorToInt(distance / deltaDistance);
        currentStep = 0;
        deltaDistanceX = deltaDistance / distance * diffX;
        deltaDistanceY = deltaDistance / distance * diffY;
        positionX = startLocation.x;
        positionY = startLocation.y;

        playerTrans = GameObject.Find("Vajra").transform;

    }
    // optimize using Rigidbody.MovePosition
    void FixedUpdate(){
        if (currentStep == 2 * step){
            currentStep = 0;
        }
        // if (currentStep % (step * 2) == 1){
        //     currentStep = 0;
        // }
        // heading to target
        if (currentStep < step){
            positionX += deltaDistanceX;
            positionY += deltaDistanceY;
            
        }
        // heading to start
        else{
            positionX -= deltaDistanceX;
            positionY -= deltaDistanceY;
        }
        transform.position = new Vector3(positionX, positionY, 0f);
        currentStep += 1;
    }

}
