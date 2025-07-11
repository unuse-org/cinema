using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTransform : MonoBehaviour
{

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<RectTransform>().position.y < 30f)
        {
            GetComponent<RectTransform>().position += Vector3.up * Time.deltaTime * 2f;
        }
    }
}
