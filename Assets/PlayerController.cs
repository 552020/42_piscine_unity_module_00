using UnityEngine;               
// Gives access to Unity engine types (MonoBehaviour, Rigidbody, Vector3...)

using UnityEngine.InputSystem;    
// Gives access to the NEW input system (Keyboard.current, etc.)


// This attribute forces the GameObject to have a Rigidbody.
// If it doesn't have one, Unity adds it automatically.
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour   // A Unity script must extend MonoBehaviour
{
    // ------------------------
    // INSPECTOR VARIABLES
    // ------------------------

    public float moveSpeed = 5f;     
    // A plain float variable YOU define.
    // Visible in the Inspector.
    // Controls how fast the player moves left/right/forward/back.
    // Used later when calculating velocity.

    public float jumpForce = 7f;     
    // Another float YOU defined.
    // Controls how strong the jump velocity is.

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
    // A Unity type that encodes which layers count as "ground".
    // Example: platforms have "Ground" layer. This mask tells the raycast what to hit.
    // If set to Nothing (0), we default to "hit everything".

    public float groundCheckDistance = 0.6f;
    // How far below the player we cast a ray to detect ground.
    // Adjust according to the player's collider size.

    // ------------------------
    // PRIVATE VARIABLES
    // ------------------------

    Rigidbody rb;                   
    // A reference to the Rigidbody component.
    // Set in Awake().

    bool isGrounded;                
    // Tracks whether player is standing on ground.
    // Set each frame by a raycast.

    // Debug helpers
    public bool debugGroundLogs = true; // Toggle logs in the Inspector
    bool _groundStateInitialized;
    bool _lastGrounded;

    Vector3 moveInput;              
    // A 3-component vector (x, y, z).
    // Stores the direction the player wants to move (from keyboard).
    // But y is always 0 because we only move horizontally.


    // Called before Start()
    // Best place to set up component references.
    void Awake()
    {
        rb = GetComponent<Rigidbody>();          // Grab Rigidbody on this GameObject
        rb.freezeRotation = true;                // Prevent tipping over
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        // Makes physics more accurate when hitting edges
    }


    // Called once per frame
    // Good for reading player input
    void Update()
    {
        if (isGameOver) return;

        var k = Keyboard.current;    // Gives access to keyboard state

        // WASD movement:
        float h = (k.aKey.isPressed ? -1f : 0f) + (k.dKey.isPressed ? 1f : 0f);
        float v = (k.sKey.isPressed ? 1f : 0f) + (k.wKey.isPressed ? -1f : 0f);
        Vector3 raw = new Vector3(v, 0f, h);
        moveInput = raw.sqrMagnitude > 1f ? raw.normalized : raw;


        // ------------------------------------
        // GROUND CHECK (raycast down)
        // ------------------------------------

        // If no groundMask assigned, hit everything (~0)
        int mask = groundMask.value == 0 ? ~0 : groundMask.value;

        // More robust ground detection using SphereCollider (if present)
        SphereCollider sc = GetComponent<SphereCollider>();
        float radius = sc ? sc.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z) : 0.5f;
        // Lift origin a tiny bit so the cast doesn't start overlapped
        Vector3 sphereCenter = transform.position + (sc ? transform.rotation * sc.center : Vector3.zero);
        Vector3 castOrigin = sphereCenter + Vector3.up * 0.05f;
        float castDistance = groundCheckDistance + 0.05f; // small fudge

        RaycastHit hitInfo;
        bool hitSomething = Physics.SphereCast(
            castOrigin,
            radius * 0.95f,             // shrink slightly to avoid initial overlap false negatives
            Vector3.down,
            out hitInfo,
            castDistance,
            mask,
            QueryTriggerInteraction.Ignore
        );
        // Additional overlap check at the foot to be resilient when already in contact
        Vector3 foot = sphereCenter - Vector3.up * (radius - 0.02f);
        bool touching = Physics.CheckSphere(foot, radius * 0.98f, mask, QueryTriggerInteraction.Ignore);
        isGrounded = (hitSomething && hitInfo.distance <= castDistance) || touching;
        
        // Every-frame debug: see what spherecast detects
        if (debugGroundLogs && k.spaceKey.wasPressedThisFrame)
        {
            Debug.Log($"[GroundProbe] hit={hitSomething}, touch={touching}, dist={(hitSomething?hitInfo.distance:0f):F3}, hitName={(hitInfo.collider?hitInfo.collider.name:"none")}, mask={mask}, originY={castOrigin.y:F2}, radius={radius:F2}, gDist={groundCheckDistance:F2}");
        }

        // Optional: log only when state changes (no spam)
        if (debugGroundLogs)
        {
            if (!_groundStateInitialized)
            {
                _lastGrounded = isGrounded;
                _groundStateInitialized = true;
                Debug.Log($"[Ground] initial={(isGrounded ? "grounded" : "airborne")} posY={transform.position.y:F2}");
            }
            else if (_lastGrounded != isGrounded)
            {
                if (isGrounded)
                {
                    string hitName = hitInfo.collider ? hitInfo.collider.name : "<none>";
                    Debug.Log($"[Ground] Landed on '{hitName}' at distance {hitInfo.distance:F3}");
                }
                else
                {
                    Debug.Log("[Ground] Left ground");
                }
                _lastGrounded = isGrounded;
            }
        }


        // ------------------------------------
        // JUMP
        // ------------------------------------

        // If jump pressed while NOT grounded, log why we skipped
        if (k.spaceKey.wasPressedThisFrame && !isGrounded && debugGroundLogs)
        {
            Debug.Log("[Jump] Pressed but not grounded — jump ignored");
        }

        if (k.spaceKey.wasPressedThisFrame && isGrounded)
        {
            // Use AddForce for a more physical jump impulse. Use VelocityChange so jump is
            // consistent regardless of mass (instant change in velocity).
            // Optionally zero the current vertical velocity for consistent jumps.
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            if (debugGroundLogs)
            {
                Debug.Log($"[Jump] Jumping with force={jumpForce:F2}, new vy={rb.linearVelocity.y:F2}");
            }
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


#if UNITY_EDITOR
    // Draw a debug line in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.down * groundCheckDistance
        );
    }
#endif
}
