using UnityEngine;

/// <summary>
/// Controls asteroid behavior, including health, collision handling,
/// explosion effects, and destruction logic.
/// </summary>
public class Asteroid : MonoBehaviour
{
    /// <summary>
    /// Health value of the asteroid. When this reaches below zero, the asteroid is destroyed.
    /// </summary>
    [SerializeField] private int health;

    /// <summary>
    /// Reference to the explosion particle effect prefab.
    /// </summary>
    public ParticleSystem explosionPrefab;

    /// <summary>
    /// Called once per frame. Checks the asteroid's health and triggers destruction,
    /// explosion, and sound effects if health is below zero.
    /// </summary>
    void Update()
    {
        if (health < 0)
        {
            Destroy(gameObject);                     ///< Remove asteroid from the scene
            Explode();                               ///< Play explosion effect
            AudioManager.instance.ExplodeSound();    ///< Play explosion sound
        }
    }

    /// <summary>
    /// Called automatically by Unity when this object collides with another 2D collider.
    /// Reduces health when hit by hostile or friendly ammo.
    /// </summary>
    /// <param name="other">Collision data associated with the impacting object.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ammo_Hostile") || other.gameObject.CompareTag("Ammo"))
        {
            health--; ///< Reduce asteroid health when hit
        }
    }
    
    /// <summary>
    /// Spawns and plays the explosion particle effect at the asteroid's position.
    /// Automatically destroys the particle system after it finishes playing.
    /// </summary>
    void Explode()
    {
        ParticleSystem ps = Instantiate(explosionPrefab, transform.position, Quaternion.identity); ///< Instantiate the explosion

        ps.Play(); ///< Start the particle system

        // Destroy the particle system after it finishes
        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }
}
