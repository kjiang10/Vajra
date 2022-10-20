using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PlayerMovement : MonoBehaviour
{
    public int hitTotalCount = 0;
    // int hitCount = 0;
    public CharacterController2D controller;
    SkeletonAnimation skeletonAnimation;
    public float runSpeed = 40f;
    float singleAttackInterval = 0.3f;
    float horizontalMove = 0f;
    bool jump = false;
    bool singleAttack = false;
    float singleAttackTime = 0;
    public bool hitCheck = false;
    int attackStatus = 0;
    float lastAttackTime = 0f;
    // if the player does not attack for a long time, reset attackStatus
    float attackStatusUpdateDuration = 0.45f;
    // Start is called before the first frame update
    void Start(){
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (skeletonAnimation == null){
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(Time.time - singleAttackTime);
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetKeyDown(KeyCode.K)){
            jump = true;
        }
        if (Input.GetKeyDown(KeyCode.J) && Time.time - singleAttackTime > singleAttackInterval){
            singleAttack = true;
            // update hitTotaltimes including empty attack
            hitTotalCount += 1;
        }
    }

    void FixedUpdate(){
        // interrupted can also be served as a signal to reset hit detection.
        bool interrupted = singleAttack;
        if (singleAttack){
            // hitCount += 1;
            // Debug.Log(hitCount);
            if (Time.time - lastAttackTime > attackStatusUpdateDuration){
                attackStatus = -1;
            }
            lastAttackTime = Time.time;
            // Debug.Log(hitCheck);
            string animationName = "single_attack";
            // if enemy has been hit before, upgrade animation
            if (hitCheck){
                // Debug.Log(attackStatus);
                if (attackStatus <= 1){
                    attackStatus += 1;
                }
                hitCheck = false;
            }
            // if no enemy has been hit
            else{
                // reset animation to single attack
                attackStatus = 0;
            }
            if (attackStatus == 1){
                animationName = "double_attack";
            }
            if (attackStatus == 2){
                animationName = "triple_attack";
                // reset attack after performing triple attack
                attackStatus = -1;
            }
            // notify SAHitDetection.cs in weaponHitBox
            // GameObject.Find("WeaponHitBox").GetComponent<SAHitDetection>().hitCheck = true;
            skeletonAnimation.state.SetAnimation(0, animationName, false);
            singleAttackTime = Time.time;
            singleAttack = false;
            
            // skeletonAnimation.state.AddAnimation(0, "run", true, 1);
        }
        // should reserve an interval so that attack animation can be fully played
        // when reserved, the player should also not flip, important!!!!!!!!!!
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump, interrupted, Time.time);
        jump = false;
        
        // Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        // if (rb.IsSleeping()){
        //     Debug.Log("sleeping");
        //     skeletonAnimation.state.SetAnimation(0, "idle", true);
        // }
    }
}
