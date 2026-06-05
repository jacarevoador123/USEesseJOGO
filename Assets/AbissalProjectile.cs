using UnityEngine;

public class AbissalProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 1f;
    public int damage = 40;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        if (other.CompareTag("BOSS"))
        {
            BossVida boss = other.GetComponent<BossVida>();
            if (boss != null)
                boss.AplicarDano(damage);

            Destroy(gameObject);
            return;
        }

        SistemaDeVidaInimigo enemy = other.GetComponent<SistemaDeVidaInimigo>();
        if (enemy != null)
        {
            enemy.AplicarDano(damage);
            Destroy(gameObject);
        }
    }
}