using System;
using UnityEngine;

/// <summary>
/// Controls the behavior of an enemy, including movement, shooting, collision handling,
/// scoring, and explosion effects.
/// </summary>
public class Enemy1Controller : MonoBehaviour
{
    /// <summary>
    /// Animator used to trigger shooting animations.
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Gun barrel transform positions.
    /// </summary>
    [SerializeField] Transform gun1;
    [SerializeField] Transform gun2;

    /// <summary>
    /// Ammo prefab to be fired.
    /// </summary>
    [SerializeField] private GameObject ammo;

    /// <summary>
    /// Horizontal and vertical movement speed, and movement distance limits.
    /// </summary>
    [SerializeField] float moveSpeed = 2f; 
    [SerializeField] float downSpeed = 1f;
    [SerializeField] float moveDistance = 3f;

    /// <summary>
    /// Shooting timer and interval between shots.
    /// </summary>
    [SerializeField] float timer = 0f;
    [SerializeField] float interval = 5f;

    /// <summary>
    /// Score value awarded when this enemy is destroyed.
    /// </summary>
    [SerializeField] int score;

    /// <summary>
    /// Explosion particle effect prefab.
    /// </summary>
    public ParticleSystem explosionPrefab;
    
    /// <summary>
    /// Current horizontal movement direction (1 = right, -1 = left).
    /// </summary>
    private int direction = 1;

    /// <summary>
    /// Starting X position for horizontal movement limits.
    /// </summary>
    private float startX;
    
    /// <summary>
    /// Called before the first frame update. Initializes starting position and animator.
    /// </summary>
    void Start()
    {
        startX = transform.position.x;        ///< Store starting X position
        animator = GetComponent<Animator>();  ///< Get Animator component
    }

    /// <summary>
    /// Called once per frame. Handles movement, shooting logic, and raycast detection
    /// to ensure proper shooting behavior.
    /// </summary>
    void Update()
    {
        transform.Translate(Vector2.down * downSpeed * Time.deltaTime, Space.World);  ///< Move downwards
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime, Space.World); ///< Move horizontally

        // Reverse horizontal direction when reaching movement limits
        if (transform.position.x >= startX + moveDistance) direction = -1;
        else if (transform.position.x <= startX - moveDistance) direction = 1;

        timer += Time.deltaTime; ///< Update shooting timer

        // Check if it's time to shoot
        if (timer >= interval)
        {
            Vector2 rayStart = (Vector2)transform.position + Vector2.down * 0.5f; ///< Raycast start position
            int enemyLayerMask = LayerMask.GetMask("Enemy");                       ///< Only check "Enemy" layer
            RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 100f, enemyLayerMask);

            if (hit.collider == null)
            {
                animator.SetTrigger("Shoot"); ///< Play shooting animation
                Instantiate(ammo, gun1.position, Quaternion.identity); ///< Fire from gun1
                Instantiate(ammo, gun2.position, Quaternion.identity); ///< Fire from gun2
                AudioManager.instance.LaserSound();                    ///< Play laser sound
                timer = 0;                                             ///< Reset timer
            }
        }
    }

    /// <summary>
    /// Called when this enemy collides with another 2D collider.
    /// Handles taking damage, updating score, reducing player lives, and destruction.
    /// </summary>
    /// <param name="other">Collision data for the impacting object.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Hit by player ammo
        if (other.gameObject.CompareTag("Ammo"))
        {
            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm != null) gm.score += score; ///< Add score to GameManager

            Destroy(gameObject); ///< Destroy enemy
            Explode();           ///< Trigger explosion
        }

        // Enemy reaches end boundary
        if (other.gameObject.CompareTag("End"))
        {
            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player != null) player.lives--; ///< Reduce player lives

            Destroy(gameObject); ///< Destroy enemy
        }
    }

    /// <summary>
    /// Spawns and plays the explosion particle effect and plays explosion sound.
    /// Automatically destroys the particle system after it finishes.
    /// </summary>
    void Explode()
    {
        ParticleSystem ps = Instantiate(explosionPrefab, transform.position, Quaternion.identity); ///< Create explosion
        ps.Play();                                                                                   ///< Play particle effect
        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);               ///< Destroy after effect
        AudioManager.instance.ExplodeSound();                                                        ///< Play explosion sound
    }
}
