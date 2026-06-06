using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool ativado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ativado) return;

        if (other.CompareTag("Player"))
        {
            ativado = true;

            CheckpointManager.Instance.SetCheckpoint(transform.position);

            Debug.Log("Checkpoint ativado!");

            // opcional: muda sprite/animação
            // GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}