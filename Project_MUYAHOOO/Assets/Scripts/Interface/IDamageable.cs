using UnityEngine;

public interface IDamageable
{
    float Health { get; set; }
    float MaxHealth { get; set; }

    bool IsAlive { get;}
    bool Breakable { get; set; }
    void TakeDamage(float damage);
}
