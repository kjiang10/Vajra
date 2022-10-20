using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour
{
    private float elapsedTime;
    private int damage;
    private float damageNumberDuration;
    private List<GameObject> currentDamageNumber;
    private float offsetY;
    Vector3 flyDirection;

    // Start is called before the first frame update
    public DamageNumber(int newDamage){
        elapsedTime = 0f;
        damage = newDamage;
        damageNumberDuration = 5f;
        currentDamageNumber = new List<GameObject>();
        offsetY = Random.Range(-1, 1);
        float randX = Random.Range(-2, 2);
        float randY = Random.Range(-2, 2);
        flyDirection = new Vector3(randX, randY, 0);

        char[] damageArray = damage.ToString().ToCharArray();
        for (int i = 0; i < damageArray.Length; i ++){
            GameObject imgObject = new GameObject();
            RectTransform trans = imgObject.AddComponent<RectTransform>();
            trans.transform.SetParent(transform); // setting parent
            trans.localScale = Vector3.one;
            // The anchoredPosition is set to be an invalid number to prevent it appear on the screen for a single frame.
            trans.anchoredPosition = new Vector2(-10000f, -10000f); // setting position, will be on center
            trans.sizeDelta = new Vector2(60, 80); // custom size
            Image image = imgObject.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>("Sprites/damage_numbers/" + damageArray[i]);
            imgObject.transform.SetParent(transform);
            currentDamageNumber.Add(imgObject);
        }
    }

    void Update(){
        if (currentDamageNumber == null){
            return;
        }
        
        // destroy numbers if time exceeds
        elapsedTime += Time.deltaTime;
        if (elapsedTime > damageNumberDuration){

            for(int i = 0; i < currentDamageNumber.Count; i ++){
                Destroy(currentDamageNumber[i]);
            }
            return;
        }

        // Note there every number(created in different setHealth function) are updated to the
        // same location, which is wrong.

        // update the position of each number
        // offsetX takes care of the position between each number in the current frame.
        float offsetX = 0f;
        
        for(int i = 0; i < currentDamageNumber.Count; i ++){
            Vector3 offset = new Vector3(offsetX, offsetY, 0f);
            Transform currentTrans = currentDamageNumber[i].GetComponent<RectTransform>().transform;
            currentTrans.position = Camera.main.WorldToScreenPoint(Vector3.Scale(transform.parent.position, new Vector3(0.8f, 0.3f, 0)) - offset);
            // probably should change to time to not be related to frame

            // animation effects
            currentTrans.position += Vector3.Scale(new Vector3(0.1f, 0.1f, 0f), flyDirection) * elapsedTime;
            currentTrans.localScale += new Vector3(0.005f, 0.005f, 0f);

            offsetX -= 1;
        }

    }
}
