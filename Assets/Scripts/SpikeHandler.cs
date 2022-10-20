using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHandler : MonoBehaviour
{
    private int damage = 50;
    private bool collidePlayer;
    private PlayerStatus playerStatus;
    private Transform playerTrans;

    void Start()
    {
        collidePlayer = false;
        playerStatus = GameObject.Find("Vajra").GetComponent<PlayerStatus>();
        playerTrans = GameObject.Find("Vajra").transform;
    }

    // void Update()
    // {
    //     // probably need to change to a coroutine style like the enemyHandler
    //     // if is colliding player
    //     if (collidePlayer){
    //         collidePlayerLogic();
    //     }
    // }

    // check if the player gets hit.
    void collidePlayerLogic(){
        // if immunity cooldown ends, the player gets hit
        if (Time.time - playerStatus.getLastHitTime() > playerStatus.immunityCooldown){
            foreach(Transform child in playerTrans){
                if (child.name == "Canvas"){
                    playerStatus.setHP(playerStatus.getHP() - damage);
                    // currently the enemey damage is fixed, modify later.
                    // playerStatus.hp -= damage;
                    child.GetComponent<PlayerHealthBar>().SetHealthBar(playerStatus.getHP(), playerStatus.hpCap, damage);
                    // reset elapsed immunity cooldown
                    playerStatus.setLastHitTime(Time.time);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider){
        if (collider.name == "Vajra"){
            // reset the collision variable.
            collidePlayer = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.name == "Vajra"){
            collidePlayer = true;
            //collidePlayerLogic();
            StartCoroutine(collidePlayerCheck(Time.deltaTime));
        }
    }

    IEnumerator collidePlayerCheck(float delay){
        while (collidePlayer){
            collidePlayerLogic();
            yield return new WaitForSeconds(delay);
        }
    }
}
