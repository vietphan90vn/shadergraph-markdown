using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HierarchyPropertyDrawer : HierarchyMarkdownDrawer
{
    public Component component;
    public string propertyName;
    
#if UNITY_EDITOR
    public override void Draw(Rect selectionRect, int instanceId, Object o)
    {
        if (!component || string.IsNullOrEmpty(propertyName))
        {
            var c = GUI.color;
            GUI.color = Color.red;
            
            if(!component)
            {
                var newComponent = (Component) EditorGUI.ObjectField(selectionRect, o.name, component, typeof(Component), true);
                if (component != newComponent)
                {
                    Undo.RegisterCompleteObjectUndo(this, $"Set {nameof(HierarchyProperty)} Component");
                    component = newComponent;
                }
            }
            else if (string.IsNullOrEmpty(propertyName))
            {
                var space = EditorGUI.PrefixLabel(selectionRect, new GUIContent(o.name + ":" + component.name));
                if (EditorGUI.DropdownButton(space, new GUIContent("Select Property"), FocusType.Passive))
                    HierarchyPropertyDrawerEditor.SelectProperty(this);
            }
            GUI.color = c;
            return;
        }
        
        if(so == null || so.targetObject != component)
            so = new SerializedObject(component);
        else
            so.Update();
        
        if(property == null || property.name != propertyName)
            property = so.FindProperty(propertyName);

        if (property == null)
        {
            EditorGUI.LabelField(selectionRect, o.name + " [Property not found, can't draw]", EditorStyles.miniLabel);
        }
        else
        {
            selectionRect.xMax -= 4;
            EditorGUI.PropertyField(selectionRect, property, null, false);
            so.ApplyModifiedProperties();
        }
    }

    private SerializedObject so;
    private SerializedProperty property;
#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(HierarchyPropertyDrawer))]
public class HierarchyPropertyDrawerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (EditorGUILayout.DropdownButton(new GUIContent("Select Property"), FocusType.Passive))
            SelectProperty(target as HierarchyPropertyDrawer);
        EditorGUILayout.EndHorizontal();
    }

    internal static void SelectProperty(HierarchyPropertyDrawer propertyDrawer)
    {
        if (!propertyDrawer || !propertyDrawer.component) return;
        
        void SetPropertyName(object custom)
        {
            if (custom is string prop)
            {
                propertyDrawer.propertyName = prop;
            }
        }
                
        var menu = new GenericMenu();
        
        // get all properties
        var componentSerializedObject = new SerializedObject(propertyDrawer.component);
        var it = componentSerializedObject.GetIterator();
        it.Next(true);
        do
        {
            menu.AddItem(new GUIContent($"{(it.propertyType != SerializedPropertyType.ManagedReference ? it.propertyType.ToString() : it.managedReferenceFieldTypename)}/{it.displayName} [{it.name}]"), false, SetPropertyName, it.name);
        } while (it.Next(false));
        
        menu.ShowAsContext();
    }
}

#endif
