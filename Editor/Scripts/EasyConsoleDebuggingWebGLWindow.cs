using UnityEditor;
using UnityEngine;

public class EasyConsoleDebuggingWebGLWindow : EditorWindow
{
    private bool _isNeedToDebugLog;
    private bool _isNeedToDebugError;
    private bool _isNeedToDebugWarn;

    [MenuItem("Tools/Easy Console Debugging WebGL")]
    public static void CreateWindow()
    {
        EditorWindow window = GetWindow<EasyConsoleDebuggingWebGLWindow>();
        window.titleContent = new GUIContent("Easy Console Debugging WebGL");
    }

    private void OnEnable()
    {
        _isNeedToDebugLog = EditorPrefs.GetBool("IsNeedToDebugLog", false);
        _isNeedToDebugError = EditorPrefs.GetBool("IsNeedToDebugError", false);
        _isNeedToDebugWarn = EditorPrefs.GetBool("IsNeedToDebugWarn", false);
    }

    private void OnDisable()
    {
        EditorPrefs.SetBool("IsNeedToDebugLog", _isNeedToDebugLog);
        EditorPrefs.SetBool("IsNeedToDebugError", _isNeedToDebugError);
        EditorPrefs.SetBool("IsNeedToDebugWarn", _isNeedToDebugWarn);
    }

    private void OnGUI()
    {
        _isNeedToDebugLog = EditorGUILayout.Toggle("Is need to debug log", _isNeedToDebugLog);
        _isNeedToDebugError = EditorGUILayout.Toggle("Is need to debug error", _isNeedToDebugError);
        _isNeedToDebugWarn = EditorGUILayout.Toggle("Is need to debug warn", _isNeedToDebugWarn);
    }
}
