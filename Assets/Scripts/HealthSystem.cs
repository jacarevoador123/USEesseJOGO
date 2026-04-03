using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 6; // Quantidade m�xima de sa�de (cada cora��o tem 2 unidades de vida)
    public int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth; // Come�a com vida cheia
    }

    // M�todo que ser� chamado ao receber dano
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

    }

    // M�todo para curar o jogador
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

    }

}
