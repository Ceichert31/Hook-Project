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
    private LineRenderer ropeRenderer;

    private void Start()
    {
        hookHolder = transform.GetChild(0);
        ropeRenderer = hookHolder.GetComponent<LineRenderer>();
        ropeRenderer.positionCount = maxHookCount;

        //Populate hook pool
        for (int i = 0; i < maxHookCount; ++i)
        {
            //Add hook
            hookPool.Add(Instantiate(hookPrefab, hookHolder));

            //Add hook to rope render
            ropeRenderer.SetPosition(i, hookPool[i].transform.position);
            
            //Disable hook
            hookPool[i].SetActive(false);
        }
    }

    private void Update()
    {
        //Update activated hooks rope positions
        for (int i = 0; i < maxHookCount; ++i)
        {
            //Reset disable hooks rope
            if (!hookPool[i].activeSelf)
            {
                ropeRenderer.SetPosition(i, Vector3.zero);
                return;
            }

            ropeRenderer.SetPosition(i, hookPool[i].transform.position);
        }
    }

    /// <summary>
    /// Finds an available hook and returns it
    /// </summary>
    /// <returns></returns>
    public GameObject GetHook()
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
