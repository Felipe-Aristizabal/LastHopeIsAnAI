using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set;}
    private GameObject gameOverUI;
    private Dictionary<string, bool> sceneStates = new Dictionary<string, bool>
    {
        { "Tutorial", true },
        { "Level1", false },
        { "Level2", false },
        { "Bossfight", false }
    };

    void Awake()
    {
        // Check if an instance already exists and if it does, destroy the new one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (!sceneStates.ContainsKey(currentSceneName))
        {
            Destroy(gameObject);
        }
    }

    private void HandlePlayerDeath()
    {
        // Show the restart UI
        gameOverUI = GameObject.Find("CanvasUI").transform.GetChild(0).gameObject;
        gameOverUI.SetActive(true);
        Button buttonRestartGame = gameOverUI.GetComponentInChildren<Button>();

        buttonRestartGame.onClick.AddListener(() => {
            RestartGame();
        });
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDeath -= HandlePlayerDeath;
    }

    public void ChangeScene(string sceneName)
    {
        if (sceneStates.ContainsKey(sceneName))
        {
            foreach (var key in new List<string>(sceneStates.Keys))
            {
                sceneStates[key] = false;
            }

            sceneStates[sceneName] = true;

            if (sceneName == "Level1")
            {
                PlayerPrefs.SetInt("IsCompletedTutorial", 1);
                PlayerPrefs.Save();
            }

            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"Scene {sceneName} not found in sceneStates dictionary.");
        }
    }

    public string GetCurrentScene()
    {
        foreach (var scene in sceneStates)
        {
            if (scene.Value)
            {
                return scene.Key;
            }
        }
        return null;
    }

    public void RestartGame()
    {
        // Implement game restart logic, e.g., reload the scene
        ChangeScene("Tutorial");
    }
}
