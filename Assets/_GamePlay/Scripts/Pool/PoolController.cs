using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolController : MonoBehaviour
{
    [Header("Map Variables")]
    private List<int[,]> mapStagesMatrix = new List<int[,]>();
    public List<Transform> mapStartPoints;

    public int mapSizeZ = 10;
    public int mapSizeX = 12;


    [Header("Brick Variables")]
    public int totalNumOfBricks = 120;
    public int numOfEachBrickColor = 30;

    public List<Brick> brickPrefabs;
    public List<Transform> brickHolderTransforms;

    public Brick normalBrick;
    public Transform normalBrickHolder;


    [Header("Stack Variables")]
    public List<Transform> stackHolderTransforms;
    public List<Stack> stackPrefabs;


    [Header("Step Variables")]
    public Step step;
    public List<Transform> stepHolders;
    public List<int> numOfStepInSlices;


    [Header("Support Variables")]
    private List<int> currentBrickIndex = new List<int>();
    private List<int> sampleMap = new List<int>();
    private Dictionary<GameObject, Renderer> objRenDict = new Dictionary<GameObject, Renderer>();


    private void Awake()
    {
        for (int i = 0; i < mapStartPoints.Count; i++)
        {
            mapStagesMatrix.Add(new int[mapSizeZ, mapSizeX]);
            mapStagesMatrix[i] = InitAMapMatrix(mapStagesMatrix[i]);
            currentBrickIndex.Add(0);
        }

        for (int i = 0; i < brickPrefabs.Count; i++)
        {
            SimplePool.Preload(brickPrefabs[i], numOfEachBrickColor, brickHolderTransforms[i]);
        }

        SimplePool.Preload(normalBrick, numOfEachBrickColor * 2, normalBrickHolder);

        for (int i = 0; i < stackPrefabs.Count; i++)
        {
            SimplePool.Preload(stackPrefabs[i], numOfEachBrickColor * 2, stackHolderTransforms[i]);
        }

    }

    private void Start()
    {
        for (int i = 0; i < brickPrefabs.Count; i++)
        {
            LoadABrickInMap(0, brickPrefabs[i]);
        }
        InitSteps();
    }

    private void InitSampleMapList()
    {
        for (int i = 0; i < brickPrefabs.Count; i++)
        {
            for (int j = 0; j < numOfEachBrickColor; j++)
            {
                sampleMap.Add(i);
            }
        }
    }
    
    private int[,] InitAMapMatrix(int[,] map2D)
    {
        InitSampleMapList();
        List<int> newList = sampleMap;
        for (int i = map2D.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = 0; j < map2D.GetLength(1); j++)
            {
                int index = Random.Range(0, newList.Count - 1);
                map2D[i, j] = newList[index];
                newList.RemoveAt(index);
            }
        }
        return map2D;
    }

    private void InitSteps()
    {
        GameUnit newStep;
        Renderer newRenderer;
        for (int i = 0; i < stepHolders.Count; i++)
        {
            for (int j = 0; j < numOfStepInSlices[i]; j++)
            {
                newStep = SimplePool.Spawn(step, stepHolders[i].position + new Vector3(0, 0.05f + 0.1f * j, 0.15f + 0.3f * j), stepHolders[i].rotation);
                newRenderer = newStep.GetComponent<Renderer>();
                objRenDict.Add(newStep.gameObject, newRenderer);
            }
        }
    }

    IEnumerator spawnDelay(Transform poolTf, Brick brick, int i, int j)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        SimplePool.Spawn(brick, poolTf.position + new Vector3(j * 0.3f, 0, i * 0.4f), poolTf.rotation);
    }

    public void LoadABrickInMap(int stageIndex, Brick brick)
    {
        SimplePool.CollectAPool(brick);
        for (int i = mapStagesMatrix[stageIndex].GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = 0; j < mapStagesMatrix[stageIndex].GetLength(1); j++)
            {
                if (mapStagesMatrix[stageIndex][i, j] == currentBrickIndex[stageIndex])
                {
                    StartCoroutine(spawnDelay(mapStartPoints[stageIndex], brick, i, j));
                }
            }
        }
        currentBrickIndex[stageIndex]++;
        if (currentBrickIndex[stageIndex] == brickPrefabs.Count) {
            currentBrickIndex[stageIndex] = 0;
        }
    }

    public Renderer GetRenderer(GameObject obj)
    {
        return objRenDict[obj];
    }

    public string GetMatString(GameObject obj)
    {
        return objRenDict[obj].sharedMaterial.name;
    }
}
