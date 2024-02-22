using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor;
using UnityEngine;
using AssetsTools.NET.Extra;
using AssetsTools.NET;
using System.IO;

public class AddressablesBuild
{
    public static string build_script
            = "Assets/AddressableAssetsData/DataBuilders/BuildScriptFastMode.asset";
    public static string settings_asset
        = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
    public static string profile_name = "Default";

    private static AddressableAssetSettings settings;

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

    static void getSettingsObject(string settingsAsset)
    {
        // This step is optional, you can also use the default settings:
        //settings = AddressableAssetSettingsDefaultObject.Settings;

        settings
            = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset)
                as AddressableAssetSettings;

        if (settings == null)
            Debug.LogError($"{settingsAsset} couldn't be found or isn't " +
                           $"a settings object.");
    }

    static void setProfile(string profile)
    {
        string profileId = settings.profileSettings.GetProfileId(profile);
        if (String.IsNullOrEmpty(profileId))
            Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                             $"using current profile instead.");
        else
            settings.activeProfileId = profileId;
    }

    static void setBuilder(IDataBuilder builder)
    {
        int index = settings.DataBuilders.IndexOf((ScriptableObject)builder);

        if (index > 0)
            settings.ActivePlayerDataBuilderIndex = index;
        else
            Debug.LogWarning($"{builder} must be added to the " +
                             $"DataBuilders list before it can be made " +
                             $"active. Using last run builder instead.");
    }

    static bool buildAddressableContent()
    {
        AddressableAssetSettings
            .BuildPlayerContent(out AddressablesPlayerBuildResult result);
        bool success = string.IsNullOrEmpty(result.Error);

        if (!success)
        {
            Debug.LogError("Addressables build error encountered: " + result.Error);
        }
        else
        {
            var dir = Path.GetDirectoryName(result.OutputPath);
            var dataPath = Path.Join(dir, EditorUserBuildSettings.activeBuildTarget.ToString());

            EditDependencies(Path.Join(dataPath, "6abb75f3680e3b06557ab0c4d27bf44d.bundle"));
            EditDependencies(Path.Join(dataPath, "f38d2768ca886a5dbbe339e1d5a46132.bundle"));
        }

        return success;
    }

    [MenuItem("Assets/Build Addressables")]
    public static bool BuildAddressables()
    {
        getSettingsObject(settings_asset);
        setProfile(profile_name);
        IDataBuilder builderScript
          = AssetDatabase.LoadAssetAtPath<ScriptableObject>(build_script) as IDataBuilder;

        if (builderScript == null)
        {
            Debug.LogError(build_script + " couldn't be found or isn't a build script.");
            return false;
        }

        setBuilder(builderScript);

        return buildAddressableContent();
    }
}
