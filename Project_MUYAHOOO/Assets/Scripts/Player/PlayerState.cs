using UnityEngine;

public class PlayerState : EntityState
{
    void Start()
    {
        Health = 5;
        MaxHealth = 5;
    }

    void Update()
    {
        
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    public override void OnDead()
    {
        base.OnDead();
    }
}
