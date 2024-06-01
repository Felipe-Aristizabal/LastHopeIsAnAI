using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set;}
    private GameObject gameOverUI;
    private GameObject playerGameObject;
    
    [SerializeField] private List<Sprite> spritesPowerUps;
    [SerializeField] private List<string> stringsPowerUps;


    private Dictionary<string, Sprite> powerUpDictionary;
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

        powerUpDictionary = new Dictionary<string, Sprite>();
        for (int i = 0; i < stringsPowerUps.Count; i++)
        {
            powerUpDictionary[stringsPowerUps[i]] = spritesPowerUps[i];
        }

        playerGameObject = GameObject.FindGameObjectWithTag("Player");

    }

    void Start()
    {
        // Assign the PowerUps if the Player can update
        // AssignRandomPowerUps();
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

    private void AssignRandomPowerUps()
    {
        GameObject powerUpParent = GameObject.FindGameObjectWithTag("AnimationPowerUp");
        if (powerUpParent != null)
        {
            Transform[] children = new Transform[powerUpParent.transform.childCount];
            for (int i = 0; i < powerUpParent.transform.childCount; i++)
            {
                children[i] = powerUpParent.transform.GetChild(i);
            }

            List<string> availableStrings = new List<string>(stringsPowerUps);
            List<Sprite> availableSprites = new List<Sprite>(spritesPowerUps);
            System.Random random = new System.Random();

            foreach (Transform child in children)
            {
                if (availableSprites.Count == 0)
                {
                    Debug.LogWarning("No more available sprites to assign.");
                    child.gameObject.SetActive(false);
                    continue;
                }

                bool assigned = false;
                int attempts = 0;
                int maxAttempts = availableStrings.Count; // Maximum attempts to find a valid power-up

                while (!assigned && attempts < maxAttempts)
                {
                    int randomIndex = random.Next(availableSprites.Count);
                    Sprite randomSprite = availableSprites[randomIndex];
                    string randomString = availableStrings[randomIndex];

                    if (CanAssignPowerUp(randomString))
                    {
                        availableSprites.RemoveAt(randomIndex);
                        availableStrings.RemoveAt(randomIndex);

                        Button childButton = child.GetComponent<Button>();
                        Image childImage = child.GetComponent<Image>();
                        if (childImage != null)
                        {
                            childImage.sprite = randomSprite;
                        } else { Debug.LogWarning("Child GameObject does not have an Image component."); }

                        child.gameObject.SetActive(true);

                        if (childButton != null)
                        {
                            AssignButtonListener(childButton, randomString);
                        } else { Debug.LogWarning("Child GameObject does not have a Button component."); }

                        assigned = true;
                    }
                    attempts++;
                }

                if (!assigned)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogWarning("GameObject with tag 'AnimationPowerUp' not found.");
        }
    }

    private bool CanAssignPowerUp(string powerUp)
    {
        PowerUp playerPowerUp = playerGameObject.GetComponent<PlayerPowerUps>()?.powerUp;
        if (playerPowerUp == null) return false;

        switch (powerUp)
        {
            case "+Speed":
                return playerPowerUp.MoveSpeed < playerPowerUp.ReturnMaxMoveSpeed(); 
            case "+MeleeDamage":
                return playerPowerUp.BaseDamage < playerPowerUp.ReturnMaxBaseDamage(); 
            case "+RangeDistance":
                return playerPowerUp.FireRange < playerPowerUp.ReturnMaxFireRange();
            case "+Health":
                return playerPowerUp.Health < playerPowerUp.ReturnMaxHealth();
            case "+DistanceAttackSpeed":
                return playerPowerUp.FireRate > playerPowerUp.ReturnMaxFireRate(); 
            case "+DistanceAttack":
                return playerPowerUp.FireDamage < playerPowerUp.ReturnMaxFireDamage(); 
            case "+CanDash":
                return !playerPowerUp.ReturnCanTeleport(); 
            default:
                Debug.LogWarning($"PowerUp {powerUp} not recognized.");
                return false;
        }
    }

    private void AssignButtonListener(Button button, string powerUp)
    {
        switch (powerUp)
        {
            case "+Speed":
                button.onClick.AddListener(() => {
                    IncreaseMoveSpeed(1f);
                });
                break;
            case "+MeleeDamage":
                button.onClick.AddListener(() => {
                    IncreaseDamage(1f);
                });
                break;
            case "+RangeDistance":
                button.onClick.AddListener(() => {
                    IncreaseFireRange(1f);
                });
                break;
            case "+Health":
                button.onClick.AddListener(() => {
                    IncreaseHealth(10f);
                });
                break;
            case "+DistanceAttackSpeed":
                button.onClick.AddListener(() => {
                    IncreaseFireRate(0.1f);
                });
                break;
            case "+DistanceAttack":
                button.onClick.AddListener(() => {
                    IncreaseFireDamege(1f);
                });
                break;
            case "+CanDash":
                button.onClick.AddListener(() => {
                    UnlockTeleport();
                });
                break;
            default:
                Debug.LogWarning($"PowerUp {powerUp} not recognized.");
                break;
        }
    }

    private void IncreaseMoveSpeed(float amount)
    {
        PowerUp powerUp = playerGameObject.GetComponent<PlayerPowerUps>()?.powerUp; 
        if (powerUp != null)
        {
            powerUp.IncreaseMoveSpeed(amount);
        } else { Debug.Log("I can not increase Movespeed"); }  
    }

    private void IncreaseDamage(float amount)
    {
        PowerUp powerUp = playerGameObject.GetComponent<PlayerPowerUps>()?.powerUp; 
        if (powerUp != null)
        {
            powerUp.IncreaseDamage(amount);
        } else { Debug.Log("I can not increase Damage"); }  
    }

    private void IncreaseFireRange(float amount)
    {
        PowerUp powerUp = playerGameObject.GetComponent<PlayerPowerUps>()?.powerUp; 
        if (powerUp != null)
        {
            powerUp.IncreaseFireRange(amount);
        } else { Debug.Log("I can not increase Firerange"); }  
    }

    private void IncreaseHealth(float amount)
    {
        PowerUp powerUp = playerGameObject.GetComponent<PlayerPowerUps>()?.powerUp; 
        if (powerUp != null)
        {
            powerUp.IncreaseHealth(amount);
        } else { Debug.Log("I can not increase Health"); }  
    }

    private void IncreaseFireRate(float amount)
    {
        PowerUp powerUp = playerGameObject.GetComponent<PlayerPowerUps>()?.powerUp; 
        if (powerUp != null)
        {
            powerUp.IncreaseFireRate(amount);
        } else { Debug.Log("I can not increase FireRate"); }  
    }

    private void IncreaseFireDamege(float amount)
    {
        PowerUp powerUp = playerGameObject.GetComponent<PlayerPowerUps>()?.powerUp; 
        if (powerUp != null)
        {
            powerUp.IncreaseFireDamage(amount);
        } else { Debug.Log("I can not increase FireDamage"); }  
    }

    private void UnlockTeleport()
    {
        PowerUp powerUp = playerGameObject.GetComponent<PlayerPowerUps>()?.powerUp; 
        if (powerUp != null)
        {
            powerUp.UnlockTeleport();
        } else { Debug.Log("I can not increase CanDash"); }  
    }

    public void RestartGame()
    {
        // Implement game restart logic, e.g., reload the scene
        ChangeScene("Tutorial");
    }
}
