using UnityEngine;

public class BasicEnemy : BaseEnemy
{
    protected override void Move()
    {
        var movement = Vector2.right * moveSpeed;
        if (rigidbody2d != null)
        {
            rigidbody2d.linearVelocity = movement;
        }
        else
        {
            transform.Translate(movement * Time.fixedDeltaTime);
        }
    }
}
