using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : GameUnit
{
    public Transform brick;
    public enum playerTags { PlayerBlueBrick, PlayerRedBrick, PlayerYellowBrick, PlayerGreenBrick };
    public playerTags playerTag;
    public bool allowAll;
    public Rigidbody rgbody;
    private bool isOnGround;

    private void OnEnable()
    {
        if (allowAll)
        {
            rgbody.AddForce(new Vector3(Random.Range(-80f, 80f), 100f, Random.Range(-80f, 80f)));
            isOnGround = false;
        }
        else
        {
            isOnGround = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals(Constant.GROUND_TAG))
        {
            isOnGround = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOnGround)
        {
            if (other.tag.Contains(Constant.PLAYER_STRING) && allowAll)
            {
                SimplePool.Despawn(this);
            }
            else if (other.CompareTag(playerTag.ToString()))
            {
                SimplePool.Despawn(this);
            }
        }
    }
}
