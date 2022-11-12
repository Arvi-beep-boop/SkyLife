using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fadeout : MonoBehaviour
{
    float timer;
    Image black;
    public Text text;

    float surprise = 300.0f;

    void Start()
    {
        
        black = GetComponent<Image>();
        timer = 10.0f;
    }

    void Update()
    {
        Color color = black.color;

        if (timer <= 5.0f && surprise > 0.0f)
        { 
        color.a = Mathf.Clamp(timer / 5.0f, 0.0f, 1.0f);
        black.color = color;
        }
        timer -= Time.deltaTime;
        surprise -= Time.deltaTime;


        if(surprise < 0.0f)
        {
            color.a = 1.0f;
            black.color = color;
            text.gameObject.SetActive(true);
        }

    }
}
