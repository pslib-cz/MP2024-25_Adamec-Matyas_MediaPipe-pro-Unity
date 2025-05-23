using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Mediapipe.Unity.Experimental
{
  public class TextureFramePool : IDisposable
  {
    private const string _TAG = nameof(TextureFramePool);

    public readonly int textureWidth;
    public readonly int textureHeight;
    public readonly TextureFormat textureFormat;
    public int poolSize { get; private set; }

    private readonly ReaderWriterLockSlim _textureFramesLock = new ReaderWriterLockSlim();
    private readonly Queue<TextureFrame> _availableTextureFrames;

    private readonly Dictionary<Guid, TextureFrame> _textureFramesInUse;

    public int frameCount => _availableTextureFrames.Count + _textureFramesInUse.Count;

    public TextureFramePool(int textureWidth, int textureHeight, TextureFormat textureFormat = TextureFormat.RGBA32, int poolSize = 10)
    {
      this.textureWidth = textureWidth;
      this.textureHeight = textureHeight;
      this.textureFormat = textureFormat;
      if (poolSize > TextureFrame.MaxTotalCount)
      {
        throw new ArgumentException($"poolSize must be <= {TextureFrame.MaxTotalCount}");
      }
      this.poolSize = poolSize;

      _availableTextureFrames = new Queue<TextureFrame>(poolSize);
      _textureFramesInUse = new Dictionary<Guid, TextureFrame>(poolSize);
    }

    public void Dispose()
    {
      _textureFramesLock.EnterWriteLock();
      try
      {
        foreach (var textureFrame in _availableTextureFrames)
        {
          textureFrame.Dispose();
        }
        _availableTextureFrames.Clear();

        foreach (var textureFrame in _textureFramesInUse.Values)
        {
          textureFrame.Dispose();
        }
        _textureFramesInUse.Clear();
        poolSize = 0;
      }
      finally
      {
        _textureFramesLock.ExitWriteLock();
      }
    }

    public bool TryGetTextureFrame(out TextureFrame outFrame)
    {
      TextureFrame nextFrame = null;
      var result = false;

      _textureFramesLock.EnterUpgradeableReadLock();
      try
      {
        if (poolSize <= frameCount)
        {
          if (_availableTextureFrames.Count > 0)
          {
            _textureFramesLock.EnterWriteLock();
            try
            {
              nextFrame = _availableTextureFrames.Dequeue();
              _textureFramesInUse.Add(nextFrame.GetInstanceID(), nextFrame);
              result = true;
            }
            finally
            {
              _textureFramesLock.ExitWriteLock();
            }
          }
        }
        else
        {
          nextFrame = CreateNewTextureFrame();
          _textureFramesLock.EnterWriteLock();
          try
          {
            _textureFramesInUse.Add(nextFrame.GetInstanceID(), nextFrame);
            result = true;
          }
          finally
          {
            _textureFramesLock.ExitWriteLock();
          }
        }
      }
      finally
      {
        _textureFramesLock.ExitUpgradeableReadLock();
      }

      outFrame = nextFrame;

      return result;
    }

    private void OnTextureFrameRelease(TextureFrame textureFrame)
    {
      _textureFramesLock.EnterWriteLock();
      try
      {
        if (!_textureFramesInUse.Remove(textureFrame.GetInstanceID()))
        {
          // won't be run
          UnityEngine.Debug.Log($"{_TAG} - The released texture does not belong to the pool");
          return;
        }

        // NOTE: poolSize won't be changed, so just enqueue the released texture here.
        _availableTextureFrames.Enqueue(textureFrame);
      }
      finally
      {
        _textureFramesLock.ExitWriteLock();
      }
    }

    private TextureFrame CreateNewTextureFrame()
    {
      var textureFrame = new TextureFrame(textureWidth, textureHeight, textureFormat);
      textureFrame.OnRelease.AddListener(OnTextureFrameRelease);

      return textureFrame;
    }
  }
}
