using System;
using System.Collections;
using UnityEngine;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace Mediapipe.Unity.Sample
{
  public abstract class BaseRunner : MonoBehaviour
  {
    //private RealSenseManager _realSenseManager = new RealSenseManager();


    protected bool isPaused;

    private readonly Stopwatch _stopwatch = new();

    private void OnEnable()
    {
        var _ = StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        AssetLoader.Provide(new StreamingAssetsResourceManager());

        yield return null;
    }

    public virtual void Play()
    {
      //_realSenseManager.Initialize();
      isPaused = false;
      _stopwatch.Restart();
    }

    public virtual void Pause()
    {
      isPaused = true;
    }

    public virtual void Resume()
    {
      isPaused = false;
    }

    public virtual void Stop()
    {
      isPaused = true;
      _stopwatch.Stop();
    }

    protected long GetCurrentTimestampMillisec() => _stopwatch.IsRunning ? _stopwatch.ElapsedTicks / TimeSpan.TicksPerMillisecond : -1;


  }
}
