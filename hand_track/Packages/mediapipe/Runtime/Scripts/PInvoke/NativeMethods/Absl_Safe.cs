using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Mediapipe
{
  internal static partial class SafeNativeMethods
  {
    [Pure, DllImport(MediaPipeLibrary, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool absl_Status__ok(IntPtr status);

    //[Pure, DllImport(MediaPipeLibrary, ExactSpelling = true)]
    //public static extern int absl_Status__raw_code(IntPtr status);




    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern void mp__SetCustomGlobalResourceProvider__P(
        [MarshalAs(UnmanagedType.FunctionPtr)] ResourceUtil.NativeResourceProvider provider);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern void mp__SetCustomGlobalPathResolver__P(
        [MarshalAs(UnmanagedType.FunctionPtr)] ResourceUtil.PathResolver resolver);




    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool mp_Packet__IsEmpty(IntPtr packet);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern long mp_Packet__TimestampMicroseconds(IntPtr packet);
    }
}