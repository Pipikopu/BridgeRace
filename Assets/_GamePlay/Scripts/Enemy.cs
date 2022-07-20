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
    private int stage;
    private bool isWin;
    public Transform winTransform;

    private void Start()
    {
        cornerIndex = 0;
        isWin = false;
        isUpgrade = false;
        targetPosition = enemyTransform.position + new Vector3(1, 0, 1);
        numOfStacks = 0;
        stage = 0;
        movement = Vector3.zero;
        agent.SetDestination(targetPosition);
        layer_mask = LayerMask.GetMask(Constant.MASK_STEP);
    }

    private void Update()
    {
        if (isWin)
        {
            return;
        }

        if (agent.hasPath)
        {
            //Debug.Log(cornerIndex);
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
                    targetPosition = stageTransform[stage].position;
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
        MoveCharacter(movement);
    }

    void MoveCharacter(Vector3 direction)
    {
        playerRb.velocity = direction * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(brickTag.ToString()))
        {
            Stack();
        }
        else
        {
            switch (collider.tag)
            {
                case Constant.NEW_STAGE_TAG:
                    collider.tag = Constant.UNTAGGED_TAG;
                    poolController.LoadStageOne(brick);
                    isUpgrade = true;
                    stage++;
                    break;
                case Constant.FINISH_TAG:
                    collider.tag = Constant.UNTAGGED_TAG;
                    Win();
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

    private void Stack()
    {
        SimplePool.Spawn(stackPrefab, stackHolder.position + stackHolder.up * numOfStacks * 0.06f, stackHolder.rotation);
        numOfStacks++;
    }
}

