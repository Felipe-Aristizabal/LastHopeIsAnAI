using System.Collections;
using System.Collections.Generic;
using LLMUnitySamples;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float meleeDamage;

    [Header("Chat Bot")]
    [SerializeField] private BossChat bossChat;
    [SerializeField] private List<string> currentPrompt = new List<string>();
    [SerializeField] private GameObject TextBg;
    [SerializeField] private Text IAdolfResponse;
    private bool isComplete = false;

    [Header("Phases Values")]
    [SerializeField] private int countAnswer = 0;
    [SerializeField] private int playerAnswer;
    [SerializeField] private bool playerIsHere;
    [SerializeField] private GameObject phase2Parent;
    [SerializeField] private List<LaserRotate> laserController = new List<LaserRotate>();

    private List<string> responses = new List<string>();
    private Rigidbody2D rigidbody2D;
    private GameObject player;
    private bool dialogueGenerated = false;

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


    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        if (GameObject.FindWithTag("Player"))
        {
            player = GameObject.FindWithTag("Player");
            StartCoroutine(ChangePlayerHere(GameObject.FindWithTag("Player")));
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
                TextBg.SetActive(true);
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
        if (playerIsHere && bossChat.warmUpDone)
        {
            if (responses.Count > 0 && player && bossPhase == ActualPhase.Idle)
            {
                bossPhase = ActualPhase.Init;
                ChageCurrentState(bossPhase);
            }
        }

        if (bossPhase == ActualPhase.Phase1)
        {
            FollowPlayer(moveSpeed, player.transform);
            if (health <= health * 0.5)
            {
                bossPhase = ActualPhase.Phase2;
                ChageCurrentState(bossPhase);
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

                break;
            case ActualPhase.Phase2:
                TextBg.SetActive(true);
                StartCoroutine(HideText(5));
                // IAdolfResponse.text =  responses[countAnswer];
                IAdolfResponse.text = "Me unire a la red para obtener mas poder";
                phase2Parent.SetActive(true);
                foreach (LaserRotate laser in laserController)
                {
                    laser.isPhase1 = true;
                }
                break;
            case ActualPhase.BadEnding:
                IAdolfResponse.text = "Oh ya lo esperaba, no eres tan tonto como para dejarte llevar por las emociones";
                //The boss is Speaking
                StartCoroutine(HideText(5.0f));
                break;
            case ActualPhase.Dead:
                IAdolfResponse.text = "AAAHHHH SE ME RECALENTO EL PENTIUM";
                StartCoroutine(HideText(5.0f));
                break;
        }
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


    void FollowPlayer(float moveSpeed, Transform player)
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
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
