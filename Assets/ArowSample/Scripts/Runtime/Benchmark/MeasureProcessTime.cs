using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ArowSample.Scripts.Runtime
{

// 出力する結果
[Serializable]
public class JsonMeasureResult
{
    public List<JsonMeasureLog> list;
}

[Serializable]
public class JsonMeasureLog
{
    public string Date;
    public List<JsonMeasureProcess> Logs;

}

// 	「道」「川」等を分ける構造
[Serializable]
public class JsonMeasureProcess
{
    public string KeyName;
    public List<JsonProcessTimeLog> TimeLogs;
}

// 各々の時間
[Serializable]
public class JsonProcessTimeLog
{
    public string ProcessName;
    public long ms;
}


public class MeasureProcessTime
{
    public enum Key
    {
        None,
        Road,
        WaterWay,
        Building,
        WaterArea,
        Ground,
        Prefab,
        Unknown,
    }


    static string folderPath
    {
        get
        {
#if UNITY_EDITOR
            return "ProcessMeasureLog";
#else
            return Path.Combine(Application.temporaryCachePath, "ProcessMeasureLog");
#endif
        }
    }
    // 出力ファイルの拡張子
    static string fileExtension = ".json";

    static string fileNamePath = Path.Combine("", "ProcessMeasureLog", "NoDate.json");

    // TODO:ストップウォッチの複数化対応
    static System.Diagnostics.Stopwatch sw = null;

    [SerializeField]
    static public JsonMeasureResult result = new JsonMeasureResult();
    static int result_index = -1;

    // 計測中のプロセス情報
    static string MeasureKey = Key.None.ToString();
    static string MeasureFuncName = "";

    static DirectoryInfo SafeCreateDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            return null;
        }

        return Directory.CreateDirectory(path);
    }

    static void InitJsonMeasureResult()
    {
        JsonMeasureLog j = new JsonMeasureLog();
        j.Logs = new List<JsonMeasureProcess>();
        j.Date = DateTime.Now.ToString("yyyy_MM_dd_HHmm");
        result.list.Add(j);
        result_index++;
    }

    static int SeekResultJson(string key_name)
    {
        if (result == null)
        {
            Debug.LogError("EditorMeasureProcessTime:resultのInit忘れ");
            Debug.Assert(false);
            return -1;
        }

        // 既にあるかどうか戻す
        for (int i = 0; i < result.list[result_index].Logs.Count; i++)
        {
            if (result.list[result_index].Logs[i].KeyName.Equals(key_name))
            {
                return i;
            }
        }

        // なければ項目を追加
        JsonMeasureProcess p = new JsonMeasureProcess();
        p.KeyName = key_name;
        result.list[result_index].Logs.Add(p);
        return result.list[result_index].Logs.Count - 1;
    }

    static void SaveLog(string k, string func_name)
    {
#if UNITY_EDITOR
        Debug.Log(func_name + "[" + sw.ElapsedMilliseconds + "(ms)]");
        int list_index = SeekResultJson(k);
        JsonProcessTimeLog t = new JsonProcessTimeLog();
        t.ProcessName = func_name;
        t.ms = sw.ElapsedMilliseconds;

        if (result.list[result_index].Logs[list_index].TimeLogs == null)
        {
            result.list[result_index].Logs[list_index].TimeLogs = new List<JsonProcessTimeLog>();
        }

        result.list[result_index].Logs[list_index].TimeLogs.Add(t);
#endif
    }


    /// <summary>
    /// Unityのアプリ実行時に呼び出しておく（デバッグ用）
    /// </summary>
    static public void Setup()
    {
        if (result.list == null)
        {
            // ファイル名を日付に
            DateTime dt = DateTime.Now;
            string d = dt.ToString("yyyy_MM_dd_HHmm");
            fileNamePath = Path.Combine(folderPath, d + fileExtension);
            result.list = new List<JsonMeasureLog>();
            // フォルダ生成
            SafeCreateDirectory(folderPath);
        }

        InitJsonMeasureResult();
        Debug.Log("処理時間計測可能");
    }

    static public void Start(Key k, string func_name)
    {
        if (result.list == null)
        {
            Debug.Log("処理時間保存していない");
        }
        else
        {
            if (k == Key.None)
            {
                Debug.Assert(false);
            }

            // プロセス情報保管
            if (k == Key.Unknown)
            {
                // 分からない場合は前回のキー　プラス　(Unknown)
                MeasureKey =  MeasureKey + "(Unknown)";
            }
            else
            {
                MeasureKey =  k.ToString();
            }

            MeasureFuncName = func_name;
        }

        if (sw == null)
        {
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();
        }
        else
        {
            sw.Reset();
            sw.Start();
        }
    }

    static public void Stop()
    {
        if (sw == null)
        {
            Debug.LogError("EditorMeasureProcessTime:Start呼び忘れ");
            Debug.Assert(false);
        }
        else
        {
            sw.Stop();
        }

        if (result.list == null)
        {
            Debug.Log("処理時間保存していない");
        }
        else
        {
            SaveLog(MeasureKey, MeasureFuncName);
        }

#if UNITY_EDITOR
        Debug.Log("[" + MeasureKey + "][" + MeasureFuncName + "][" + sw.ElapsedMilliseconds + "(ms)]");
#endif
    }

    static public void Output()
    {
        // 外部テキストへ出力
        File.WriteAllText(fileNamePath, JsonUtility.ToJson(result, true));
        Debug.Log("処理時間計測結果出力:" + fileNamePath);
    }

}

}
