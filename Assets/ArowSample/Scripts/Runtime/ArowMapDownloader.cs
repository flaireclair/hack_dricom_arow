using System.Collections;
using System.IO;
using UnityEngine;

namespace ArowSampleGame.Runtime
{

// 読み込んだデータが格納されるクラスについては外に出す必要があるかも（ディベロッパー側で値等を追加する可能性があるため

/// <summary>
/// 地図の動的ロード用の「.arowmap」ダウンローダー
/// </summary>
public class ArowMapDownloader
{
    private readonly string FILE_NAME_FORMAT = "block_{0}_{1}.arowmap";
    private readonly string INT_TO_STRING_FORMAT = "0000000000";

    public bool IsExistFile(int longitude, int latitude)
    {
        var dirPath = Path.Combine(Application.temporaryCachePath, "arow_map");
        var filePath = Path.Combine(dirPath, MakeFileName(longitude, latitude));
        return (File.Exists(filePath));
    }
    public byte[] GetMap(int longitude, int latitude)
    {
        string filename = MakeFileName(longitude, latitude);
        var dirPath = Path.Combine(Application.temporaryCachePath, "arow_map");
        var filePath = Path.Combine(dirPath, filename);
        return File.ReadAllBytes(filePath);
    }

    // コルーチン
    /// <summary>
    /// Arows the map download.
    /// 地図をサーバーからダウンロードする
    /// </summary>
    /// <returns>The map download.</returns>
    /// <param name="longitude">Longitude.</param>
    /// <param name="latitude">Latitude.</param>
    public IEnumerator ArowMapDownload(int longitude, int latitude)
    {
        if (IsExistFile(longitude, latitude))
        {
            yield break;
        }

        string filename = MakeFileName(longitude, latitude);
        var dirPath = Path.Combine(Application.temporaryCachePath, "arow_map");
        var filePath = Path.Combine(dirPath, filename);
        var unityWebRequest = UnityEngine.Networking.UnityWebRequest.Get(SampleScripts.ArowURLDefine.DYNAMIC_LOAD_MAP_SERVER_URL + filename);
        Debug.Log("ここにアクセス:" + SampleScripts.ArowURLDefine.DYNAMIC_LOAD_MAP_SERVER_URL + filename);
        ArowMain.Runtime.RequestManager.SetWebRequest(unityWebRequest, (www) =>
        {
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
            }
            else
            {
                var data = www.downloadHandler.data;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                File.WriteAllBytes(filePath, data);
            }
        });
    }

    private string MakeFileName(int longitude, int latitude)
    {
        return string.Format(FILE_NAME_FORMAT, longitude.ToString(INT_TO_STRING_FORMAT), latitude.ToString(INT_TO_STRING_FORMAT));
    }

}
}