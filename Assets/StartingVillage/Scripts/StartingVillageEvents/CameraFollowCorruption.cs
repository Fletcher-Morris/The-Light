using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCorruption : MonoBehaviour
{
    [SerializeField] Transform Corruption, Camera;
    Quaternion OriginRot;
    Vector3 OriginPos;
    IEnumerator Coroutine;
    void Awake()
    {
        this.enabled = false;
        OriginRot = Camera.transform.localRotation;
        OriginPos = Camera.transform.localPosition;
    }

    void OnEnable()
    {
        Coroutine = DoFollow();
        StartCoroutine(Coroutine);
        StartCoroutine(DoZoom());
    }

    void OnDisable()
    {
        StopCoroutine(Coroutine);
        Camera.transform.localRotation = OriginRot;
        Camera.transform.localPosition = OriginPos;
    }

    // Update is called once per frame
    IEnumerator DoFollow()
    {
        while (true)
        {
            Camera.LookAt(Corruption);
            yield return true;
        }
    }
    public bool KeepZoom = true;
    IEnumerator DoZoom()
    {
        while (KeepZoom)
        {
            Camera.transform.position = Vector3.MoveTowards(Camera.transform.position, Corruption.position, 0.05f);
            yield return true;
        }
        while (true)
        {
            Camera.transform.localPosition = Vector3.MoveTowards(Camera.transform.localPosition, OriginPos, 0.1f);
            yield return true;
        }
    }
}
