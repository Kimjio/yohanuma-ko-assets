using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

using System;
using System.IO;

using AssetsTools.NET;
using AssetsTools.NET.Extra;

public class PostProcessBuild
{
    [PostProcessBuildAttribute]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.StandaloneWindows64)
        {
            var projectName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
            var dir = Path.GetDirectoryName(pathToBuiltProject);

            var dataPath = Path.Join(Path.Join(Path.Join(dir, $"{projectName}_Data"), "StreamingAssets", "aa"), BuildTarget.StandaloneWindows64.ToString());

            EditDependencies(Path.Join(dataPath, "6abb75f3680e3b06557ab0c4d27bf44d.bundle"));
            EditDependencies(Path.Join(dataPath, "f38d2768ca886a5dbbe339e1d5a46132.bundle"));
        }
    }

    static readonly AssetsManager manager = new();

    static void EditDependencies(string path)
    {
        var bundle = manager.LoadBundleFile(path);

        if (bundle.file.DataIsCompressed)
        {
            var unpackStream = new MemoryStream();
            bundle.file.Unpack(new AssetsFileWriter(unpackStream));

            unpackStream.Position = 0;

            bundle.file.Close();

            bundle.file = new AssetBundleFile();
            bundle.file.Read(new AssetsFileReader(unpackStream));
        }

        var assets = manager.LoadAssetsFileFromBundle(bundle, 0);

        var assetBundleInfo = assets.file.GetAssetsOfType(AssetClassID.AssetBundle)[0];

        AssetTypeValueField value = manager.GetBaseField(assets, assetBundleInfo);

        value.Children.Find(a => a.FieldName == "m_Dependencies")!.Children[0].Children[0].AsString = "cab-b6da7caf4f9adab40946bdf926ae8109";

        assetBundleInfo.SetNewData(value);

        bundle.file.BlockAndDirInfo.DirectoryInfos[0].SetNewData(assets.file);

        assets.file.Metadata.Externals[0] = new AssetsFileExternal { Type = AssetsFileExternalType.Normal, PathName = "archive:/CAB-b6da7caf4f9adab40946bdf926ae8109/CAB-b6da7caf4f9adab40946bdf926ae8109", VirtualAssetPathName = "" };

        var packStream = new MemoryStream();

        bundle.file.Write(new AssetsFileWriter(packStream));
        packStream.Position = 0;

        bundle.file.Close();

        bundle.file = new AssetBundleFile();
        bundle.file.Read(new AssetsFileReader(packStream));

        using var writer = new AssetsFileWriter(path);
        bundle.file.Pack(writer, AssetBundleCompressionType.LZ4);
        bundle.file.Close();

        Debug.Log($"Dependencies modifed: {path}");
    }
}
