using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Mediapipe
{
  [SuppressUnmanagedCodeSecurity]
  internal static partial class UnsafeNativeMethods
  {
    internal const string MediaPipeLibrary =
#if UNITY_EDITOR
      "mediapipe_c";
#elif UNITY_IOS || UNITY_WEBGL
      "__Internal";
#elif UNITY_ANDROID
      "mediapipe_jni";
#else
      "mediapipe_c";
#endif

    private delegate void FreeHGlobalDelegate(IntPtr hglobal);

  }
}
