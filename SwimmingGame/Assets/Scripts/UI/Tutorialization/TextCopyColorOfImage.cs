using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextCopyColorOfImage : MonoBehaviour
{
    public Image image;
    private TMP_Text text;
    void Start()
    {
        text=GetComponent<TMP_Text>();   
    }

    // Update is called once per frame
    void Update()
    {
        text.color=image.color;
    }
}
