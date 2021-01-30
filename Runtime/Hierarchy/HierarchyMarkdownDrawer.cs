using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HierarchyMarkdownDrawer : MonoBehaviour
{
#if UNITY_EDITOR
    public abstract void Draw(Rect selectionRect, int instanceId, Object o);
#endif
}
