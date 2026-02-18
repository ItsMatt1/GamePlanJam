using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private DualitySystem dualitySystem;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        dualitySystem = GetComponent<DualitySystem>();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        moveInput = Vector2.zero;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) moveInput.y += 1;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) moveInput.y -= 1;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) moveInput.x -= 1;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveInput.x += 1;
        moveInput = moveInput.normalized;

        // Flip sprite toward mouse
        var mouse = Mouse.current;
        if (mouse != null && spriteRenderer != null)
        {
            Vector2 mouseScreen = mouse.position.ReadValue();
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
                new Vector3(mouseScreen.x, mouseScreen.y, 0f));

            spriteRenderer.flipX = mouseWorld.x < transform.position.x;
        }
    }

    void FixedUpdate()
    {
        float speed = baseSpeed;

        // Devil form slight speed boost
        if (dualitySystem != null && dualitySystem.currentForm == PlayerForm.Devil)
            speed *= 1.15f;

        rb.linearVelocity = moveInput * speed;
    }
}
