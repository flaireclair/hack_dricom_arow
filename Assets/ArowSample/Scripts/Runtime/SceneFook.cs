using System;
using System.Collections.Generic;
using ArowMain.Public.Scripts.Runtime;
using ArowMain.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ArowSample.Scripts.Runtime
{
public class SceneFook : MonoBehaviour
{
    private GameObject _content;
    private GameObject Content
    {
        get
        {
            if (_content != null)
            {
                return _content;
            }

            _content = GameObject.Find("Content");

            if (_content == null)
            {
                Debug.LogError("Scroll View 以下の Content 取得に失敗しています。");
                return null;
            }

            return Content;
        }
    }

    private void Start()
    {
        CreateButtonToJumpScene(ArowSceneManager.TraceRoadSceneName);
        CreateButtonToJumpScene(ArowSceneManager.PlayerControlSceneName);
        CreateButtonToJumpScene(ArowSceneManager.GpsSceneName);
        CreateInitialButtons();
    }

    void CreateInitialButtons()
    {
        CreateButton("ArowMap", delegate(string text)
        {
            ClearButtons();
            string serverUrl = ArowSampleGame.SampleScripts.ArowURLDefine.DEFAULT_SERVER_URL;
            var unityWebRequest = UnityWebRequest.Get(serverUrl + "filelist.json");
            RequestManager.SetWebRequest(unityWebRequest, (www) =>
            {
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    var fileList = FileListDownloadUtils.GetParsedFileList(www.downloadHandler.text);
                    CreateArowMapSelectButtons(fileList);
                }
            });
        });
    }
    void CreateButtonToJumpScene(string button_text)
    {
        CreateButton(button_text, delegate(string text)
        {
            ArowSceneManager.ChangeScene(button_text);
        });
    }

    void ClearButtons()
    {
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Destroy(Content.transform.GetChild(i).gameObject);
        }
    }

    void CreateButton(string text, Action<string> action)
    {
        var prefab = Resources.Load("UI/Button");
        string viewName = text;
        string fileName = text;
        var buttonInstance = (GameObject)Instantiate(prefab);
        buttonInstance.transform.SetParent(Content.transform);
        var textComponent = buttonInstance.GetComponentInChildren<Text>();
        textComponent.text = viewName;
        textComponent.alignment = TextAnchor.MiddleLeft;
        var button = buttonInstance.GetComponentInChildren<Button>();
        button.onClick.AddListener(new UnityAction(delegate
        {
            action(fileName);
        }));
    }

    void CreateArowMapSelectButtons(List<string> fileList)
    {
        for (int i = 0; i < fileList.Count; i++)
        {
            CreateButton(fileList[i], delegate(string fileName)
            {
                GoTo(fileName);
            });
        }

        CreateButton("back", delegate(string text)
        {
            ClearButtons();
            CreateInitialButtons();
        });
    }

    void GoTo(string fileName)
    {
        Debug.Log(fileName);
        ArowSceneManager.FileName = fileName;
        ArowSceneManager.ChangeScene(ArowSceneManager.WalkSceneName);
    }
}

public static class ArowSceneManager
{
    public static readonly string WalkSceneName = "WalkInCreatedMap";
    public static readonly string StartSceneName = "_StartScene";
    public static readonly string TraceRoadSceneName = "Scene_WalkInCreatedMap";
    public static readonly string PlayerControlSceneName = "Scene_MoveControlCreatedMap";
    public static readonly string GpsSceneName = "Scene_GpsPlayer";

    public static string FileName
    {
        get;
        set;
    } = "";

    public static void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
}
