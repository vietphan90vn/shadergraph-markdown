using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class HierarchyMarkdown
{
    private static Texture2D bgTexture;
    private static Texture2D lineTexture;
    private static Texture2D thinLineTexture;
    private static Color bgColor;
    private static float rightSideOffset;
    
    [InitializeOnLoadMethod]
    private static void Setup()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    private static void InitResources()
    {
        Color GetDefaultBackgroundColor()
        {
            var num = EditorGUIUtility.isProSkin ? 0.22f : 0.76f;
            return new Color(num, num, num, 1f);
        }
        
        Texture2D GetTexture(Color bgColor)
        {
            var bgTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            bgTexture.SetPixel(0, 0, bgColor);
            bgTexture.Apply();
            return bgTexture;
        }

        // EditorGUIUtility.GetDefaultBackgroundColor
        var method = typeof(EditorGUIUtility).GetMethod("GetDefaultBackgroundColor", (BindingFlags) (-1));
        if (method != null)
            bgColor = (Color) method.Invoke(null, null);
        else
            bgColor = GetDefaultBackgroundColor();

        var rightArrow = (GUIStyle) "ArrowNavigationRight";
        rightSideOffset = rightArrow != null ? rightArrow.fixedWidth + (float) rightArrow.margin.horizontal : 20;
            
        bgTexture = GetTexture(bgColor); 
        lineTexture = GetTexture(new Color(1, 1, 1, 0.1f));
        thinLineTexture = GetTexture(new Color(1, 1, 1, 0.05f));
    }

    private static void HierarchyWindowItemOnGUI(int instanceId, Rect selectionRect)
    {
        if (!bgTexture)
            InitResources();

        if (!bgTexture) {
            Debug.LogWarning(nameof(HierarchyMarkdown) + " not properly set up.");
            return;
        }

        if (instanceId == 0) return;
        var obj = EditorUtility.InstanceIDToObject(instanceId);
        if (!obj) return;
        if (Selection.Contains(instanceId)) return;
        
        // TODO how to check if line is hovered?
        // SceneHierarchyWindow.m_SceneHierarchy.treeView.hoveredItem
        // if (this.m_TreeView.hoveredItem != item || this.isDragging)

        bool isDisabled = false;
        if (obj is GameObject go && go && !go.activeInHierarchy)
        {
            isDisabled = true;
            EditorGUI.BeginDisabledGroup(true);
        }

        // correct for prefab arrow, we want to draw over that as well.
        selectionRect.xMax += rightSideOffset;

        if (obj.name.Equals("--", StringComparison.Ordinal) || obj.name.Equals("---", StringComparison.Ordinal))
        {
            GUI.DrawTexture(selectionRect, bgTexture);
        }
        else if(obj.name.Equals("----", StringComparison.Ordinal) || obj.name.Equals("-----", StringComparison.Ordinal)) 
        {
            GUI.DrawTexture(selectionRect, bgTexture);
            var r2 = selectionRect;
            r2.xMin = 32;
            r2.xMax = selectionRect.xMax + 300;
            r2.yMin += r2.height / 2;
            r2.height = 1;
            GUI.DrawTexture(r2, lineTexture);
        }
        else if (obj.name.StartsWith("# ", StringComparison.Ordinal))
        {
            // selectionRect.xMin = 32; // grey bar on the left
            // selectionRect.xMin -= 8; // slight shift to the left
            var displayText = obj.name.TrimStart(' ', '#');
            var content = new GUIContent(displayText);
            GUI.DrawTexture(selectionRect, bgTexture);

            if (string.IsNullOrEmpty(displayText)) return;

            var textRect = selectionRect;
            textRect.y -= 1;
            GUI.Label(textRect, content, EditorStyles.boldLabel);
            
            // left line
            var r2 = selectionRect;
            r2.xMin = 32;
            r2.xMax = selectionRect.xMin - 8;
            r2.yMin += r2.height / 2;
            r2.height = 1;
            GUI.DrawTexture(r2, thinLineTexture);
            
            // width
            var width = EditorStyles.boldLabel.CalcSize(content);
            
            // right line
            var r3 = r2;
            r3.xMin = selectionRect.xMin + width.x + 8;
            r3.xMax = selectionRect.xMin + width.x + 300;
            GUI.DrawTexture(r3, thinLineTexture);

            // GUI.DrawTexture(new Rect(selectionRect.xMin, selectionRect.yMin, width.x, width.y), Texture2D.whiteTexture);
        }
        else if (obj.name.StartsWith("!DRAWER"))
        {
            if (obj is GameObject go2)
            {
                var drawer = go2.GetComponent<HierarchyMarkdownDrawer>();
                if (drawer && drawer.enabled)
                {
                    GUI.DrawTexture(selectionRect, bgTexture);
                    drawer.Draw(selectionRect, instanceId, obj);
                }
            }
        }
        
        if(isDisabled)
            EditorGUI.EndDisabledGroup();
    }
}
