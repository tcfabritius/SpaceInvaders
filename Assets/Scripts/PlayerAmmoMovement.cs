using System;
using UnityEngine;

/// <summary>
/// Controls the movement and collision behavior of the player's ammo.
/// </summary>
public class PlayerAmmoMovement : MonoBehaviour
{
    /// <summary>
    /// Called when the object is first created.
    /// Automatically destroys the projectile after a set time.
    /// </summary>
    void Start()
    {
        Destroy(gameObject, 5f); ///< Destroy ammo after 5 seconds
    }
    
    /// <summary>
    /// Called once per frame.
    /// Moves the player's ammo upward over time.
    /// </summary>
    void Update()
    {
        transform.Translate(0, 2 * Time.deltaTime, 0); ///< Move ammo upward
    }

    /// <summary>
    /// Called when this object collides with another 2D collider.
    /// Destroys the ammo when it hits rocks, enemies, or other projectiles.
    /// </summary>
    /// <param name="other">Collision data for the impacting object.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Rock") ||
            other.gameObject.CompareTag("Enemy") ||
            other.gameObject.CompareTag("Ammo") ||
            other.gameObject.CompareTag("Ammo_Hostile"))
        {
            Destroy(gameObject); ///< Destroy ammo on collision
        }
    }
}