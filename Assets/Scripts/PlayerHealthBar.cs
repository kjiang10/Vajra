using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private Slider slider;
    public Color low;
    public Color high;

    public void SetHealthBar(int health, int maxHealth, int damage){
        float randX = Random.Range(-2f, 2f);
        float randY = Random.Range(-2f, 2f);
        Vector3 flyDirection = new Vector3(randX, randY, 0f);
        slider.maxValue = maxHealth;
        slider.value = health;
        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(low, high, slider.normalizedValue);
        
        GameObject textObject = new GameObject();
        textObject.transform.SetParent(transform);
        RectTransform trans = textObject.AddComponent<RectTransform>();
        trans.transform.SetParent(transform); // setting parent
        trans.localScale = Vector3.one;
        // The anchoredPosition is set to be an invalid number to prevent it appear on the screen for a single frame.
        trans.anchoredPosition = new Vector2(0f, 0f); // setting position, will be on center
        // trans.sizeDelta = new Vector2(60, 80); // custom size
        
        Text text = textObject.AddComponent<Text>();
        text.text = damage.ToString();
        text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.fontSize = 50;
        text.color = Color.red;
        
        StartCoroutine(DisplayDamageNumber(textObject, Random.Range(-1f, 1f), Random.Range(-1f, 1f), flyDirection, 0f));
    }

    // Start is called before the first frame update
    void Start()
    {
        // get the slider, slider should be the first child of Canvas
        foreach(Transform child in transform){
            if (child.name == "Slider"){
                slider = child.gameObject.GetComponent<Slider>();
                break;
            }
        }
        PlayerStatus playerStatus = GameObject.Find("Vajra").GetComponent<PlayerStatus>();
        slider.maxValue = playerStatus.hpCap;
        slider.value = playerStatus.getHP();
        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(low, high, slider.normalizedValue);
    }


    IEnumerator DisplayDamageNumber(GameObject textObject, float randOffsetX, float randOffsetY, Vector3 randFlyDirection, float delay){
        Transform currentTrans = textObject.GetComponent<RectTransform>().transform;
        Text text = currentTrans.GetComponent<Text>();
        Vector3 offset = new Vector3(randOffsetX, randOffsetY, 0f);
        for (int i = 0; i < 200; i ++){
            if (i <= 100){
                Color newTextColor = text.color;
                newTextColor.a = 0.01f * i;
                text.color = newTextColor;
            }
            else{
                // add fading out
                Color newTextColor = text.color;
                newTextColor.a = 1f - 0.01f * (i - 100);
                text.color = newTextColor;
            }
            currentTrans.position = Camera.main.WorldToScreenPoint(transform.parent.position - offset);
            // probably should change to time to not be related to frame
            // animation effects
            currentTrans.position += Vector3.Scale(new Vector3(0.1f, 0.1f, 0f), randFlyDirection) * i;
            currentTrans.localScale += new Vector3(0.005f, 0.005f, 0f);
            yield return new WaitForSeconds(0.0035f);
        }

        Destroy(textObject);
    }
}
