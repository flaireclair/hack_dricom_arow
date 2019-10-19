using UnityEditor;
using UnityEngine;
using ArowMain.Runtime;

namespace ArowMain.Public.Scripts.Editor.LocalFileDeliveryServer
{
public class LaunchServerConfigEditor : EditorWindow
{

    [MenuItem(LaunchServer.kLocalServerMenu + "/Open Config", false, MenuItemProperty.ArowLocalServerOpenConfigGroup)]
    static void Open()
    {
        var configEditor = GetWindow<LaunchServerConfigEditor>();
        configEditor.port = EditorPrefs.GetInt(LaunchServer.kEditorPrefsLocalServerPortKey, 18080);
        configEditor.path = EditorPrefs.GetString(LaunchServer.kEditorPrefsLocalServerRootPathKey, "Assets/StreamingAssets");
    }

    private int port;
    private string path;

    void OnGUI()
    {
        port = EditorGUILayout.IntField("Port", port);
        path = EditorGUILayout.TextField("Server Root Path", path);

        if (GUILayout.Button("Select Server Root Path"))
        {
            SelectPath();
        }

        if (GUILayout.Button("Save"))
        {
            EditorPrefs.SetInt(LaunchServer.kEditorPrefsLocalServerPortKey, port);
            EditorPrefs.SetString(LaunchServer.kEditorPrefsLocalServerRootPathKey, path);
        }
    }

    void SelectPath()
    {
        path = EditorUtility.OpenFolderPanel("Select Server Root Path", "", "");

        if (path.Length != 0)
        {
            Debug.Log(path);
        }
    }
}
}