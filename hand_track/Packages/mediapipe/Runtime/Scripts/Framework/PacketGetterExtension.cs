using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Google.Protobuf;
using UnityEngine;

namespace Mediapipe
{
  public static class PacketGetterExtension
  {

    public static Image Get(this Packet<Image> packet)
    {
      UnsafeNativeMethods.mp_Packet__GetImage(packet.mpPtr, out var ptr).Assert();
      GC.KeepAlive(packet);
      return new Image(ptr, false);
    }

  }
}
