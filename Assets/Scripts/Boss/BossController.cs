using System;
using System.Collections;
using System.Collections.Generic;
using LLMUnitySamples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float meleeDamage;

    [Header("Chat Bot")]
    [SerializeField] private List<string> currentPrompt = new List<string>();
    [SerializeField] private BossChat bossChat;
    private GameObject TextBg;
    private Text IAdolfResponse;
    private bool isComplete = false;

    [Header("Phases Values")]
    [SerializeField] private int countAnswer = 0;
    [SerializeField] private int playerAnswer;
    [SerializeField] private bool playerIsHere;
    private Transform phase2Pos;
    private GameObject phase2Parent;

    public GameObject canva;
    private List<string> responses = new List<string>();
    private Rigidbody2D rigidbody2D;
    private GameObject player;
    private bool dialogueGenerated = false;
    private GameObject AnaiAnswers;
    private bool isBossScene = false;
    private bool isSearching = true;
    private SpriteRenderer spriteRenderer;
    public int PlayerAnswer
    {
        get { return playerAnswer; }
        set { playerAnswer = value; }
    }

    public bool PlayerIsHere
    {
        get { return playerIsHere; }
        set { playerIsHere = value; }
    }

    public enum ActualPhase
    {
        Idle = 0,
        Init = 1,
        Phase1 = 2,
        Phase2 = 3,
        Phase3 = 4,
        BadEnding = 5,
        Dead = 6
    };

    public ActualPhase bossPhase;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }
    void Start()
    {
        health = 350;
        rigidbody2D = GetComponent<Rigidbody2D>();

        if (GameObject.FindWithTag("Player"))
        {
            player = GameObject.FindWithTag("Player");
        }
        PlayerAnswer = 2;

    }
    public async void GenerateBossDialogue()
    {

        for (int i = responses.Count; i < currentPrompt.Count; i++)
        {
            Debug.Log("I'm starting");
            string response = await bossChat.SendMessage(currentPrompt[i]);
            Debug.Log(response);
            responses.Add(response);
            if (currentPrompt.Count == responses.Count)
            {
                isComplete = true;
                Debug.Log("Acabe SIUUUU");
            }
        }
    }

    void FixedUpdate()
    {
        if (bossChat.warmUpDone && !dialogueGenerated)
        {
            GenerateBossDialogue();
            dialogueGenerated = true;
        }

        if (SceneManager.GetActiveScene().name == "Boss")
        {
            isBossScene = true;
        }
        if (isBossScene)
        {
            if (isSearching)
            {
                TextBg = GameObject.Find("TextBg");
                IAdolfResponse = GameObject.Find("IAdolfResponse").GetComponent<Text>();
                phase2Parent = GameObject.Find("------PHASE 2-------");
                AnaiAnswers = GameObject.Find("AnaiAnswers");
                phase2Pos = GameObject.Find("PosPhase2").transform;
                StartCoroutine(ChangePlayerHere(GameObject.FindWithTag("Player")));
                spriteRenderer.enabled = true;
                IAdolfResponse.text = responses[0];
                canva.SetActive(true);
                isSearching = false;
            }

            if (playerIsHere && bossChat.warmUpDone)
            {
                if (responses.Count > 0 && player && bossPhase == ActualPhase.Idle)
                {
                    bossPhase = ActualPhase.Init;
                    ChageCurrentState(bossPhase);
                }
            }

            Debug.Log($"OUT {health}");
            if (bossPhase == ActualPhase.Phase1)
            {
                Debug.Log($"IN {health}");
                player = GameObject.Find("Player");
                FollowPlayer(moveSpeed, player.transform);
                if (health <= 175)
                {
                    bossPhase = ActualPhase.Phase2;
                    ChageCurrentState(bossPhase);
                }
            }
        }
    }

    void ChageCurrentState(ActualPhase phase)
    {
        switch (phase)
        {
            case ActualPhase.Idle:
                // Debug.Log($"Im in {phase}");
                break;
            case ActualPhase.Init:
                SpeeckingWithPlayer(PlayerAnswer);
                break;
            case ActualPhase.Phase1:
                AnaiAnswers.SetActive(false);

                break;
            case ActualPhase.Phase2:
                TextBg.SetActive(true);
                StartCoroutine(HideText(5));
                StartCoroutine(SmoothMoveToPosition(phase2Pos.position, 5f));

                IAdolfResponse.text = "Me unire a la red para obtener mas poder";
                phase2Parent.SetActive(true);
                break;
            case ActualPhase.BadEnding:
                IAdolfResponse.text = "Oh ya lo esperaba, no eres tan tonto como para dejarte llevar por las emociones";
                //The boss is Speaking
                StartCoroutine(HideText(5.0f));
                break;
            case ActualPhase.Dead:
                TextBg.SetActive(true);
                IAdolfResponse.text = "AAAHHHH SE ME RECALENTO EL PENTIUM";
                StartCoroutine(HideText(5.0f));
                break;
        }
    }

    IEnumerator SmoothMoveToPosition(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
    void SpeeckingWithPlayer(int response)
    {
        if (response == 0)
        {
            bossPhase = ActualPhase.BadEnding;
            ChageCurrentState(bossPhase);
            Debug.Log(response);
        }
        else if (response == 1)
        {
            Debug.Log(response);
            Debug.Log(countAnswer);

            if (countAnswer >= 2)
            {
                bossPhase = ActualPhase.Phase1;
                ChageCurrentState(bossPhase);
                StartCoroutine(HideText(5.0f));
            }
            else
            {
                IAdolfResponse.text = responses[countAnswer];
                countAnswer++;
                Debug.Log("Updated text: " + IAdolfResponse.text);
            }
        }
        else if (response == 2)
        {
            IAdolfResponse.text = responses[countAnswer];
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RecivesDamage(10);
        }
    }
    void FollowPlayer(float moveSpeed, Transform player)
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    public void DestroyTurret()
    {
        health -= 40;
    }
    public void DecisionPlayer(int answ)
    {
        PlayerAnswer = answ;
        SpeeckingWithPlayer(answ);
    }

    IEnumerator ChangePlayerHere(bool value)
    {
        yield return new WaitForSeconds(2);
        PlayerIsHere = value;
    }

    IEnumerator HideText(float value)
    {
        yield return new WaitForSeconds(value);
        TextBg.SetActive(false);
    }

    public void RecivesDamage(float damage)
    {
        health -= damage;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
