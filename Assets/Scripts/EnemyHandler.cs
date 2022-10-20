using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    int prevHitTotalCount = 0;
    // int hitCount = 0;
    private HealthBar healthBar;
    private Color hitColor = Color.red;
    public int hpCap = 100;
    private int hp;
    // lastHitTime denotes the time being last hit by the player
    float lastHitTime = -100f;
    float healthBarMaxDuration = 3.0f;
    public int damage = 10;
    private bool collidePlayer;
    private PlayerStatus playerStatus;
    private Transform playerTrans;
    // note, if a player do not attack for a long time, reset the attackStatuas
    // todo

    // Start is called before the first frame update
    void Start()
    {
        // healthBar(Canvas) should always be the first child of the current enemy
        foreach(Transform child in transform){
            if (child.name == "Canvas"){
                healthBar = child.gameObject.GetComponent<HealthBar>();
                break;
            }
        } 
        hp = hpCap;
        collidePlayer = false;
        playerStatus = GameObject.Find("Vajra").GetComponent<PlayerStatus>();
        playerTrans = GameObject.Find("Vajra").transform;
    }



    // void FixedUpdate(){
    //     float diffX = transform.position.x - playerTrans.position.x;
    //     float diffY = transform.position.y - playerTrans.position.y;
    //     if (Mathf.Sqrt(diffX * diffX + diffY * diffY) > 40){
    //         enabled = false;
    //     }
    // }


    // check if the player gets hit.
    void collidePlayerLogic(){
        // if immunity cooldown ends, the player gets hit
        if (Time.time - playerStatus.getLastHitTime() > playerStatus.immunityCooldown){
            foreach(Transform child in playerTrans){
                if (child.name == "Canvas"){
                    // currently the enemey damage is fixed, modify later.
                    playerStatus.setHP(playerStatus.getHP() - damage);
                    //playerStatus.hp -= damage;
                    child.GetComponent<PlayerHealthBar>().SetHealthBar(playerStatus.getHP(), playerStatus.hpCap, damage);
                    // reset elapsed immunity cooldown
                    playerStatus.setLastHitTime(Time.time);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider){
        // if player exit then get damage.
        // todo
        if (collider.name == "Vajra"){
            // reset the collision variable.
            collidePlayer = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.name == "Vajra"){
            collidePlayer = true;
            // collidePlayerLogic();
            StartCoroutine(collidePlayerCheck(Time.deltaTime));

        }
        bool singleAttack = collider.gameObject.name == "SingleAttackHitBox";
        bool doubleAttack = collider.gameObject.name == "DoubleAttackHitBox";
        bool tripleAttack = collider.gameObject.name == "TripleAttackHitBox";
        // Note this has to be changed later to be compatible with other types of attack.
        if (singleAttack || doubleAttack || tripleAttack){
            // if hit total count changed, it means a new attack is performed. Collision detection can be detected again.
            if (prevHitTotalCount != GameObject.Find("Vajra").GetComponent<PlayerMovement>().hitTotalCount){
                prevHitTotalCount = GameObject.Find("Vajra").GetComponent<PlayerMovement>().hitTotalCount;
                // hitcount is just for testing
                // hitCount += 1;
                // Debug.Log("eagle get hit " + hitCount);
                // notify PlayMovement to change animation
                GameObject.Find("Vajra").GetComponent<PlayerMovement>().hitCheck = true;
                // hp decreasing amount depends on the weapon type.
                // Debug.Log(collider.gameObject);
                int baseDamage = GameObject.Find("Vajra").GetComponent<PlayerStatus>().atk;
                int damage = 0;
                if (singleAttack){
                    damage = baseDamage;
                }
                if (doubleAttack){
                    damage = baseDamage * 2;
                }
                if (tripleAttack){
                    damage = baseDamage * 5;
                }
                hp -= damage;
                // Debug.Log(hp);
                if (hp <= 0){
                    Destroy(gameObject);
                    return;
                }
                // Debug.Log(hp);
                healthBar.SetHealthBar(hp, hpCap, damage);
                // healthBar.SetHealth(hp, hpCap);

                lastHitTime = Time.time;
                // hide health bar 
                StartCoroutine(hideHealthBar(healthBarMaxDuration));

                // flash only if the enemy does not die
                SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                renderer.color = hitColor;
                StartCoroutine(toOrigColor(renderer, 0.1f));
                // reset hitSignal
            }
            // else nothing happened
            
            
        }
    }

    IEnumerator collidePlayerCheck(float delay){
        while (collidePlayer){
            collidePlayerLogic();
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator hideHealthBar(float delay){
        yield return new WaitForSeconds(delay);
        if (Time.time - lastHitTime > healthBarMaxDuration){
            healthBar.hideHealthBar();
        }
    }

    IEnumerator toOrigColor(SpriteRenderer renderer, float delay){
        yield return new WaitForSeconds(delay);
        renderer.color = Color.white;
        // Debug.Log("Should be called when being hit");
    }
    
    // void OnBecameVisible(){
    //     enabled = true;
    // }

    // void OnBecameInvisible(){
    //     enabled = false;
        
    // }
    
}