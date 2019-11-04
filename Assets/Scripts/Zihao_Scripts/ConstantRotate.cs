using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(rotate());
    }

    IEnumerator rotate()
    {

        while (true)
        {
            transform.Rotate(new Vector3(0, 0, 0.1f));
            yield return null;
        }
    }

}
