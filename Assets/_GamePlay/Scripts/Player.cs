using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // TODO: Seperate Script Into Smaller Ones

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
    public Constant.BrickTags brickTag;
    public Brick normalBrick;

    // Stack variables
    public Stack stackPrefab;
    public Transform stackHolder;
    private int numOfStacks;

    // Layer mask for step raycast
    private int layer_mask;
    // Step variables
    public Constant.StepTags stepTag;
    public Material stepMaterial;

    // Pool controller
    public PoolController poolController;

    // Player and other playables
    public Player thisPlayer;
    public List<Enemy> otherEnemies = new List<Enemy>();

    // Other variables
    private bool isFall;
    private bool isOnBridge;
    private bool isWin;
    private int currentStage;
    public Transform winTransform;
    public Animator playerAnimator;

    void Start()
    {
        OnInit();
    }

    private void OnInit()
    {
        inputX = 0;
        inputZ = 0;

        numOfStacks = 0;
        currentStage = 0;
        isWin = false;
        isFall = false;
        isOnBridge = false;

        layer_mask = LayerMask.GetMask(Constant.MASK_STEP);
    }

    void Update()
    {
        if (isWin)
        {
            StartCoroutine(openEndGameMenu());
            return;
        }
        else if (isFall)
        {
            movement = Vector3.zero;
            return;
        }
        else Move();
    }

    private void Move()
    {
        inputX = joystick.inputHorizontal();
        inputZ = joystick.inputVertical();

        if (inputX == 0 && inputZ == 0)
        {
            Stop();
            return;
        }
        else
        {
            playerAnimator.SetBool(Constant.ANIM_IS_RUN, true);
            SetRotation();
            SetMovement();
        }
    }

    private void FixedUpdate()
    {
        if (!isFall || !isWin)
        {
            MoveCharacter(movement);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Is fall
        if (isFall)
        {
            return;
        }
        // Hit edible bricks
        if (collider.CompareTag(brickTag.ToString())|| collider.CompareTag(Constant.NORMAL_BRICK_TAG))
        {
            Stack(collider.gameObject);
        }
        // Hit other playables
        else if (collider.tag.Contains(Constant.PLAYER_STRING))
        {
            if (isOnBridge)
            {
                return;
            }
            else
            {
                CollideWithOthers(collider);
            }
        }
        // Other triggers
        else
        {
            switch (collider.tag)
            {
                // Go to new stage
                case Constant.NEW_STAGE_TAG:
                    collider.tag = Constant.UNTAGGED_TAG;
                    currentStage++;
                    poolController.LoadABrickInMap(currentStage, brick);
                    break;
                // Hit finish point
                case Constant.FINISH_TAG:
                    collider.tag = Constant.UNTAGGED_TAG;
                    Win();
                    break;
                // Is on bridge
                case Constant.BRIDGE_TAG:
                    isOnBridge = true;
                    break;
                // Is on ground
                case Constant.GROUND_TAG:
                    isOnBridge = false;
                    break;
            }
        }
    }

    private void Stop()
    {
        playerAnimator.SetBool(Constant.ANIM_IS_RUN, false);
        movement = Vector3.zero;
    }

    private void SetRotation()
    {
        angleToRotate = Mathf.Rad2Deg * Mathf.Atan2(inputX, inputZ);
        modelTransform.rotation = Quaternion.RotateTowards(modelTransform.rotation, Quaternion.AngleAxis(angleToRotate, Vector3.up), rotateSpeed * Time.deltaTime);
    }

    private void SetMovement()
    {
        // Can not go up
        if (!CanGo() && inputZ > 0)
        {
            inputZ = 0;
        }

        // Go down bridge
        if (isOnBridge && movement.z < 0)
        {
            movement = new Vector3(inputX, inputZ * 0.7f, inputZ);
        }
        // Normal movement
        else
        {
            movement = new Vector3(inputX, 0, inputZ);
        }
    }

    public bool CanGo()
    {
        if (Physics.Raycast(playerTransform.position + Vector3.up * 0.05f, Vector3.forward, out RaycastHit hit, 0.1f, layer_mask))
        {
            if (hit.collider.tag.Equals(stepTag.ToString()))
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

    private void BuildStep(GameObject colliderObj)
    {
        // Set renderer of step
        Renderer render = poolController.GetMaterial(colliderObj);
        render.material = stepMaterial;
        render.enabled = true;

        colliderObj.tag = stepTag.ToString();

        SimplePool.DespawnNewest(stackPrefab);
        SimplePool.SpawnOldest(brick);

        numOfStacks--;
    }

    void MoveCharacter(Vector3 direction)
    {
        playerRb.velocity = direction * moveSpeed * Time.deltaTime;
    }

    private void CollideWithOthers(Collider collider)
    {
        foreach (Enemy otherEnemy in otherEnemies)
        {
            if (collider.CompareTag(otherEnemy.gameObject.tag))
            {
                // Compare num of stacks
                if (thisPlayer.GetNumOfStacks() < otherEnemy.GetNumOfStacks())
                {
                    Fall();
                    return;
                }
            }
        }
    }

    public int GetNumOfStacks()
    {
        return numOfStacks;
    }

    private void Stack(GameObject colliderObj)
    {
        SimplePool.SpawnWithParent(stackPrefab, stackHolder.position + stackHolder.up * numOfStacks * 0.06f, stackHolder.rotation, stackHolder);
        numOfStacks++;
    }

    private void Win()
    {
        isWin = true;
        movement = Vector3.zero;

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
        isFall = true;
        playerAnimator.SetBool(Constant.ANIM_IS_FALL, true);
        while (numOfStacks > 0)
        {
            GameUnit stack = SimplePool.DespawnNewest(stackPrefab);
            SimplePool.Spawn(normalBrick, stack.gameObject.transform.position, stack.gameObject.transform.rotation);
            numOfStacks--;
        }

        yield return new WaitForSeconds(1.5f);

        playerAnimator.SetBool(Constant.ANIM_IS_FALL, false);
        isFall = false;
    }

    IEnumerator openEndGameMenu()
    {
        yield return new WaitForSeconds(3f);
        UIManager.Ins.onOpenWinGameMenu();
    }
}
