using System;
using System.Runtime.InteropServices;

namespace Mediapipe
{
  internal static partial class UnsafeNativeMethods
  {

    #region Proto
    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern unsafe MpReturnCode mp__PacketFromDynamicProto_At__PKc_PKc_i_ll(string typeName, byte* proto, int size, long timestampMicrosec,
        out IntPtr status, out IntPtr packet);
    #endregion

    #region PacketMap
    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_PacketMap__(out IntPtr packetMap);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_PacketMap__emplace__PKc_Rp(IntPtr packetMap, string key, IntPtr packet);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_PacketMap__find__PKc(IntPtr packetMap, string key, out IntPtr packet);
    #endregion
  }
}
