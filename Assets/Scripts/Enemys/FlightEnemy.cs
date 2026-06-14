using UnityEngine;

public class FlightEnemy : BaseEnemy
{
    [SerializeField]
    private float hoverHeight = 2f;

    [SerializeField]
    private float hoverSpeed = 1f;

    private float hoverOffset;

    protected override void Awake()
    {
        base.Awake();
        hoverOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    protected override void Move()
    {
        // Move horizontally like ground enemy but with vertical sinusoidal hover
        var baseMovement = Vector2.right * moveSpeed;
        var hover = Mathf.Sin((Time.time + hoverOffset) * hoverSpeed) * hoverHeight;
        var movement = new Vector2(baseMovement.x, hover);

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
