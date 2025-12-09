using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Manages the pause menu functionality, including pausing/resuming the game,
/// saving/loading game state, quitting, and retrying levels.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// Reference to the pause menu UI panel.
    /// </summary>
    public GameObject pauseMenuUI;

    /// <summary>
    /// Tracks whether the game is currently paused.
    /// </summary>
    private bool isPaused = false;

    /// <summary>
    /// Reference to the GameManager to access score and level data.
    /// </summary>
    GameManager gm;

    /// <summary>
    /// Called before the first frame update.
    /// Caches the GameManager reference.
    /// </summary>
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>(); ///< Cache GameManager
    }
    
    /// <summary>
    /// Called once per frame.
    /// Handles toggling pause/resume when the Escape key is pressed,
    /// except on Win/Lose screens.
    /// </summary>
    void Update()
    {
        string SceneName = SceneManager.GetActiveScene().name;

        if (SceneName != "LoseScreen" && SceneName != "WinScreen")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused) Resume();
                else Pause();
            }
        }
    }

    /// <summary>
    /// Resumes the game from a paused state.
    /// </summary>
    public void Resume()
    {
        pauseMenuUI.SetActive(false); ///< Hide pause menu
        Time.timeScale = 1f;          ///< Resume game time
        isPaused = false;             ///< Update pause state
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void Pause()
    {
        pauseMenuUI.SetActive(true); ///< Show pause menu
        Time.timeScale = 0f;         ///< Stop game time
        isPaused = true;             ///< Update pause state
    }
    
    /// <summary>
    /// Saves the current game state, including score and level progress, to a file.
    /// </summary>
    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.score = gm.score;

        foreach (var kvp in gm.status)
        {
            data.levelNames.Add(kvp.Key);
            data.levelStatuses.Add(kvp.Value);
        }

        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// Loads saved game state from file, restoring score and level progress.
    /// Automatically loads the first uncompleted level or the WinScreen if all levels completed.
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            gm.score = data.score;
            gm.status.Clear();

            for (int i = 0; i < data.levelNames.Count; i++)
            {
                gm.status[data.levelNames[i]] = data.levelStatuses[i];
            }

            string firstUncompletedLevel = null;
            for (int i = 1; i <= 4; i++)
            {
                string levelName = "level" + i;
                if (gm.status.ContainsKey(levelName) && !gm.status[levelName])
                {
                    firstUncompletedLevel = levelName;
                    break;
                }
            }

            if (firstUncompletedLevel != null)
            {
                SceneManager.LoadScene(firstUncompletedLevel);
                Debug.Log("Loading first uncompleted level: " + firstUncompletedLevel);
            }
            else
            {
                SceneManager.LoadScene("WinScreen"); 
                Debug.Log("All levels completed!");
            }
        }
    }

    /// <summary>
    /// Loads the main menu scene and resumes normal time scale.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    /// <summary>
    /// Reloads the first gameplay scene (retry).
    /// </summary>
    public void TryAgain()
    {
        SceneManager.LoadScene(1);
    }
}
