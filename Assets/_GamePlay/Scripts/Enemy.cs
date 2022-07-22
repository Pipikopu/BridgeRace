using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform enemyTransform;
    public Transform modelTransform;

    private Vector3 targetPosition;
    private Vector3 cornerPosition;

    public List<Transform> stageTransform = new List<Transform>();

    // Move and rotate variables
    private Vector3 movement;
    public float moveSpeed;
    public float rotateSpeed;
    private float angleToRotate;

    public Rigidbody playerRb;

    public NavMeshAgent agent;

    public Brick brick;
    public enum brickTags { BlueBrick, RedBrick, GreenBrick, YellowBrick };
    public brickTags brickTag;

    // Stack variables
    public Stack stackPrefab;
    public Transform stackHolder;
    private int numOfStacks;
    public Material stackMaterial;
    public int numOfStacksToBuild;

    private int cornerIndex;
    private int cornerLength;
    public Animator playerAnimator;

    // Layer mask for step raycast
    private int layer_mask;

    // Step variables
    public string stepTag;
    public Material stepMaterial;

    public PoolController poolController;

    private bool isUpgrade;
    private int currentStage;
    private bool isWin;
    public Transform winTransform;

    private bool isFall;

    public Player player;
    public Enemy thisEnemy;
    public List<Enemy> otherEnemies = new List<Enemy>();

    public NavMeshObstacle agentObstacle;

    public Brick normalBrick;

    private bool isOnBridge;

    public Camera cameraMain;
    public Transform cameraWinTransform;
    public float cameraSpeed;

    private void Start()
    {
        cornerIndex = 0;
        isWin = false;
        isUpgrade = false;
        targetPosition = enemyTransform.position + new Vector3(1, 0, 1);
        numOfStacks = 0;
        currentStage = 0;
        isFall = false;
        isOnBridge = false;
        movement = Vector3.zero;
        agent.SetDestination(targetPosition);
        layer_mask = LayerMask.GetMask(Constant.MASK_STEP);
        agentObstacle.enabled = false;
    }

    private void Update()
    {
        if (isWin)
        {
            cameraMain.transform.position = Vector3.MoveTowards(cameraMain.transform.position, cameraWinTransform.position, cameraSpeed * Time.deltaTime);
            if (Vector3.Distance(cameraMain.transform.position, cameraWinTransform.position) < 0.1f)
            {
                StartCoroutine(openEndGameMenu());
            }
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
                cornerPosition = agent.path.corners[cornerIndex];
                if (Vector3.Distance(enemyTransform.position, cornerPosition) < 0.1f)
                {
                    cornerIndex++;
                    movement = Vector3.zero;
                }
                else
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
                    angleToRotate = Mathf.Rad2Deg * Mathf.Atan2(movement.x, movement.z);
                    modelTransform.rotation = Quaternion.RotateTowards(modelTransform.rotation, Quaternion.AngleAxis(angleToRotate, Vector3.up), rotateSpeed * Time.deltaTime);
                    playerAnimator.SetBool(Constant.ANIM_IS_RUN, true);
                }
            }
            else if (cornerIndex == cornerLength)
            {
                cornerPosition = targetPosition;
                if (Vector3.Distance(enemyTransform.position, cornerPosition) < 0.1f)
                {
                    cornerIndex++;
                    movement = Vector3.zero;
                }
                else
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
                    angleToRotate = Mathf.Rad2Deg * Mathf.Atan2(movement.x, movement.z);
                    modelTransform.rotation = Quaternion.RotateTowards(modelTransform.rotation, Quaternion.AngleAxis(angleToRotate, Vector3.up), rotateSpeed * Time.deltaTime);
                    playerAnimator.SetBool(Constant.ANIM_IS_RUN, true);
                }
            }
            else
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
        }
    }

    public bool CanGo()
    {
        if (Physics.Raycast(enemyTransform.position + Vector3.up * 0.05f, Vector3.forward, out RaycastHit hit, 0.1f, layer_mask))
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

    private void FixedUpdate()
    {
        if (!isFall)
        {
            MoveCharacter(movement);
        }
    }

    void MoveCharacter(Vector3 direction)
    {
        playerRb.velocity = direction * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (isFall)
        {
            return;
        }
        if (collider.CompareTag(brickTag.ToString()) || collider.CompareTag("NormalBrick"))
        {
            Stack(collider.gameObject);
        }
        else if (collider.tag.Contains("Player"))
        {
            if (isOnBridge)
            {
                return;
            }
            if (collider.CompareTag(player.gameObject.tag))
            {
                if (thisEnemy.GetNumOfStacks() < player.GetNumOfStacks())
                {
                    Fall();
                    return;
                }
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
        else
        {
            switch (collider.tag)
            {
                case Constant.NEW_STAGE_TAG:
                    currentStage++;
                    collider.tag = Constant.UNTAGGED_TAG;
                    poolController.LoadABrickInMap(currentStage, brick);
                    isUpgrade = true;
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
        UIManager.Ins.onOpenLoseGameMenu();
    }
}

