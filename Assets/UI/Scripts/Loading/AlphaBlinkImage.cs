using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaBlinkImage : MonoBehaviour
{
    float AlphaSpeed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Blinking(this.GetComponent<Image>()));
    }

    // Update is called once per frame
    IEnumerator Blinking(Image image)
    {
        while (true)
        {
            while (image.color.a >= 0)
            {
                image.color = new Color (image.color.r, image.color.g, image.color.b, image.color.a - AlphaSpeed);
                yield return null;
            }
            while (image.color.a <1)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + AlphaSpeed);
                yield return null;
            }
            yield return null;
        }
        
    }
}
