using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float followSpeed = 0.1f;
    public Vector3 offset;

    [Header("Treme camera")]
    public float shakeDuration = 1.0f;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    // Vector3 originalPos;
    float currentShakeDuration = 0f;


    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position + offset, followSpeed);

        if (currentShakeDuration > 0)
        {
            transform.position = transform.position + Random.insideUnitSphere * shakeAmount;
            currentShakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            currentShakeDuration = 0;
        }
    }

    public void Tremer(float duration)
    {
        if (duration < 0)
        {
            currentShakeDuration = shakeDuration;
        }
        else
        {
            currentShakeDuration = duration;
        }
    }

}
