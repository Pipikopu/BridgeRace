using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimplePool
{
    static int DEFAULT_AMOUNT = 10;
    static Dictionary<GameUnit, Pool> poolObjects = new Dictionary<GameUnit, Pool>();
    static Dictionary<GameUnit, Pool> poolParents = new Dictionary<GameUnit, Pool>();

    public static void Preload(GameUnit prefab, int amount, Transform parent)
    {
        if (!poolObjects.ContainsKey(prefab))
        {
            poolObjects.Add(prefab, new Pool(prefab, amount, parent));
        }
    }

    public static GameUnit Spawn(GameUnit prefab, Vector3 position, Quaternion rotation)
    {
        GameUnit obj = null;

        if (!poolObjects.ContainsKey(prefab) || poolObjects[prefab] == null)
        {
            poolObjects.Add(prefab, new Pool(prefab, DEFAULT_AMOUNT, null));
        }

        obj = poolObjects[prefab].Spawn(position, rotation);

        return obj;
    }

    public static GameUnit SpawnWithParent(GameUnit prefab, Vector3 position, Quaternion rotation, Transform newParent)
    {
        GameUnit obj = null;

        if (!poolObjects.ContainsKey(prefab) || poolObjects[prefab] == null)
        {
            poolObjects.Add(prefab, new Pool(prefab, DEFAULT_AMOUNT, null));
        }

        obj = poolObjects[prefab].SpawnWithParent(position, rotation, newParent);

        return obj;
    }

    public static void SpawnOldest(GameUnit prefab)
    {
        if (!poolObjects.ContainsKey(prefab) || poolObjects[prefab] == null)
        {
            return;
        }
        poolObjects[prefab].SpawnOldest();
    }

    public static void Despawn(GameUnit obj)
    {
        if (poolParents.ContainsKey(obj))
        {
            poolParents[obj].Despawn(obj);
        }
        else
        {
            GameObject.Destroy(obj);
        }
    }

    public static GameUnit DespawnOldest(GameUnit prefab)
    {
        if (poolObjects.ContainsKey(prefab))
        {
            return poolObjects[prefab].DespawnOldest();
        }
        else
        {
            return null;
        }
    }

    public static GameUnit DespawnNewest(GameUnit prefab)
    {
        if (poolObjects.ContainsKey(prefab))
        {
            return poolObjects[prefab].DespawnNewest();
        }
        else
        {
            return null;
        }
    }

    public static void CollectAPool(GameUnit prefab)
    {
        poolObjects[prefab].Collect();
    }

    public static void CollectAll()
    {
        foreach (var item in poolObjects)
        {
            item.Value.Collect();
        }
    }

    public static void ReleaseAll()
    {
        foreach (var item in poolObjects)
        {
            item.Value.Release();
        }
    }

    public static Vector3 GetFirstAcObjPos(GameUnit prefab, Vector3 defaultPosition)
    {
        return poolObjects[prefab].GetFirstAcObjPos(defaultPosition);
    }

    public class Pool
    {
        Queue<GameUnit> pools = new Queue<GameUnit>();
        List<GameUnit> activeObjs = new List<GameUnit>();

        Transform parent;
        GameUnit prefab;

        public Pool(GameUnit prefab, int amount, Transform parent)
        {
            this.prefab = prefab;

            for (int i = 0; i < amount; i++)
            {
                GameUnit obj = GameObject.Instantiate(prefab, parent);
                poolParents.Add(obj, this);
                pools.Enqueue(obj);
                obj.gameObject.SetActive(false);
            }
        }

        public GameUnit Spawn(Vector3 position, Quaternion rotation)
        {
            GameUnit obj = null;

            if (pools.Count == 0)
            {
                obj = GameObject.Instantiate(prefab, parent);
                poolParents.Add(obj, this);
            }
            else
            {
                obj = pools.Dequeue();
            }

            obj.transform.SetPositionAndRotation(position, rotation);
            new WaitForSeconds(Random.Range(1f, 5f));
            obj.gameObject.SetActive(true);

            activeObjs.Add(obj);
            return obj;
        }

        public GameUnit SpawnWithParent(Vector3 position, Quaternion rotation, Transform newParent)
        {
            GameUnit obj = null;

            if (pools.Count == 0)
            {
                obj = GameObject.Instantiate(prefab, newParent);
                poolParents.Add(obj, this);
            }
            else
            {
                obj = pools.Dequeue();
            }

            obj.transform.SetPositionAndRotation(position, rotation);
            new WaitForSeconds(Random.Range(1f, 5f));
            obj.gameObject.SetActive(true);

            activeObjs.Add(obj);
            return obj;
        }

        public void SpawnOldest()
        {
            if (pools.Count > 0)
            {
                GameUnit obj = pools.Dequeue();
                obj.gameObject.SetActive(true);
                activeObjs.Add(obj);
            }
        }

        public void Despawn(GameUnit obj)
        {
            activeObjs.Remove(obj);
            pools.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }

        public GameUnit DespawnOldest()
        {
            if (activeObjs.Count > 0)
            {
                GameUnit obj = activeObjs[0];
                activeObjs.RemoveAt(0);
                pools.Enqueue(obj);
                obj.gameObject.SetActive(false);
                return obj;
            }
            else
            {
                return null;
            }
        }

        public GameUnit DespawnNewest()
        {
            if (activeObjs.Count > 0)
            {
                GameUnit obj = activeObjs[activeObjs.Count - 1];
                activeObjs.RemoveAt(activeObjs.Count - 1);
                pools.Enqueue(obj);
                obj.gameObject.SetActive(false);
                return obj;
            }
            else
            {
                return null;
            }
        }

        public void Collect()
        {
            while (activeObjs.Count > 0)
            {
                Despawn(activeObjs[0]);
            }
        }

        public void Release()
        {
            Collect();

            while (pools.Count > 0)
            {
                GameUnit obj = pools.Dequeue();
                GameObject.Destroy(obj);
            }
        }

        public Vector3 GetFirstAcObjPos(Vector3 defaultPosition)
        {
            if (activeObjs.Count > 0)
            {
                return activeObjs[0].transform.position;
            }
            else
            {
                return defaultPosition;
            }
        }
    }
}

public class GameUnit: MonoBehaviour
{
    private Transform tf;

    public Transform Transform
    {
        get
        {
            if (this.tf == null)
            {
                this.tf = transform;
            }

            return tf;
        }
    }
}
