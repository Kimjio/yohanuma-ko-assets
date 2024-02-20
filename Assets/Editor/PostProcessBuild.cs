using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

using System;
using System.IO;

using AssetsTools.NET;

public class PostProcessBuild
{
    [PostProcessBuildAttribute]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log(pathToBuiltProject);

        if (target == BuildTarget.StandaloneWindows64)
        {
            var projectName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
            var dir = Path.GetDirectoryName(pathToBuiltProject);
            Debug.Log(dir);
            var dataPath = Path.Join(Path.Join(Path.Join(dir, $"{projectName}_Data"), "StreamingAssets", "aa"), BuildTarget.StandaloneWindows64.ToString());

            Debug.Log(dataPath);
            var file = new AssetsFile();
            file.Read(new AssetsFileReader(Path.Join(dataPath, "6abb75f3680e3b06557ab0c4d27bf44d.bundle")));
            Debug.Log(file.AssetInfos);
        }
    }
}
