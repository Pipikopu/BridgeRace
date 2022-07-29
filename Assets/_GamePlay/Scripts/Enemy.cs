using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // TODO: Seperate Script Into Smaller Ones

    // Enemy variables
    public Transform enemyTransform;
    public Transform modelTransform;
    public Rigidbody enemyRb;

    // Variables for moving by NavMesh
    public NavMeshAgent agent;
    public NavMeshObstacle agentObstacle;

    private int cornerIndex;
    private int cornerLength;
    private Vector3 targetPosition;
    private Vector3 cornerPosition;

    public List<Transform> stageTransform = new List<Transform>();

    // Move and rotate variables
    private Vector3 movement;
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
    public Material stackMaterial;
    public int numOfStacksToBuild;

    // Layer mask for step raycast
    private int layer_mask;

    // Step variables
    public Constant.StepTags stepTag;
    public Material stepMaterial;
    public string stepMatName;

    // Pool controller
    public PoolController poolController;

    // Enemy and other playables
    public Enemy thisEnemy;
    public Player player;
    public List<Enemy> otherEnemies = new List<Enemy>();

    // Camera variables
    public Camera cameraMain;
    public Transform cameraWinTransform;
    public float cameraSpeed;

    // Other variables
    private bool isFall;
    private bool isOnBridge;
    private bool isUpgrade;
    private int currentStage;
    private bool isWin;
    public Transform winTransform;
    public Animator playerAnimator;

    public bool isHitNextStage;

    private void Start()
    {
        isHitNextStage = false;
        isWin = false;
        isUpgrade = false;
        isFall = false;
        isOnBridge = false;
        stepMatName = stepMaterial.name;

        cornerIndex = 0;
        numOfStacks = 0;
        currentStage = 0;
        movement = Vector3.zero;

        targetPosition = enemyTransform.position + new Vector3(1, 0, 1);
        agent.SetDestination(targetPosition);
        layer_mask = LayerMask.GetMask(Constant.MASK_STEP);
        agentObstacle.enabled = false;
    }

    private void Update()
    {
        if (isWin)
        {
            MoveCamera();
            return;
        }

        if (isFall)
        {
            return;
        }

        if (agent.hasPath)
        {
            cornerLength = agent.path.corners.Length;
            if (cornerIndex < cornerLength)
            {
                MoveToNextCorner();
            }
            else if (cornerIndex == cornerLength)
            {
                MoveToTarget();
            }
            else
            {
                ChangeTarget();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isFall)
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
        if (collider.CompareTag(brickTag.ToString()) || collider.CompareTag(Constant.NORMAL_BRICK_TAG))
        {
            Stack(collider.gameObject);
        }
        // it other playables
        else if (collider.tag.Contains(Constant.PLAYER_STRING))
        {
            // Is on bridge, no interraction
            
            // Hit player
            if (collider.CompareTag(player.gameObject.tag))
            {
                CollideWithPlayer();
                return;
            }
            // Hit other enemies
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
                    if (!isHitNextStage) {
                        currentStage++;
                        poolController.LoadABrickInMap(currentStage, brick);
                        isUpgrade = true;
                        isHitNextStage = true;
                        ChangeTarget();
                    }
                    break;
                // Hit finish point
                case Constant.FINISH_TAG:
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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constant.NEW_STAGE_TAG))
        {
            isHitNextStage = false;
        }
    }

    private void CollideWithOthers(Collider collider)
    {
        if (isOnBridge)
        {
            Avoid();
            return;
        }
        foreach (Enemy otherEnemy in otherEnemies)
        {
            if (collider.CompareTag(otherEnemy.gameObject.tag))
            {
                if (thisEnemy.GetNumOfStacks() < otherEnemy.GetNumOfStacks())
                {
                    Fall();
                    return;
                }
            }
        }
        agent.SetDestination(targetPosition);
    }

    private void CollideWithPlayer()
    {
        if (isOnBridge)
        {
            return;
        }
        if (thisEnemy.GetNumOfStacks() < player.GetNumOfStacks())
        {
            Fall();
        }
    }

    void MoveCharacter(Vector3 direction)
    {
        enemyRb.velocity = direction * moveSpeed * Time.deltaTime;
    }

    private void ChangeTarget()
    {
        cornerIndex = 0;
        if (numOfStacks >= numOfStacksToBuild && !isUpgrade)
        {
            targetPosition = stageTransform[currentStage].position;
            agent.SetDestination(targetPosition);
        }
        else
        {
            if (isUpgrade)
            {
                isUpgrade = false;
            }
            targetPosition = SimplePool.GetFirstAcObjPos(brick, enemyTransform.position);
            agent.SetDestination(targetPosition);
        }
    }

    private void MoveToTarget()
    {
        cornerPosition = targetPosition;
        if (Vector3.Distance(enemyTransform.position, cornerPosition) < 0.1f)
        {
            cornerIndex++;
            movement = Vector3.zero;
        }
        else
        {
            SetMovement();
            SetRotation();
            playerAnimator.SetBool(Constant.ANIM_IS_RUN, true);
        }
    }

    private void MoveToNextCorner()
    {
        cornerPosition = agent.path.corners[cornerIndex];
        if (Vector3.Distance(enemyTransform.position, cornerPosition) < 0.08f)
        {
            cornerIndex++;
            movement = Vector3.zero;
        }
        else
        {
            SetMovement();
            SetRotation();
            playerAnimator.SetBool(Constant.ANIM_IS_RUN, true);
        }
    }

    private void SetMovement()
    {
        movement = (cornerPosition - enemyTransform.position).normalized;
        if (!CanGo())
        {
            if (movement.z > 0)
            {
                movement.z = 0;
            }

            cornerIndex = 0;
            targetPosition = SimplePool.GetFirstAcObjPos(brick, enemyTransform.position);
            agent.SetDestination(targetPosition);
            return;
        }
    }

    private void SetRotation()
    {
        angleToRotate = Mathf.Rad2Deg * Mathf.Atan2(movement.x, movement.z);
        modelTransform.rotation = Quaternion.RotateTowards(modelTransform.rotation, Quaternion.AngleAxis(angleToRotate, Vector3.up), rotateSpeed * Time.deltaTime);
    }

    private void MoveCamera()
    {
        cameraMain.transform.position = Vector3.MoveTowards(cameraMain.transform.position, cameraWinTransform.position, cameraSpeed * Time.deltaTime);
        if (Vector3.Distance(cameraMain.transform.position, cameraWinTransform.position) < 0.1f)
        {
            StartCoroutine(openEndGameMenu());
        }
    }

    public bool CanGo()
    {
        if (Physics.Raycast(enemyTransform.position + Vector3.up * 0.05f, Vector3.forward, out RaycastHit hit, 0.1f, layer_mask))
        {
            string hitMatName = poolController.GetMatString(hit.collider.gameObject);
            if (hitMatName.Contains(stepMatName))
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
        Renderer render = poolController.GetRenderer(colliderObj);
        render.material = stepMaterial;
        render.enabled = true;

        SimplePool.DespawnNewest(stackPrefab);
        SimplePool.SpawnOldest(brick);

        numOfStacks--;
    }

    private void Win()
    {
        movement = Vector3.zero;
        isWin = true;

        SimplePool.CollectAPool(stackPrefab);
        enemyTransform.position = winTransform.position;
        playerAnimator.Play(Constant.ANIM_WIN);
    }

    private void Stack(GameObject colliderObj)
    {
        Stack newStack = (Stack)SimplePool.SpawnWithParent(stackPrefab, stackHolder.position + stackHolder.up * numOfStacks * 0.06f, stackHolder.rotation, stackHolder);
        numOfStacks++;
    }

    public int GetNumOfStacks()
    {
        return numOfStacks;
    }

    IEnumerator Avoid()
    {
        agent.enabled = false;
        agentObstacle.enabled = true;
        yield return new WaitForSeconds(1.5f);
        agentObstacle.enabled = false;
        agent.enabled = true;
        agent.SetDestination(targetPosition);
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
        agent.enabled = false;
        agentObstacle.enabled = true;
        yield return new WaitForSeconds(1.5f);
        playerAnimator.SetBool(Constant.ANIM_IS_FALL, false);
        isFall = false;
        agentObstacle.enabled = false;
        agent.enabled = true;
        agent.SetDestination(targetPosition);
    }

    IEnumerator openEndGameMenu()
    {
        yield return new WaitForSeconds(3f);
        UIManager.Ins.OpenUI(UIID.UICFail);
        Time.timeScale = 0;
    }
}

