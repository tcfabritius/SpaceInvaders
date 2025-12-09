using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the overall game state, including score, level progress,
/// saving/loading, and scene transitions. Implements the singleton pattern.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the GameManager.
    /// </summary>
    public static GameManager instance;

    /// <summary>
    /// Current player score.
    /// </summary>
    public int score;

    /// <summary>
    /// UI text element used to display the score.
    /// </summary>
    TextMeshProUGUI scoreText;

    /// <summary>
    /// Dictionary storing level completion status.
    /// Key = level name, Value = completed (true) or not (false).
    /// </summary>
    public Dictionary<string, bool> status = new Dictionary<string, bool>();
    
    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the singleton instance and ensures only one GameManager exists.
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject); ///< Persist between scenes
            instance = this;                ///< Set singleton instance
        }
        else
        {
            Destroy(gameObject);           ///< Destroy duplicate manager
        }
    }

    /// <summary>
    /// Starts the game by loading the next scene in build order.
    /// </summary>
    public void StartGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
    
    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
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
    /// Saves the current game state, including score and level progress, to a file.
    /// </summary>
    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.score = score;

        foreach (var kvp in status)
        {
            data.levelNames.Add(kvp.Key);
            data.levelStatuses.Add(kvp.Value);
        }

        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// Loads the saved game state from a file, restoring score and level progress.
    /// Automatically loads the first uncompleted level or the win screen if all completed.
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            score = data.score;
            status.Clear();

            for (int i = 0; i < data.levelNames.Count; i++)
            {
                status[data.levelNames[i]] = data.levelStatuses[i];
            }

            string firstUncompletedLevel = null;
            for (int i = 1; i <= 4; i++)
            {
                string levelName = "level" + i;
                if (status.ContainsKey(levelName) && !status[levelName])
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
    /// Called before the first frame update.
    /// Initializes level completion statuses.
    /// </summary>
    void Start()
    {
        status["level1"] = false;
        status["level2"] = false;
        status["level3"] = false;
        status["level4"] = false;
    }

    /// <summary>
    /// Called once per frame.
    /// Updates the score display if the current scene is the lose or win screen.
    /// </summary>
    void Update()
    {
        string SceneName = SceneManager.GetActiveScene().name;
        if (SceneName == "LoseScreen" || SceneName == "WinScreen")
        {
            scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
            scoreText.text = score.ToString();
        }
    }
}

/// <summary>
/// Serializable class used for saving and loading player data,
/// including score and level completion status.
/// </summary>
[Serializable]
public class PlayerData
{
    /// <summary>
    /// Saved player score.
    /// </summary>
    public int score;

    /// <summary>
    /// List of level names.
    /// </summary>
    public List<string> levelNames = new List<string>();

    /// <summary>
    /// List of level completion statuses corresponding to <see cref="levelNames"/>.
    /// </summary>
    public List<bool> levelStatuses = new List<bool>();
}
