using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Profiling;
#endif
using UnityEngine;
using UnityEngine.Events;

public class HierarchyButtonDrawer : HierarchyMarkdownDrawer
{
    [System.Serializable]
    public class Event
    {
        public string name;
        public UnityEvent evt;
    }

    public List<Event> events;

#if UNITY_EDITOR
    public override void Draw(Rect selectionRect, int instanceId, Object o)
    {
        if (events == null || events.Count < 1)
        {
            EditorGUI.LabelField(selectionRect, "No events set on Drawer", EditorStyles.miniLabel);
            return;
        }
        
        for (int i = 0; i < events.Count; i++)
        {
            var content = new GUIContent(events[i].name);
            var size = EditorStyles.miniButton.CalcSize(content);
            selectionRect.xMax = selectionRect.xMin + size.x;
            EditorGUI.BeginDisabledGroup(!((GameObject) o).activeInHierarchy);
            if (GUI.Button(selectionRect, content, EditorStyles.miniButton)) {
                events[i].evt.Invoke();
            }

            selectionRect.x += selectionRect.width;
            EditorGUI.EndDisabledGroup();
        }
    }
#endif
}
