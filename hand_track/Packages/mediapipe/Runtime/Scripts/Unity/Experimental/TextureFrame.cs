using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Mediapipe.Unity.Experimental
{
#pragma warning disable IDE0065
  using Color = UnityEngine.Color;
#pragma warning restore IDE0065

  public class TextureFrame : IDisposable
  {
    public class ReleaseEvent : UnityEvent<TextureFrame> { }

    private const string _TAG = nameof(TextureFrame);

    internal const int MaxTotalCount = 100;

    private static readonly GlobalInstanceTable<Guid, TextureFrame> _InstanceTable = new GlobalInstanceTable<Guid, TextureFrame>(MaxTotalCount);

    private static readonly Dictionary<uint, Guid> _NameTable = new Dictionary<uint, Guid>();

    private readonly Texture2D _texture;

    private IntPtr _nativeTexturePtr = IntPtr.Zero;

    private readonly Guid _instanceId;
    public readonly int width;
    public readonly int height;
    public readonly TextureFormat format;


#pragma warning disable IDE1006  // UnityEvent is PascalCase
    public readonly ReleaseEvent OnRelease;
#pragma warning restore IDE1006

    private RenderTexture _tmpRenderTexture;

    private TextureFrame(Texture2D texture)
    {
      _texture = texture;
      width = texture.width;
      height = texture.height;
      format = texture.format;
      OnRelease = new ReleaseEvent();
      _instanceId = Guid.NewGuid();
      _InstanceTable.Add(_instanceId, this);
      _onReadBackRenderTexture = OnReadBackRenderTexture;
    }

    public TextureFrame(int width, int height, TextureFormat format) : this(new Texture2D(width, height, format, false)) { }

    public void Dispose()
    {
      RemoveAllReleaseListeners();
      if (_nativeTexturePtr != IntPtr.Zero)
      {
        var name = (uint)_nativeTexturePtr;
        lock (((ICollection)_NameTable).SyncRoot)
        {
          _ = _NameTable.Remove(name);
        }
      }
      _ = _InstanceTable.Remove(_instanceId);
    }

    public AsyncGPUReadbackRequest ReadTextureAsync(Texture src, bool flipHorizontally = false, bool flipVertically = false)
    {
      ReadTextureInternal(src, flipHorizontally, flipVertically);
      return AsyncGPUReadback.Request(_tmpRenderTexture, 0, _onReadBackRenderTexture);
    }

    private void ReadTextureInternal(Texture src, bool flipHorizontally, bool flipVertically)
    {
      var graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(format, true);
      _tmpRenderTexture = RenderTexture.GetTemporary(src.width, src.height, 32, graphicsFormat);
      var currentRenderTexture = RenderTexture.active;
      RenderTexture.active = _tmpRenderTexture;

      var scale = new Vector2(1.0f, 1.0f);
      var offset = new Vector2(0.0f, 0.0f);

      if (flipHorizontally)
      {
        scale.x = -1.0f;
        offset.x = 1.0f;
      }
      if (flipVertically)
      {
        scale.y = -1.0f;
        offset.y = 1.0f;
      }
      
      Graphics.Blit(src, _tmpRenderTexture, scale, offset);
      RenderTexture.active = currentRenderTexture;
    }

    private readonly Action<AsyncGPUReadbackRequest> _onReadBackRenderTexture;
    private void OnReadBackRenderTexture(AsyncGPUReadbackRequest req) // realsense
    {
      if (_texture == null)
      {
        return;
      }
      _texture.LoadRawTextureData(req.GetData<byte>());
      _texture.Apply();
      RenderTexture.ReleaseTemporary(_tmpRenderTexture);
    }

    public Guid GetInstanceID() => _instanceId;

    public Image BuildCPUImage() => new Image(ImageFormat.Types.Format.Srgba, _texture);

    public void RemoveAllReleaseListeners() => OnRelease.RemoveAllListeners();

    public void Release()
    {
      OnRelease.Invoke(this);
    }

  }
}
