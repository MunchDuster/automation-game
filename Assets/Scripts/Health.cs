using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    private float _health;

    /// <summary>
    /// Reduces health
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>Whether killed</returns>
    public bool TakeDamage(float damage)
    {
        _health -= damage;

        bool killed = _health <= 0;
        if (killed)
        {
            Destroy(gameObject);
        }

        return killed;
    }
}
