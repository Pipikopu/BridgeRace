using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolController : MonoBehaviour
{
    public Transform poolTransform;
    public Transform newPoolTransform;

    public Transform tf_redBrick;
    public Transform tf_greenBrick;
    public Transform tf_blueBrick;
    public Transform tf_yellowBrick;

    public Transform tf_blueStackHolder;
    public Transform tf_redStackHolder;

    public Transform tf_steps;

    public Transform tf_steps_0;
    public Transform tf_steps_1;
    public Transform tf_steps_2;

    public Brick redBrick;
    public Brick greenBrick;
    public Brick blueBrick;
    public Brick yellowBrick;
    public Stack blueStack;
    public Stack redStack;

    public Step step;
    private int numOfStepsPerSlide;

    private int brickIndex;

    private Dictionary<GameObject, Renderer> objRenDict = new Dictionary<GameObject, Renderer>();

    // RED = 0, GREEN = 1, BLUE = 2, YELLOW = 3
    private int[,] map2D = new int[,] { { 0, 1, 3, 2, 3, 2, 0, 2, 2, 3, 1, 2},
                                        { 3, 2, 3, 2, 1, 0, 1, 0, 3, 3, 2, 2},
                                        { 3, 1, 3, 3, 0, 2, 2, 3, 2, 0, 0, 1},
                                        { 1, 2, 1, 2, 0, 1, 0, 3, 0, 3, 3, 0},
                                        { 1, 1, 0, 0, 1, 0, 3, 2, 2, 1, 0, 2},
                                        { 0, 2, 0, 3, 3, 0, 2, 3, 1, 0, 3, 1},
                                        { 1, 3, 3, 2, 1, 1, 1, 0, 3, 1, 0, 0},
                                        { 1, 2, 1, 2, 3, 0, 3, 1, 1, 0, 1, 1},
                                        { 2, 2, 0, 1, 1, 0, 3, 0, 1, 3, 3, 2},
                                        { 2, 3, 3, 2, 0, 2, 3, 2, 2, 0, 1, 0} };

    private int[,] newMap2D = new int[,] { { 0, 1, 3, 2, 3, 2, 0, 2, 2, 3, 1, 2},
                                            { 3, 2, 3, 2, 1, 0, 1, 0, 3, 3, 2, 2},
                                            { 3, 1, 3, 3, 0, 2, 2, 3, 2, 0, 0, 1},
                                            { 1, 2, 1, 2, 0, 1, 0, 3, 0, 3, 3, 0},
                                            { 1, 1, 0, 0, 1, 0, 3, 2, 2, 1, 0, 2},
                                            { 0, 2, 0, 3, 3, 0, 2, 3, 1, 0, 3, 1},
                                            { 1, 3, 3, 2, 1, 1, 1, 0, 3, 1, 0, 0},
                                            { 1, 2, 1, 2, 3, 0, 3, 1, 1, 0, 1, 1},
                                            { 2, 2, 0, 1, 1, 0, 3, 0, 1, 3, 3, 2},
                                            { 2, 3, 3, 2, 0, 2, 3, 2, 2, 0, 1, 0} };

    private void Awake()
    {
        SimplePool.Preload(redBrick, 30, tf_redBrick);
        SimplePool.Preload(greenBrick, 30, tf_greenBrick);
        SimplePool.Preload(blueBrick, 30, tf_blueBrick);
        SimplePool.Preload(yellowBrick, 30, tf_yellowBrick);

        SimplePool.Preload(blueStack, 30, tf_blueStackHolder); ;
        SimplePool.Preload(redStack, 30, tf_redStackHolder); ;

        SimplePool.Preload(step, 30, tf_steps);
    }

    private void Start()
    {
        brickIndex = 0;
        InitBricks();

        numOfStepsPerSlide = 10;
        InitSteps();
    }

    private void InitBricks()
    {
        for (int i = map2D.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = 0; j < map2D.GetLength(1); j++)
            {
                switch (map2D[i, j])
                {
                    case 0:
                        StartCoroutine(spawnDelay(poolTransform, redBrick, i, j));
                        break;
                    case 1:
                        StartCoroutine(spawnDelay(poolTransform, greenBrick, i, j));
                        break;
                    case 2:
                        StartCoroutine(spawnDelay(poolTransform, blueBrick, i, j));
                        break;
                    case 3:
                        StartCoroutine(spawnDelay(poolTransform, yellowBrick, i, j));
                        break;
                }
            }
        }
    }

    private void InitSteps()
    {
        GameUnit newStep;
        Renderer newRenderer;
        for (int i = 0; i < numOfStepsPerSlide; i++)
        {
            newStep = SimplePool.Spawn(step, tf_steps_0.position + new Vector3(0, 0.05f + 0.1f * i, 0.15f + 0.3f * i), tf_steps.rotation);
            newRenderer = newStep.GetComponent<Renderer>();
            objRenDict.Add(newStep.gameObject, newRenderer);

            newStep = SimplePool.Spawn(step, tf_steps_1.position + new Vector3(0, 0.05f + 0.1f * i, 0.15f + 0.3f * i), tf_steps.rotation);
            newRenderer = newStep.GetComponent<Renderer>();
            objRenDict.Add(newStep.gameObject, newRenderer);

            newStep = SimplePool.Spawn(step, tf_steps_2.position + new Vector3(0, 0.05f + 0.1f * i, 0.15f + 0.3f * i), tf_steps.rotation);
            newRenderer = newStep.GetComponent<Renderer>();
            objRenDict.Add(newStep.gameObject, newRenderer);
        }
    }

    IEnumerator spawnDelay(Transform poolTf, Brick brick, int i, int j)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        SimplePool.Spawn(brick, poolTf.position + new Vector3(j * 0.18f, 0, i * 0.2f), poolTf.rotation);
    }

    private void LoadBrick(int[,] map, Transform poolTf, Brick brick)
    {
        SimplePool.CollectAPool(brick);
        for (int i = map2D.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map2D[i, j] == brickIndex)
                {
                    StartCoroutine(spawnDelay(poolTf, brick, i, j));

                }
            }
        }
        brickIndex++;
        if (brickIndex == 4) {
            brickIndex = 0;
        }
    }

    public void LoadStageOne(Brick brick)
    {
        LoadBrick(newMap2D, newPoolTransform, brick);
    }

    public Renderer GetMaterial(GameObject obj)
    {
        return objRenDict[obj];
    }
}
