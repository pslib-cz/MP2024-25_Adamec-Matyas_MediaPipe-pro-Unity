using System;
using System.Runtime.InteropServices;

namespace Mediapipe
{
  [StructLayout(LayoutKind.Sequential)]
  internal readonly struct NativeCategory
  {
    public readonly int index;
    public readonly float score;
    private readonly IntPtr _categoryName;
    private readonly IntPtr _displayName;

    public string categoryName => Marshal.PtrToStringAnsi(_categoryName);
    public string displayName => Marshal.PtrToStringAnsi(_displayName);
  }

}
