using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Mediapipe
{
  public class ImageFrame : MpResourceHandle
  {
    public static readonly uint DefaultAlignmentBoundary = 16;
    public static readonly uint GlDefaultAlignmentBoundary = 4;

    public delegate void Deleter(IntPtr ptr);

    private static readonly Deleter _VoidDeleter = VoidDeleter;

    [AOT.MonoPInvokeCallback(typeof(Deleter))]
    internal static void VoidDeleter(IntPtr _) { }



    private delegate MpReturnCode CopyToBufferHandler(IntPtr ptr, IntPtr buffer, int bufferSize);

  }
}
