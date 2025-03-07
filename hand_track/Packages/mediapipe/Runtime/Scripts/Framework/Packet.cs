using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace Mediapipe
{
  public static class Packet
  {

    public static Packet<Image> CreateImageAt(Image value, long timestampMicrosec)
    {
      UnsafeNativeMethods.mp__MakeImagePacket_At__PI_ll(value.mpPtr, timestampMicrosec, out var ptr).Assert();
      value.Dispose(); // respect move semantics

      return new Packet<Image>(ptr, true);
    }

    public static Packet<TMessage> CreateProtoAt<TMessage>(TMessage value, long timestampMicrosec) where TMessage : IMessage<TMessage>
    {
      unsafe
      {
        var size = value.CalculateSize();
        var arr = stackalloc byte[size];
        value.WriteTo(new Span<byte>(arr, size));


        UnsafeNativeMethods.mp__PacketFromDynamicProto_At__PKc_PKc_i_ll(value.Descriptor.FullName, arr, size, timestampMicrosec, out var statusPtr, out var ptr).Assert();
        var ok = SafeNativeMethods.absl_Status__ok(statusPtr);
        if (!ok)
        {
            UnityEngine.Debug.Log("Status is not ok");
        }
        else
        {
            UnsafeNativeMethods.absl_Status__delete(statusPtr);
        }

        return new Packet<TMessage>(ptr, true);
      }
    }

  }

  public partial class Packet<TValue> : MpResourceHandle
  {
    internal Packet(IntPtr ptr, bool isOwner) : base(ptr, isOwner) { }

    public long TimestampMicroseconds()
    {
      var value = SafeNativeMethods.mp_Packet__TimestampMicroseconds(mpPtr);
      GC.KeepAlive(this);

      return value;
    }

    public bool IsEmpty() => SafeNativeMethods.mp_Packet__IsEmpty(mpPtr);

  }
}
