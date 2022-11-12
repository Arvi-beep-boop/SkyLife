using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PressSpace : MonoBehaviour
{
    float timer;

    void Start()
    {
        timer = 0.0f;
    }

    void Update()
    {
        // Change text visibility

        Color color = GetComponent<Text>().color;
        color.a = Mathf.Abs(Mathf.Sin(timer));
        timer += Time.deltaTime;
        GetComponent<Text>().color = color;

        // Load scene

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("SampleScene");
        }

    }
}
