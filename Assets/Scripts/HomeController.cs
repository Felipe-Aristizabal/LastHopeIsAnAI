using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeController : MonoBehaviour
{
    [SerializeField] private GameObject homeUIGameObject;
    [SerializeField] private GameObject creditsUIGameObject;
    
    [Space(10)]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button creditsUIButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button exitCreditsUIGameButton;
    [SerializeField] private AudioSource buttonAudio;

    void Start()
    {
        startGameButton.onClick.AddListener(() => {
            StartGame();
        });

        exitGameButton.onClick.AddListener(() => {
            ExitGame();
        });

        creditsUIButton.onClick.AddListener(() => {
            ChangeActiveUIGameObject(creditsUIGameObject, homeUIGameObject);
        });

        exitCreditsUIGameButton.onClick.AddListener(() => {
            ChangeActiveUIGameObject(homeUIGameObject, creditsUIGameObject);
        });
    }

    private void StartGame()
    {
        buttonAudio.Play();
        if (PlayerPrefs.GetInt("IsCompletedTutorial", 0) == 1)
        {
            SceneManager.LoadScene("Level1");
        }
        else
        {
            SceneManager.LoadScene("Tutorial");
        }
    }

    private void ExitGame()
    {
        buttonAudio.Play();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    private void ChangeActiveUIGameObject(GameObject gameObjectToActive, GameObject gameObjectToDeactive)
    {
        buttonAudio.Play();
        gameObjectToActive.SetActive(true);
        gameObjectToDeactive.SetActive(false);
    }
}
