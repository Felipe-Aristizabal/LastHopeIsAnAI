using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{

    private Rigidbody2D rigidbody2D;
    private GameObject player;

    private enum ActualPhase
    {
        Init,
        Phase1,
        Phase2,
        Phase3,
        BadEnding,
        Dead
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

    // Update is called once per frame
    void Update()
    {

    }

    void FollowPlayer(ActualPhase phase)
    {
        switch (phase)
        {
            case ActualPhase.Init:
                //The boss is Speaking
                break;
            case ActualPhase.Phase1:
                //The boss is Speaking
                break;
            case ActualPhase.Phase2:
                //The boss is Speaking
                break;
            case ActualPhase.Phase3:
                //The boss is Speaking
                break;
            case ActualPhase.Dead:
                //The boss is Speaking
                break;

        }
    }
}
