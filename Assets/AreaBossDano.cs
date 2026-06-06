using UnityEngine;

public class AreaBossDano : MonoBehaviour
{
    public int dano = 25;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.SendMessage(
                "AplicarDano",
                dano,
                SendMessageOptions.DontRequireReceiver
            );
        }
    }
}