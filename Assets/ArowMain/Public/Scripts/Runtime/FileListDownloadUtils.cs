using System.Collections.Generic;
using ArowMain.MiniJSON;

namespace ArowMain.Public.Scripts.Runtime
{

public class FileListDownloadUtils
{
    /// <summary>
    /// レスポンス結果からファイル一覧のリストを返す
    /// </summary>
    /// <param name="text">DownloadHandler.text を受け取る想定</param>
    /// <returns></returns>
    public static List<string> GetParsedFileList(string text)
    {
        var root = Json.Deserialize(text) as Dictionary<string, object>;
        var files = root["files"] as List<object>;
        return files.ConvertAll((input) => input.ToString());
    }
}

}
