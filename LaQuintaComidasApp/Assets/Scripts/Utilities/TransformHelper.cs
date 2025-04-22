using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransformHelper
{
    public static void DeleteAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    public static Transform GetFirstChild(Transform parent)
    {
        if (parent.childCount > 0)
        {
            return parent.GetChild(0);
        }
        return null;
    }

    public static List<Transform> GetAllChildTransforms(Transform parent)
    {
        List<Transform> childTransforms = new List<Transform>();

        foreach (Transform child in parent)
        {
            childTransforms.Add(child);
        }

        return childTransforms;
    }
}
