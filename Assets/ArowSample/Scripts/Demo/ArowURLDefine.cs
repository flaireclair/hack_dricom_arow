namespace ArowSampleGame.SampleScripts
{

/// <summary>
/// Arow URL Define.
/// 「.arowmap」ファイルがあるサーバーのURL定義
/// </summary>
public static partial class ArowURLDefine
{
    // sampleシーン「_StartScene」の「ArowMap」を選択した際の接続先サーバーURL
    public static readonly string DEFAULT_SERVER_URL = "http://localhost:18080/";
    // sample「Scene_DynamicMapLoad」を選択した際の接続先サーバーURL
    public static readonly string DYNAMIC_LOAD_MAP_SERVER_URL = DEFAULT_SERVER_URL;

}
}