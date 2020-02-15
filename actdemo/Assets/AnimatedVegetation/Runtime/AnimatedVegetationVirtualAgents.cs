using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatedVegetationVirtualAgents
{
    private List<Transform> workingList = null;

    private List<Transform> freeList = null;

    private Transform root = null;

    public AnimatedVegetationVirtualAgents()
    {
        workingList = new List<Transform>();
        freeList = new List<Transform>();

        GameObject rootGo = new GameObject("AnimatedVegetationVirtualAgentsRoot");
        root = rootGo.transform;
        root.parent = null;
        root.localPosition = Vector3.zero;
        root.localEulerAngles = Vector3.zero;
        root.localScale = Vector3.one;
    }

    public Transform Get()
    {
        if (freeList == null || workingList == null || root == null)
        {
            return null;
        }

        Transform agent = null;
        if (freeList.Count > 0)
        {
            agent = freeList[freeList.Count - 1];
            freeList.RemoveAt(freeList.Count - 1);
        }
        else
        {
            GameObject go = new GameObject();
            agent = go.transform;
            agent.parent = root;
            agent.localPosition = Vector3.zero;
            agent.localEulerAngles = Vector3.zero;
            agent.localScale = Vector3.one;
        }

//#if UNITY_EDITOR
//        if (agent != null && agent.gameObject != null)
//        {
//            agent.gameObject.name = "Working";
//        }
//#endif

        workingList.Add(agent);

        return agent;
    }

    public void Release(Transform agent)
    {
        if (freeList == null || workingList == null || root == null || agent == null)
        {
            return;
        }

        int index = workingList.IndexOf(agent);
        if (index == -1)
        {
            return;
        }

//#if UNITY_EDITOR
//        if (agent != null && agent.gameObject != null)
//        {
//            agent.gameObject.name = "Free";
//        }
//#endif

        workingList.RemoveAt(index);
        freeList.Add(agent);
    }

    public void Destroy()
    {
        DestroyList(workingList);
        workingList = null;
        DestroyList(freeList);
        freeList = null;
        DestroyRoot();
    }

    private void DestroyRoot()
    {
        if (root != null && root.gameObject != null)
        {
            GameObject.Destroy(root.gameObject);
            root = null;
        }
    }

    private void DestroyList(List<Transform> list)
    {
        if (list == null)
        {
            return;
        }

        int numItems = list.Count;
        for (int i = 0; i < numItems; ++i)
        {
            Transform item = list[i];
            if (item != null && item.gameObject != null)
            {
                GameObject.Destroy(item.gameObject);
            }
        }
        list.Clear();
    }
}
