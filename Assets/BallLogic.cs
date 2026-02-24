using UnityEngine;
using UnityEngine.SceneManagement; // Namespace required to manage scenes (restart, load levels)

public class BallLogic : MonoBehaviour
{
    // Public variables appear in the Unity Inspector
    // We increased force to counter the gravity setting in the Rigidbody
    public float launchForce = 12f;
    public float minSpeed = 5f;
    public float maxSpeed = 10f;

    // Private variables are internal to this script
    private Rigidbody2D rb;
    private Vector3 initialPosition;

    void Start()
    {
        // 'GetComponent<Type>()' looks for a specific component attached to THIS GameObject.
        // We cache it in 'rb' so we don't have to look for it every frame (performance optimization).
        rb = GetComponent<Rigidbody2D>();
        
        // Save the spawn point (x,y,z) so we can reset the ball later
        initialPosition = transform.position;

        LaunchBall();
    }

    void Update()
    {
        // Check if the ball falls below the screen limit (Y < -6)
        if (transform.position.y < -6f) 
        {
            // 'this.gameObject' refers to the object this script is attached to.
            // 'Destroy' removes it from memory and the hierarchy instantly.
            Destroy(this.gameObject);
        }
    }

    // FixedUpdate is called at a fixed time interval (0.02s by default).
    // ALWAYS use this for Physics calculations to ensure smooth, consistent movement.
    void FixedUpdate()
    {
        // ---------------------------------------------------------
        // VELOCITY CLAMPING (Speed Limiter)
        // ---------------------------------------------------------

        // 1. Get the current velocity vector (Direction + Speed)
        Vector2 currentVelocity = rb.linearVelocity;
        
        // 2. Calculate 'magnitude'. This is the length of the vector, representing pure Speed.
        // (e.g., if velocity is (3, 4), magnitude is 5).
        float speed = currentVelocity.magnitude;

        // 3. CASE A: TOO FAST (Clamp max speed)
        if (speed > maxSpeed)
        {
            // 'normalized' returns the vector with a length of 1 (keeps direction, removes speed).
            // We multiply by 'maxSpeed' to set the exact speed we want.
            rb.linearVelocity = currentVelocity.normalized * maxSpeed;
        }
        
        // 4. CASE B: TOO SLOW (Boost min speed)
        // We check 'speed > 0.1f' to ensure we don't boost a ball that is supposed to be stopped
        // or sitting at the very top of a gravity arc (where speed naturally hits 0 momentarily).
        else if (speed < minSpeed && speed > 0.1f) 
        {
             rb.linearVelocity = currentVelocity.normalized * minSpeed;
        }
    }

    // This event is triggered automatically by Unity Physics when a collision happens
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check the name of the object we hit.
        // IMPORTANT: Ensure your Paddle object in the Hierarchy is named exactly "Paddle".
        if (collision.gameObject.name == "Paddle")
        {
            // ---------------------------------------------------------
            // 1. SCORE LOGIC
            // ---------------------------------------------------------
            // 'FindObjectOfType' searches the entire scene for the GameManager script.
            // Then we call its public method 'AddScore'.
            FindObjectOfType<GameManager>().AddScore();

            // ---------------------------------------------------------
            // 2. ARKANOID BOUNCE PHYSICS
            // ---------------------------------------------------------
            // This calculates a custom angle based on where the ball hit the paddle.

            // Get the X position of the Ball
            float myPosX = transform.position.x;
            
            // Get the X position of the Paddle (the object we hit)
            float paddleCenterX = collision.transform.position.x;
            
            // Calculate offset:
            // Positive value = Hit right side. Negative value = Hit left side.
            float offset = myPosX - paddleCenterX;
            
            // Add a tiny bit of random noise (-1.0 to 1.0) so the ball never gets stuck in a perfect vertical loop.
            float noise = Random.Range(-1f, 1f);
            
            // Apply an instant force (Impulse).
            // We modify X based on the offset * 10 (multiplier for strength).
            // We leave Y as 0 and let the physics engine handle the vertical bounce.
            rb.AddForce(new Vector2((offset * 10f) + noise, 0), ForceMode2D.Impulse);
        }
    }

    void LaunchBall()
    {
        // Reset velocity to (0,0) to remove any previous momentum
        rb.linearVelocity = Vector2.zero; 
        
        // Move ball back to start
        transform.position = initialPosition;
        
        // Create a direction vector.
        // .normalized ensures the total length of the vector is 1, so direction doesn't affect power.
        Vector2 launchDirection = new Vector2(0, 0).normalized;

        // Apply force instantly.
        // ForceMode2D.Impulse is used for explosions or instant hits (like a serve).
        // ForceMode2D.Force is used for continuous pushing (like a rocket engine).
        rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);
    }

    // Public method called by GameManager to increase difficulty over time
    public void UpdateDifficulty(float newMax)
    {
        maxSpeed = newMax;

        // Immediate check: If the ball is currently too slow for the NEW difficulty, speed it up now.
        if (rb.linearVelocity.magnitude < minSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * minSpeed;
        }
    }
}