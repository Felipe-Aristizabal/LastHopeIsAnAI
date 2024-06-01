using System.Collections;
using System.Collections.Generic;
using LLMUnitySamples;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [SerializeField] private BossChat bossChat;
    [SerializeField] private List<string> currentPrompt = new List<string>();
    [SerializeField] private Text IAdolfResponse;

    private List<string> responses = new List<string>();
    private Rigidbody2D rigidbody2D;
    private GameObject player;

    private enum ActualPhase
    {
        Idle = 0,
        Init = 1,
        Phase1 = 2,
        Phase2 = 3,
        Phase3 = 4,
        BadEnding = 5,
        Dead = 6
    };

    [SerializeField] private ActualPhase bossPhase;


    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        if (GameObject.FindWithTag("Player"))
        {
            player = GameObject.FindWithTag("Player");
        }



    }
    public async void GenerateBossDialogue()
    {
        for (int i = responses.Count; i < currentPrompt.Count; i++)
        {
            Debug.Log("I'm starting");
            string response = await bossChat.SendMessage(currentPrompt[0]);
            Debug.Log(response);
            responses.Add(response);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bossChat.warmUpDone)
        {
            GenerateBossDialogue();
            if (responses.Count > 0)
            {
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
                // StartCoroutine(TypeText(responses[0]));
                IAdolfResponse.text = responses[0];
                // Debug.Log($"Im in {phase}");
                break;
            case ActualPhase.Phase1:
                Debug.Log($"Im in {phase}");
                // StartCoroutine(TypeText(responses[4]));
                //The boss is Speaking
                break;
            case ActualPhase.Phase2:
                Debug.Log($"Im in {phase}");
                //The boss is Speaking
                break;
            case ActualPhase.Phase3:
                Debug.Log($"Im in {phase}");
                //The boss is Speaking
                break;
            case ActualPhase.Dead:
                Debug.Log($"Im in {phase}");
                //The boss is Speaking
                break;
        }
    }

    private IEnumerator TypeText(string text)
    {
        IAdolfResponse.text = "";
        foreach (char letter in text.ToCharArray())
        {
            IAdolfResponse.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
