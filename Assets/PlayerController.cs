using UnityEngine;               
using UnityEngine.InputSystem;    

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour   
{

    public float moveSpeed = 5f;     
    public float jumpForce = 7f;     

    bool isGameOver = false;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Plane")
        {
            isGameOver = true;

            Debug.Log("Game Over: Player touched the Plane");
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;

        }

    }

    public LayerMask groundMask;      

    

    Rigidbody rb;                   
    bool isGrounded;                
    Vector3 moveInput;              
    // A 3-component vector (x, y, z).
    // Stores the direction the player wants to move (from keyboard).
    // But y is always 0 because we only move horizontally.


    // Called before Start() - Best place to initialize
    void Awake()
    {
        rb = GetComponent<Rigidbody>();          // Grab Rigidbody on this GameObject
        rb.freezeRotation = true;                // Prevent tipping over
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        // Makes physics more accurate when hitting edges
    }

    void Update()
    {
        if (isGameOver)
        {
            // Player.Destroy(this);
            Destroy(this.gameObject);
            return;
        }

        var k = Keyboard.current;    // Gives access to keyboard state

        // WASD movement:
        float h = (k.aKey.isPressed ? -1f : 0f) + (k.dKey.isPressed ? 1f : 0f);  // h = horizontal (left/right)
        float v = (k.sKey.isPressed ? 1f : 0f) + (k.wKey.isPressed ? -1f : 0f);  // v = vertical in 2D sense (forward/back)
        Vector3 raw = new Vector3(v, 0f, h);  // raw = unnormalized input vector
        // sqrMagnitude = squared length of vector (faster than magnitude, avoids sqrt)
        // normalized = vector scaled to length 1, keeping same direction
        moveInput = raw.sqrMagnitude > 1f ? raw.normalized : raw;  // Normalize diagonal movement to prevent faster speed


        // GROUND CHECK (raycast down)
        // If no groundMask assigned, hit everything (~0)
        // Note: ~0 is bitwise NOT operator - flips all bits to 1, meaning "all layers" (-1)
        int mask = groundMask.value == 0 ? ~0 : groundMask.value;

    // Simplified: our player uses a SphereCollider with radius 0.5, center (0,0,0), scale (1,1,1)
    // If these assumptions change, switch back to reading from the collider dynamically.
    const float radius = 0.5f;
    // Lift origin a tiny bit so the cast doesn't start overlapped
    Vector3 sphereCenter = transform.position;
        // Minimal overlap test for grounded state at the "foot" of the sphere
        Vector3 foot = sphereCenter - Vector3.up * (radius - 0.02f);
        isGrounded = Physics.CheckSphere(foot, radius * 0.98f, mask, QueryTriggerInteraction.Ignore);


        // ------------------------------------
        // JUMP
        // ------------------------------------

        if (k.spaceKey.wasPressedThisFrame && isGrounded)
        {
            // Use AddForce for a more physical jump impulse. Use VelocityChange so jump is
            // consistent regardless of mass (instant change in velocity).
            // Optionally zero the current vertical velocity for consistent jumps.
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }


    // Called on a physics step (fixed interval)
    // Good for touching Rigidbody properties
    void FixedUpdate()
    {
        // Keep the Y velocity from gravity or jumping and apply horizontal input
        rb.linearVelocity = new Vector3(
            moveInput.x * moveSpeed,
            rb.linearVelocity.y,
            moveInput.z * moveSpeed
        );
    }
}
