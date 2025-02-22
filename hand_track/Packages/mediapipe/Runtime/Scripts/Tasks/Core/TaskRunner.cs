using System;
using Google.Protobuf;

namespace Mediapipe.Tasks.Core
{
  public class TaskRunner : MpResourceHandle
  {
    public delegate void NativePacketsCallback(int name, IntPtr status, IntPtr packetMap);
    public delegate void PacketsCallback(PacketMap packetMap);


    public static TaskRunner Create(CalculatorGraphConfig config, int callbackId = -1, NativePacketsCallback packetsCallback = null)
    {
      var bytes = config.ToByteArray();
      UnsafeNativeMethods.mp_tasks_core_TaskRunner_Create__PKc_i_PF(bytes, bytes.Length, callbackId, packetsCallback, out var statusPtr, out var taskRunnerPtr).Assert();

      AssertStatusOk(statusPtr);
      return new TaskRunner(taskRunnerPtr);
    }

    private TaskRunner(IntPtr ptr) : base(ptr) { }


    public void Send(PacketMap inputs)
    {
      UnsafeNativeMethods.mp_tasks_core_TaskRunner__Send__Ppm(mpPtr, inputs.mpPtr, out var statusPtr).Assert();
      inputs.Dispose();

      GC.KeepAlive(this);
      AssertStatusOk(statusPtr);
    }

    public void Close()
    {
      UnsafeNativeMethods.mp_tasks_core_TaskRunner__Close(mpPtr, out var statusPtr).Assert();
      GC.KeepAlive(this);

      AssertStatusOk(statusPtr);
    }

  }
}
