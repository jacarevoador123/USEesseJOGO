using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armadilha : MonoBehaviour
{
    [SerializeField] float damage = 2;
    [SerializeField] Transform checkPointPosition = null;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            player.AplicarDano(damage);

            if (checkPointPosition != null)
            {
                player.MoveToCheckPoint(checkPointPosition);
            }
        }
    }

}