using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

public class HierarchyProfilerDrawer : HierarchyMarkdownDrawer
{
    public string customSamplerName = "MyCustomSampler";
    Recorder behaviourUpdateRecorder;

    public override void Draw(Rect selectionRect, int instanceId, Object o)
    {
        // if(behaviourUpdateRecorder == null)
        // {
        //     behaviourUpdateRecorder = Recorder.Get(customSamplerName);
        //     behaviourUpdateRecorder.enabled = true;
        // }
        //
        // if (behaviourUpdateRecorder.isValid)
        //     EditorGUI.DoubleField(selectionRect, customSamplerName + " ms", behaviourUpdateRecorder.elapsedNanoseconds / 1000000.0);
        // else
        // {
        //     var c = GUI.color;
        //     GUI.color = Color.red;
        //     EditorGUI.LabelField(selectionRect, "Invalid Sampler");
        //     GUI.color = c;
        // }
        
        EditorGUI.DoubleField(selectionRect, customSamplerName + " ms", GetTime("MyProfileTest"));

        EditorApplication.delayCall += EditorApplication.RepaintHierarchyWindow;
    }

    [ContextMenu("Test Profiler Sample Log")]
    void Log()
    {
        Debug.Log(GetTime("MyProfileTest"));
    }

    double GetTime(string name)
    {
        var view = UnityEditorInternal.ProfilerDriver.GetRawFrameDataView(UnityEditorInternal.ProfilerDriver.lastFrameIndex, 0);
        if(view == null) return -1;
        if (!view.valid) return -1;
        if(view.sampleCount < 1) return -1;
        
        for (int i = 0; i < view.sampleCount; i++)
        {
            var sname = view.GetSampleName(i);
            if (sname.Contains("MyProfileTest"))
            {
                // view.ResolveMethodInfo();
                // view.GetSampleCallstack(i, );
                return view.GetSampleTimeNs(i) / 1000000.0;
            }
        }

        return -1;
    }
}
