using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public List<Enemy> otherEnemies = new List<Enemy>();

    // Other variables
    private bool isFall;
    public Animator playerAnimator;
    public Transform winTransform;
    private bool isWin;

    private bool isOnBridge;

    public Brick normalBrick;

    private int currentStage;

    void Start()
    {
        OnInit();
    }

    private void OnInit()
    {
        currentStage = 0;
        inputX = 0;
        inputZ = 0;
        numOfStacks = 0;
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
        if (!isFall)
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

        if (isOnBridge && movement.z < 0)
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
        if (Physics.Raycast(playerTransform.position + Vector3.up * 0.05f, Vector3.forward, out RaycastHit hit, 0.1f, layer_mask))
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

    private void FixedUpdate()
    {
        if (!isFall || !isWin)
        {
            MoveCharacter(movement);
        }
    }

    void MoveCharacter(Vector3 direction)
    {
        playerRb.velocity = direction * moveSpeed * Time.deltaTime;
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
        if (collider.CompareTag(brickTag.ToString())|| collider.CompareTag("NormalBrick"))
        {
            Stack(collider.gameObject);
        }
        else if (collider.tag.Contains("Player"))
        {
            if (isOnBridge)
            {
                return;
            }
            foreach (Enemy otherEnemy in otherEnemies)
            {
                if (collider.CompareTag(otherEnemy.gameObject.tag))
                {
                    if (thisPlayer.GetNumOfStacks() < otherEnemy.GetNumOfStacks())
                    {
                        Fall();
                        return;
                    }
                }
            }
        }
        else
        {
            switch (collider.tag)
            {
                case Constant.NEW_STAGE_TAG:
                    currentStage++;
                    collider.tag = Constant.UNTAGGED_TAG;
                    poolController.LoadABrickInMap(currentStage, brick);
                    break;
                case Constant.FINISH_TAG:
                    collider.tag = Constant.UNTAGGED_TAG;
                    Win();
                    break;
                case "Bridge":
                    isOnBridge = true;
                    break;
                case "Ground":
                    isOnBridge = false;
                    break;
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
            GameUnit stack = SimplePool.DespawnNewest(stackPrefab);
            SimplePool.Spawn(normalBrick, stack.gameObject.transform.position, stack.gameObject.transform.rotation);
            numOfStacks--;
        }
        isFall = true;
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
