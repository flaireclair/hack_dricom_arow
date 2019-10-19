using System;
using System.Collections.Generic;
using System.IO;
using ArowMain.Editor;
using ArowMain.Public.Scripts.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ArowSample.Scripts.Editor
{
public class EditorDownloadArowMap : EditorWindow
{
    [MenuItem("ArowSample/Download ArowMap", false, MenuItemProperty.ArowSampleDLGroup)]
    static void Open()
    {
        var exampleWindow = GetWindow<EditorDownloadArowMap>();
        exampleWindow.Show();
    }

    private const string DefaultServerUrl = "http://localhost:18080/";

    public string serverUrl = DefaultServerUrl;
    private bool isConnecting = false;
    private List<string> fileList = new List<string>();

    private void OnGUI()
    {
        GUI.enabled = true;
        EditorGUILayout.LabelField("serverUrl");
        serverUrl = EditorGUILayout.TextField(serverUrl);
        GUI.enabled = !isConnecting;

        if (GUILayout.Button("Download filelist"))
        {
            var unityWebRequest = UnityWebRequest.Get(serverUrl + "filelist.json");
            RequestManager.SetWebRequest(unityWebRequest, (www) =>
            {
                if (!string.IsNullOrEmpty(www.error))
                {
                    EditorUtility.DisplayDialog("Download json Error", www.error, "OK", "");
                }
                else
                {
                    try
                    {
                        fileList = FileListDownloadUtils.GetParsedFileList(www.downloadHandler.text);
                    }
                    catch (Exception e)
                    {
                        EditorUtility.DisplayDialog("Download json parse faild", www.downloadHandler.text + "\n" + e, "OK", "");
                    }

                    isConnecting = false;
                }
            });
            isConnecting = true;
        }

        GUI.enabled = true;

        foreach (var filename in fileList)
        {
            if (GUILayout.Button("Download " + filename))
            {
                var unityWebRequest = UnityWebRequest.Get(serverUrl + filename);
                RequestManager.SetWebRequest(unityWebRequest, (www) =>
                {
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        EditorUtility.DisplayDialog("Download json Error", www.error, "OK", "");
                    }
                    else
                    {
                        Directory.CreateDirectory("Assets/StreamingAssets");
                        var outputPath = Path.Combine("Assets/StreamingAssets", filename);
                        File.WriteAllBytes(outputPath, www.downloadHandler.data);
                        Debug.Log(outputPath + " をダウンロードして保存しました.");
                        AssetDatabase.Refresh();
                    }
                });
            }
        }
    }
}
}
