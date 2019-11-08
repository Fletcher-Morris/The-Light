using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaBlinkText : MonoBehaviour
{
    float AlphaSpeed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Blinking(this.GetComponent<Text>()));
    }

    // Update is called once per frame
    IEnumerator Blinking(Text text)
    {
        while (true)
        {
            while (text.color.a >= 0)
            {
                text.color = new Color (text.color.r, text.color.g, text.color.b, text.color.a - AlphaSpeed);
                yield return null;
            }
            while (text.color.a <1)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + AlphaSpeed);
                yield return null;
            }
            yield return null;
        }
        
    }
}
