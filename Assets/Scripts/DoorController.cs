using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] Transform leftDoorPart = default;
    [SerializeField] Transform rightDoorPart = default;
    bool doorsOpened = false;

    private void Awake()
    {
        PlayerBehaviour.OpenTheDoor += YouShallPass;
    }
    void YouShallPass()
    {
        if (!doorsOpened)
        {
            leftDoorPart.LeanMoveX(leftDoorPart.position.x - 1, 1f).setEaseOutQuad();
            rightDoorPart.LeanMoveX(rightDoorPart.position.x + 1, 1f).setEaseOutQuad();
            doorsOpened = true;
        }
    }
}
