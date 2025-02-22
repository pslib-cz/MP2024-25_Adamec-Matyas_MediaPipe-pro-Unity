using System;
using UnityEngine;

namespace Mediapipe.Tasks.Core
{
  internal class PacketsCallbackTable
  {
    private const int _MaxSize = 20;

    private static int _Counter = 0;
    private static readonly GlobalInstanceTable<int, TaskRunner.PacketsCallback> _Table = new GlobalInstanceTable<int, TaskRunner.PacketsCallback>(_MaxSize);

    public static (int, TaskRunner.NativePacketsCallback) Add(TaskRunner.PacketsCallback callback)
    {
      if (callback == null)
      {
        return (-1, null);
      }

      var callbackId = _Counter++;
      _Table.Add(callbackId, callback);
      return (callbackId, InvokeCallbackIfFound);
    }

    public static bool TryGetValue(int id, out TaskRunner.PacketsCallback callback) => _Table.TryGetValue(id, out callback);

    [AOT.MonoPInvokeCallback(typeof(TaskRunner.NativePacketsCallback))]
    private static void InvokeCallbackIfFound(int callbackId, IntPtr statusPtr, IntPtr packetMapPtr)
    {
      UnityEngine.Profiling.Profiler.BeginThreadProfiling("Mediapipe", "PacketsCallbackTable.InvokeCallbackIfFound");
      UnityEngine.Profiling.Profiler.BeginSample("PacketsCallbackTable.InvokeCallbackIfFound");

      if (packetMapPtr == IntPtr.Zero)
      {
        Debug.LogError(statusPtr);
        return;
      }

      if (TryGetValue(callbackId, out var callback))
      {
        try
        {
          callback(new PacketMap(packetMapPtr, false));
        }
        catch (Exception e)
        {
          //Debug.LogException(e);
        }
      }

      UnityEngine.Profiling.Profiler.EndSample();
      UnityEngine.Profiling.Profiler.EndThreadProfiling();
    }
  }
}
