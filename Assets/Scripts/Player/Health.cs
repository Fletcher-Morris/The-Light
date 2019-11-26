using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float m_health = 10.0f;
    private float m_prevHealth;
    public float HealthFloat { get => m_health;}
    public float HealthInt { get => Mathf.CeilToInt(m_health); }

    [SerializeField] private float m_maxHealth = 10.0f;
    public float MaxHealth { get => m_maxHealth; }

    [SerializeField] private float m_regen = 0.0f;
    public float Regen { get => m_regen; set => m_regen = value; }

    public void DoDamage(int _damage) { DoDamage((float)_damage); }
    public void DoDamage(float _damage)
    {
        m_health -= _damage;
        m_health = Mathf.Clamp(m_health, 0.0f, MaxHealth);
    }

    public void HealthUpdate(float _delta)
    {
        if(m_health != m_prevHealth)
        {
            if (m_health <= 0.0f)
            {
                OnHealthZero.Invoke();
            }
        }

        if (m_health > 0.0f) m_health += m_regen * _delta;

        m_health.Clamp(0.0f, MaxHealth);

        m_prevHealth = m_health;
    }

    public bool IsDead() { return m_health <= 0.0f; }


    public UnityEvent OnHealthZero;

}
