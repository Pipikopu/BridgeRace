using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton_Example<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T ins;

    public static T Ins
    {
        get
        {
            if (ins == null)
            {
                ins = FindObjectOfType<T>();
            }

            if (ins == null)
            {
                GameObject go = new GameObject();
                ins = go.AddComponent<T>();
            }

            return ins;
        }
    }
}
