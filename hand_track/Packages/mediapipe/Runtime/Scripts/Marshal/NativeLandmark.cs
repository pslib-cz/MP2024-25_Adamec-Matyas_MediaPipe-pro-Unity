using System;
using System.Runtime.InteropServices;

namespace Mediapipe
{
  [StructLayout(LayoutKind.Sequential)]
  internal readonly struct NativeLandmark
  {
    public readonly float x;
    public readonly float y;
    public readonly float z;

    [MarshalAs(UnmanagedType.I1)]
    public readonly bool hasVisibility;
    public readonly float visibility;

    [MarshalAs(UnmanagedType.I1)]
    public readonly bool hasPresence;
    public readonly float presence;

    private readonly IntPtr _name;

    public string name => Marshal.PtrToStringAnsi(_name);
  }

  [StructLayout(LayoutKind.Sequential)]
  internal readonly struct NativeNormalizedLandmark
  {
    public readonly float x;
    public readonly float y;
    public readonly float z;

    [MarshalAs(UnmanagedType.I1)]
    public readonly bool hasVisibility;
    public readonly float visibility;

    [MarshalAs(UnmanagedType.I1)]
    public readonly bool hasPresence;
    public readonly float presence;

    private readonly IntPtr _name;

    public string name => Marshal.PtrToStringAnsi(_name);
  }

  [StructLayout(LayoutKind.Sequential)]
  internal readonly struct NativeLandmarks
  {
    private readonly IntPtr _landmarks;
    public readonly uint landmarksCount;

    public ReadOnlySpan<NativeLandmark> AsReadOnlySpan()
    {
      unsafe
      {
        return new ReadOnlySpan<NativeLandmark>((NativeLandmark*)_landmarks, (int)landmarksCount);
      }
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  internal readonly struct NativeNormalizedLandmarks
  {
    private readonly IntPtr _landmarks;
    public readonly uint landmarksCount;

    public ReadOnlySpan<NativeNormalizedLandmark> AsReadOnlySpan()
    {
      unsafe
      {
        return new ReadOnlySpan<NativeNormalizedLandmark>((NativeNormalizedLandmark*)_landmarks, (int)landmarksCount);
      }
    }
  }


  [StructLayout(LayoutKind.Sequential)]
  internal readonly struct NativeNormalizedLandmarksArray
  {
    private readonly IntPtr _data;
    public readonly int size;

    public void Dispose()
    {
      UnsafeNativeMethods.mp_api_NormalizedLandmarksArray__delete(this);
    }

    public ReadOnlySpan<NativeNormalizedLandmarks> AsReadOnlySpan()
    {
      unsafe
      {
        return new ReadOnlySpan<NativeNormalizedLandmarks>((NativeNormalizedLandmarks*)_data, size);
      }
    }
  }
}
