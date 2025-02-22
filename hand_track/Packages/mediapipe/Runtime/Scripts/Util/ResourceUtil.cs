using System;
using System.Collections.Generic;
using System.IO;

namespace Mediapipe
{
  public static class ResourceUtil
  {
    internal delegate string PathResolver(string path);
    internal delegate bool NativeResourceProvider(string path, IntPtr dest);

    private static readonly string _TAG = nameof(ResourceUtil);

    private static bool _IsInitialized;
    private static readonly Dictionary<string, string> _AssetPathMap = new Dictionary<string, string>();

    public static void EnableCustomResolver()
    {
      if (_IsInitialized)
      {
        return;
      }
      SafeNativeMethods.mp__SetCustomGlobalPathResolver__P(PathToResourceAsFile);
      SafeNativeMethods.mp__SetCustomGlobalResourceProvider__P(GetResourceContents);
      _IsInitialized = true;
    }

    public static void SetAssetPath(string assetKey, string assetPath) => _AssetPathMap[assetKey] = assetPath;

    public static bool TryGetFilePath(string assetPath, out string filePath)
    {
      if (_AssetPathMap.TryGetValue(assetPath, out filePath))
      {
        return true;
      }
      if (_AssetPathMap.TryGetValue(GetAssetNameFromPath(assetPath), out filePath))
      {
        return true;
      }
      return false;
    }

    [AOT.MonoPInvokeCallback(typeof(PathResolver))]
    private static string PathToResourceAsFile(string assetPath)
    {
      UnityEngine.Debug.Log($"{assetPath} is requested");
      try
      {
        UnityEngine.Debug.Log($"{_TAG} - {assetPath} is requested");
        if (TryGetFilePath(assetPath, out var filePath))
        {
          return filePath;
        }
        throw new KeyNotFoundException($"Failed to find the file path for `{assetPath}`");
      }
      catch (Exception e)
      {
        UnityEngine.Debug.Log(e);
        return "";
      }
    }

    [AOT.MonoPInvokeCallback(typeof(NativeResourceProvider))]
    private static bool GetResourceContents(string path, IntPtr dst)
    {
      UnityEngine.Debug.Log($"{path} is requested");

      try
      {
        UnityEngine.Debug.Log($"{_TAG} - {path} is requested");

        if (!TryGetFilePath(path, out var filePath))
        {
          throw new KeyNotFoundException($"Failed to find the file path for `{path}`");
        }

        var asset = File.ReadAllBytes(filePath);
        using (var srcStr = new StdString(asset))
        {
          srcStr.Swap(new StdString(dst, false));
        }
        return true;
      }
      catch (Exception e)
      {
        UnityEngine.Debug.Log(e);
        return false;
      }
    }

    private static string GetAssetNameFromPath(string assetPath)
    {
      var assetName = Path.GetFileNameWithoutExtension(assetPath);
      var extension = Path.GetExtension(assetPath);

      switch (extension)
      {
        case ".binarypb":
        case ".tflite":
          {
            return $"{assetName}.bytes";
          }
        case ".pbtxt":
          {
            return $"{assetName}.txt";
          }
        default:
          {
            return $"{assetName}{extension}";
          }
      }
    }
  }
}
