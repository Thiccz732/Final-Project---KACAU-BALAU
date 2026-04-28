using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    // Kita buat referensi ke file Input Action kamu
    private InputSystem_Actions controls;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new InputSystem_Actions();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void FixedUpdate()
    {
        // Langsung baca nilai dari Input Action setiap frame fisika
        // "Move" adalah nama action di dalam file InputSystem_Actions kamu
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        // Gerakan mulus tanpa tersendat
        rb.linearVelocity = moveInput * speed;
    }
}