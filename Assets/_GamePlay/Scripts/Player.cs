using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Player variables
    public Transform playerTransform;
    public Transform modelTransform;
    public Rigidbody playerRb;

    // Joystick variables
    public JoytickController joystick;
    private float inputX;
    private float inputZ;

    // Move and rotate variables
    public Vector3 movement;
    public float moveSpeed;
    public float rotateSpeed;
    private float angleToRotate;

    // Brick variables
    public Brick brick;
    public enum brickTags { BlueBrick, RedBrick, GreenBrick, YellowBrick };
    public brickTags brickTag;

    // Stack variables
    public Stack stackPrefab;
    public Transform stackHolder;
    private int numOfStacks;

    // Step variables
    public string stepTag;
    public Material stepMaterial;

    // Layer mask for step raycast
    private int layer_mask;

    // Pool controller
    public PoolController poolController;

    // Player and Other Players
    public Player thisPlayer;
    public List<Player> otherPlayers = new List<Player>();

    // Other variables
    private bool isFall;
    public Animator playerAnimator;
    public Transform winTransform;
    private bool isWin;

    void Start()
    {
        OnInit();
    }

    private void OnInit()
    {
        inputX = 0;
        inputZ = 0;
        numOfStacks = 0;
        isWin = false;
        isFall = false;
        layer_mask = LayerMask.GetMask(Constant.MASK_STEP);
    }

    void Update()
    {
        if (!isWin)
        {
            Move();
        }
    }

    private void Move()
    {
        inputX = joystick.inputHorizontal();
        inputZ = joystick.inputVertical();

        if (inputX == 0 && inputZ == 0)
        {
            playerAnimator.SetBool(Constant.ANIM_IS_RUN, false);
            movement = Vector3.zero;
            return;
        }
        playerAnimator.SetBool(Constant.ANIM_IS_RUN, true);

        angleToRotate = Mathf.Rad2Deg * Mathf.Atan2(inputX, inputZ);
        modelTransform.rotation = Quaternion.RotateTowards(modelTransform.rotation, Quaternion.AngleAxis(angleToRotate, Vector3.up), rotateSpeed * Time.deltaTime);

        if (!CanGo() && inputZ > 0)
        {
            inputZ = 0;
        }

        if (OnBridge())
        {
            movement = new Vector3(inputX, inputZ * 0.7f, inputZ);
        }
        else
        {
            movement = new Vector3(inputX, 0, inputZ);
        }
    }

    public bool CanGo()
    {
        if (Physics.Raycast(playerTransform.position + Vector3.up * 0.05f, Vector3.forward, out RaycastHit hit, 0.08f, layer_mask))
        {
            if (hit.collider.tag == stepTag)
            {
                return true;
            }
            else if (numOfStacks == 0)
            {
                return false;
            }
            else
            {
                BuildStep(hit.collider.gameObject);
                return true;
            }
        }

        return true;
    }

    public bool OnBridge()
    {
        if (Physics.Raycast(playerTransform.position, Vector3.forward, out RaycastHit hit, 0.1f, layer_mask))
        {
            return true;
        }
        return false;
    }

    private void FixedUpdate()
    {
        MoveCharacter(movement);
    }

    void MoveCharacter(Vector3 direction)
    {
        playerRb.velocity = direction * moveSpeed;
    }

    private void BuildStep(GameObject colliderObj)
    {
        Renderer render = poolController.GetMaterial(colliderObj);
        render.material = stepMaterial;
        render.enabled = true;

        colliderObj.tag = stepTag;

        SimplePool.DespawnNewest(stackPrefab);
        SimplePool.SpawnOldest(brick);

        numOfStacks--;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (isFall)
        {
            return;
        }
        if (collider.CompareTag(brickTag.ToString()))
        {
            Stack();
        }
        else if (collider.tag.Contains("Player"))
        {
            foreach(Player otherPlayer in otherPlayers)
            {
                if (collider.CompareTag(otherPlayer.gameObject.tag))
                {
                    if (OnBridge())
                    {
                        return;
                    }
                    else if (thisPlayer.GetNumOfStacks() < otherPlayer.GetNumOfStacks())
                    {
                        Fall();
                    }
                }
            }
        }
        else
        {
            switch (collider.tag)
            {
                case Constant.NEW_STAGE_TAG:
                    collider.tag = Constant.UNTAGGED_TAG;
                    poolController.LoadStageOne(brick);
                    break;
                case Constant.FINISH_TAG:
                    collider.tag = Constant.UNTAGGED_TAG;
                    Win();
                    break;
            }
        }
    }

    public int GetNumOfStacks()
    {
        return numOfStacks;
    }

    private void Stack()
    {
        SimplePool.Spawn(stackPrefab, stackHolder.position + stackHolder.up * numOfStacks * 0.06f, stackHolder.rotation);
        numOfStacks++;
    }

    private void Win()
    {
        movement = Vector3.zero;
        isWin = true;

        SimplePool.CollectAPool(stackPrefab);
        playerTransform.position = winTransform.position;
        playerAnimator.Play(Constant.ANIM_WIN);
    }

    private void Fall()
    {
        StartCoroutine(NotFall());
    }

    IEnumerator NotFall()
    {
        playerAnimator.SetBool(Constant.ANIM_IS_FALL, true);
        while (numOfStacks > 0)
        {
            SimplePool.DespawnNewest(stackPrefab);
            SimplePool.SpawnOldest(brick);
            numOfStacks--;
        }
        isFall = true;
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool(Constant.ANIM_IS_FALL, false);
        isFall = false;

    }
}
