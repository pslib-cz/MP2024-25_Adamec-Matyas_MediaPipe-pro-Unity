using System;
using System.Runtime.InteropServices;

namespace Mediapipe
{
  internal static partial class UnsafeNativeMethods
  {
    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner_Create__PKc_i_PF(byte[] serializedConfig, int size,
        int callbackId, [MarshalAs(UnmanagedType.FunctionPtr)] Tasks.Core.TaskRunner.NativePacketsCallback packetsCallback,
        out IntPtr status, out IntPtr taskRunner);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner__Send__Ppm(IntPtr taskRunner, IntPtr inputs, out IntPtr status);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner__Close(IntPtr taskRunner, out IntPtr status);
  }
}
