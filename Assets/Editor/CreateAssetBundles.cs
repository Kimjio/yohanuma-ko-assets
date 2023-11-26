using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles (Android)")]
    static void BuildAllAssetBundlesAndroid()
    {
        BuildPipeline.BuildAssetBundles("Assets/", BuildAssetBundleOptions.None, BuildTarget.Android);
    }
    [MenuItem("Assets/Build AssetBundles (iOS)")]
    static void BuildAllAssetBundlesIOS()
    {
        BuildPipeline.BuildAssetBundles("Assets/", BuildAssetBundleOptions.None, BuildTarget.iOS);
    }
    [MenuItem("Assets/Build AssetBundles (StandaloneWindows64)")]
    static void BuildAllAssetBundlesStandaloneWindows64()
    {
        BuildPipeline.BuildAssetBundles("Assets/", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}
