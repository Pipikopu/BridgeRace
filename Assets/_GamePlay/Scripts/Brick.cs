using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : GameUnit
{
    public Transform brick;
    public enum playerTags { PlayerBlueBrick, PlayerRedBrick, PlayerYellowBrick, PlayerGreenBrick };
    public playerTags playerTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag.ToString()))
        {
            SimplePool.Despawn(this);
        }
    }
}
