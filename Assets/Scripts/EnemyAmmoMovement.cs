using UnityEngine;

/// <summary>
/// Controls the movement of enemy ammo and handles collisions
/// with other objects in the scene.
/// </summary>
public class EnemyAmmoMovement : MonoBehaviour
{
    /// <summary>
    /// Called once per frame. Moves the enemy ammo downward.
    /// </summary>
    void Update()
    {
        transform.Translate(0, -2 * Time.deltaTime, 0); ///< Move the ammo downward over time
    }

    /// <summary>
    /// Called automatically by Unity when this object collides with another 2D collider.
    /// Destroys the ammo if it hits rocks, player, other ammo, enemies, or the end boundary.
    /// </summary>
    /// <param name="other">Collision data associated with the impacting object.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Destroy ammo if it hits rocks, player, other ammo, or end boundary
        if (other.gameObject.CompareTag("Rock") ||
            other.gameObject.CompareTag("Player") ||
            other.gameObject.CompareTag("Ammo") ||
            other.gameObject.CompareTag("End"))
        {
            Destroy(gameObject);
        }
        // Destroy ammo if it hits another enemy
        else if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}