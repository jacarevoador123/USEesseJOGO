using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 20f; // Velocidade da bala
    public float lifetime = 2f; // Tempo de vida da bala antes de ser destruída

    private void Start()
    {
        // Destrói a bala após um certo tempo
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move a bala na direção em que ela está apontando
        transform.Translate(speed * Time.deltaTime * Vector2.right);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se a colisão foi com um inimigo
        if (other.CompareTag("Enemy"))
        {
            // Destrói o inimigo
            Destroy(other.gameObject);
        }

        // Destrói a bala
        Destroy(gameObject);
    }
}
