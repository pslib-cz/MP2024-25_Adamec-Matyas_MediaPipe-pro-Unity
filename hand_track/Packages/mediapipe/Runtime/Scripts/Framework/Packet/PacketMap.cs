using System;

namespace Mediapipe
{
  public class PacketMap : MpResourceHandle
  {
    public PacketMap() : base()
    {
      UnsafeNativeMethods.mp_PacketMap__(out var ptr).Assert();
      this.ptr = ptr;
    }

    public PacketMap(IntPtr ptr, bool isOwner) : base(ptr, isOwner) { }

    public Packet<T> At<T>(string key)
    {
      UnsafeNativeMethods.mp_PacketMap__find__PKc(mpPtr, key, out var packetPtr).Assert();

      if (packetPtr == IntPtr.Zero)
      {
        return default; // null
      }
      GC.KeepAlive(this);
      return new Packet<T>(packetPtr, true);
    }


    public void Emplace<T>(string key, Packet<T> packet)
    {
      UnsafeNativeMethods.mp_PacketMap__emplace__PKc_Rp(mpPtr, key, packet.mpPtr).Assert();
      packet.Dispose(); // respect move semantics
      GC.KeepAlive(this);
    }

  }
}
