using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int hpCap = 1000;
    private int hp;
    public int atk = 10;
    public float criticalRate = 0.1f;
    public float immunityCooldown = 1.5f;
    private float lastHitTime;
    // Start is called before the first frame update
    void Start()
    {
        hp = hpCap;
        lastHitTime = -100f;
        Application.targetFrameRate = 240;
    }

    public int getHP(){
        return hp;
    }
    public void setHP(int newHP){
        hp = newHP;
    }

    public float getLastHitTime(){
        return lastHitTime;
    }

    public void setLastHitTime(float newLastHitTime){
        lastHitTime = newLastHitTime;
    }
    // // Update is called once per frame
    // void Update()
    // {

    // }
}
