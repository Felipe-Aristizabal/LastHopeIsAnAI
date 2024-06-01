using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set;}
    private GameObject gameOverUI;

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

    public void RestartGame()
    {
        // Implement game restart logic, e.g., reload the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("CoreScene");
    }
}
