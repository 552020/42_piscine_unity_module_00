using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerComplex : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    // Game Over on Plane
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

    public float groundCheckDistance = 0.6f;
    // Hardcoded for our sphere with radius 0.5.
    // Better practice: retrieve this dynamically from the collider at runtime or use bounds.

    Rigidbody rb;
    bool isGrounded;
    Vector3 moveInput; // horizontal-only movement vector

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void Update()
    {
        if (isGameOver)
        {
            Destroy(this.gameObject);
            return;
        }

        var k = Keyboard.current;

        // WASD movement:
        float h = (k.aKey.isPressed ? -1f : 0f) + (k.dKey.isPressed ? 1f : 0f);  // h = horizontal (left/right)
        float v = (k.sKey.isPressed ? 1f : 0f) + (k.wKey.isPressed ? -1f : 0f);  // v = vertical in 2D sense (forward/back)
        Vector3 raw = new Vector3(v, 0f, h);  // raw = unnormalized input vector
        moveInput = raw.sqrMagnitude > 1f ? raw.normalized : raw;  // Normalize diagonal movement

        // ------------------------------------
        // GROUND CHECK (raycast down + overlap)
        // ------------------------------------

        // If no groundMask assigned, hit everything (~0)
        // Note: ~0 is bitwise NOT operator - flips all bits to 1, meaning "all layers" (-1)
        int mask = groundMask.value == 0 ? ~0 : groundMask.value;

        // Our player uses a SphereCollider with radius 0.5, center (0,0,0), scale (1,1,1)
        // If these assumptions change, switch back to reading from the collider dynamically.
        const float radius = 0.5f;
        Vector3 sphereCenter = transform.position;

        // Sphere cast from a tiny offset above center to avoid starting overlapped
        Vector3 castOrigin = sphereCenter + Vector3.up * 0.05f;
        float castDistance = groundCheckDistance + 0.05f; // small epsilon

        RaycastHit hitInfo;
        bool hitSomething = Physics.SphereCast(
            castOrigin,
            radius * 0.95f,             // slightly smaller to avoid initial overlap
            Vector3.down,
            out hitInfo,
            castDistance,
            mask,
            QueryTriggerInteraction.Ignore
        );

        // Overlap check at foot for robust contact when already touching
        Vector3 foot = sphereCenter - Vector3.up * (radius - 0.02f);
        bool touching = Physics.CheckSphere(foot, radius * 0.98f, mask, QueryTriggerInteraction.Ignore);
        isGrounded = (hitSomething && hitInfo.distance <= castDistance) || touching;

        // ------------------------------------
        // JUMP
        // ------------------------------------
        if (k.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(
            moveInput.x * moveSpeed,
            rb.linearVelocity.y,
            moveInput.z * moveSpeed
        );
    }
}
