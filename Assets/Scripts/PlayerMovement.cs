using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the player character, including movement, shooting, collisions, UI updates, and level progression.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Animator used for player animations (e.g., shooting).
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Player movement speed.
    /// </summary>
    [SerializeField] float moveSpeed;

    /// <summary>
    /// Gun barrel positions for shooting.
    /// </summary>
    [SerializeField] Transform gun1;
    [SerializeField] Transform gun2;

    /// <summary>
    /// Ammo prefab used for shooting.
    /// </summary>
    [SerializeField] private GameObject ammo;

    /// <summary>
    /// UI text elements for displaying score and lives.
    /// </summary>
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI livesText;

    /// <summary>
    /// Player lives.
    /// </summary>
    public int lives;

    /// <summary>
    /// Time between consecutive shots.
    /// </summary>
    public float fireRate = 0.5f;

    /// <summary>
    /// Next allowed time to fire a shot.
    /// </summary>
    private float nextFireTime = 0f;

    /// <summary>
    /// List of enemies currently in the scene.
    /// </summary>
    private List<GameObject> enemies = new List<GameObject>();

    /// <summary>
    /// Reference to the GameManager to track score and level status.
    /// </summary>
    private GameManager gm;
    
    /// <summary>
    /// Called before the first frame update.
    /// Initializes references to GameManager and populates the enemy list.
    /// </summary>
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>(); ///< Get reference to GameManager
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));     ///< Store all enemies in the scene
    }
    
    /// <summary>
    /// Called once per frame.
    /// Handles player movement, shooting, UI updates, death, and level progression.
    /// </summary>
    void Update()
    {
        // Move player horizontally
        transform.Translate(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0, 0);

        // Flip player sprite based on movement direction
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.localScale = new Vector3(Input.GetAxisRaw("Horizontal"), 1, 1);
        }

        // Shooting logic with fire rate control
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            animator.SetTrigger("Shoot");          ///< Trigger shooting animation
            nextFireTime = Time.time + fireRate;   ///< Set next allowed fire time
            Instantiate(ammo, gun1.position, Quaternion.identity); ///< Fire ammo from gun1
            Instantiate(ammo, gun2.position, Quaternion.identity); ///< Fire ammo from gun2
            AudioManager.instance.LaserSound();   ///< Play laser sound
        }

        // Check for player death
        if (lives <= 0)
        {
            SceneManager.LoadScene("LoseScreen"); ///< Load lose screen
            Destroy(gameObject);                  ///< Destroy player
        }

        // Update UI
        scoreText.text = gm.score.ToString();
        livesText.text = lives.ToString();

        // Remove destroyed enemies from the list
        enemies.RemoveAll(item => item == null);

        // Progress to next level if all enemies are defeated
        if (enemies.Count == 0)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            string sceneName = "level" + currentSceneIndex;

            if (gm.status.ContainsKey(sceneName)) gm.status[sceneName] = true; ///< Mark level as completed

            SceneManager.LoadScene(currentSceneIndex + 1); ///< Load next level
        }
    }

    /// <summary>
    /// Called when the player collides with another 2D collider.
    /// Reduces player lives when hit by hostile ammo.
    /// </summary>
    /// <param name="other">Collision data for the impacting object.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ammo_Hostile"))
        {
            lives--; ///< Reduce player lives
        }
    }
}
