using System;
using System.Collections.Generic;

namespace Mediapipe.Tasks.Components.Containers
{
  public static class PacketExtension
  {
    public static void Get(this Packet<List<Classifications>> packet, List<Classifications> outs)
    {
      UnsafeNativeMethods.mp_Packet__GetClassificationsVector(packet.mpPtr, out var classificationResult).Assert();
      var tmp = new ClassificationResult(outs, null);
      ClassificationResult.Copy(classificationResult, ref tmp);
      classificationResult.Dispose();
    }

    public static void Get(this Packet<List<NormalizedLandmarks>> packet, List<NormalizedLandmarks> outs)
    {
      UnsafeNativeMethods.mp_Packet__GetNormalizedLandmarksVector(packet.mpPtr, out var landmarksArray).Assert();
      outs.FillWith(landmarksArray);
      landmarksArray.Dispose();
    }
  }
}
