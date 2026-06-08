using UnityEngine;

public class Player : MonoBehaviour
{
    float xInput;
    float yInput;

    [SerializeField]
    float speed = 10f;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        Vector2 movement = new Vector2(xInput, yInput).normalized;

        rb.linearVelocity = movement * speed;
    }
}