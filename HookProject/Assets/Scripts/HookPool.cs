using System.Collections.Generic;
using UnityEngine;

public class HookPool : MonoBehaviour
{
    [SerializeField]
    private GameObject hookPrefab;

    [Header("Pool Settings")]
    [SerializeField]
    private int maxHookCount;

    private List<GameObject> hookPool = new();

    private Transform hookHolder;

    private void Start()
    {
        hookHolder = transform.GetChild(0);

        //Populate hook pool
        for (int i = 0; i < maxHookCount; ++i)
        {
            //Add hook
            hookPool.Add(Instantiate(hookPrefab, hookHolder));

            //Disable hook
            hookPool[i].SetActive(false);
        }
    }
    /// <summary>
    /// Finds an available hook and returns it
    /// </summary>
    /// <returns></returns>
    public GameObject GetInstance()
    {
        //Find an available hook
        for (int i = 0; i < maxHookCount; ++i)
        {
            if (!hookPool[i].activeSelf)
            {
                hookPool[i].SetActive(true);
                return hookPool[i];
            }
        }

        //If no hooks are available, use oldest
        return hookPool[0];
    }

    /// <summary>
    /// Gets the oldest active instance, if there are no active instances returns null
    /// </summary>
    /// <returns></returns>
    public GameObject GetOldestInstance()
    {
        for (int i = 0; i < maxHookCount; ++i)
        {
            //Return first active instance
            if (hookPool[i].activeSelf)
            {
                return hookPool[i];
            }
        }
        return null;
    }

    public void DisableInstance(GameObject instance)
    {
        //Search for instance and disable it

    }

    /// <summary>
    /// Disables all hook instances
    /// </summary>
    public void DisableAllInstances()
    {
        //Find an available hook
        for (int i = 0; i < maxHookCount; ++i)
        {
            if (hookPool[i].activeSelf)
            {
                hookPool[i].SetActive(false);
            }
        }
    }
}