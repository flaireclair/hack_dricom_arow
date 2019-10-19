using System.IO;
using UnityEngine;

namespace ArowMain.Public.Scripts.Runtime
{
public class Utilities
{
    public static byte[] ReadStreamingAssetsFileAllBytes(string filepath)
    {
        var filePath = Path.Combine(Application.streamingAssetsPath, filepath);
#if !UNITY_EDITOR && UNITY_ANDROID
        var request = UnityEngine.Networking.UnityWebRequest.Get(filePath);
        request.SendWebRequest();

        while (!request.downloadHandler.isDone)
        {
        }

        byte[] data = request.downloadHandler.data;
        return data;
#else
        byte[] data = File.ReadAllBytes(filePath);
        return data;
#endif
    }

}
}
