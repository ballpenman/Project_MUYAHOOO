using UnityEngine;

public class EntityState : MonoBehaviour,IDamageable
{
    public float Health { get; set; }
    public float MaxHealth { get; set; }

    public bool IsAlive => Health > 0;

    public bool Breakable { get; set; }
    
    public virtual void TakeDamage(float damage)
    {
        if (!Breakable) return;
        Health -= damage;
        if (!IsAlive)
            OnDead();
    }

    public virtual void OnDead()
    {

    }
}
