using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollider : MonoBehaviour
{
    private PlayerController playerController;

    public void Initialize(PlayerController controller)
    {
        playerController = controller;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        playerController.OnMeleeAttackTriggerEnter2D(other);
    }
}
