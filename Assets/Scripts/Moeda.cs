using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]//Obriga a moeda a ter um collider
public class Moeda : MonoBehaviour
{
    [SerializeField] int valor = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HUDController.Instance.ColetarMoeda(valor);
            Destroy(gameObject);
        }
    }
}
