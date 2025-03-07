using System;
using System.Runtime.InteropServices;

namespace Mediapipe
{
  internal static partial class UnsafeNativeMethods
  {

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern void absl_Status__delete(IntPtr status);

    //[DllImport(MediaPipeLibrary, ExactSpelling = true)]
    //public static extern MpReturnCode absl_Status__ToString(IntPtr status, out IntPtr str);



    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern void delete_array__PKc(IntPtr str);

    #region String

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode std_string__PKc_i(byte[] bytes, int size, out IntPtr str);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern void std_string__swap__Rstr(IntPtr src, IntPtr dst);
    #endregion


    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Image__ui_i_i_i_Pui8_PF(
        ImageFormat.Types.Format format, int width, int height, int widthStep, IntPtr pixelData,
        [MarshalAs(UnmanagedType.FunctionPtr)] ImageFrame.Deleter deleter, out IntPtr image);


    #region Packet
    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp__MakeImagePacket_At__PI_ll(IntPtr image, long timestampMicrosec, out IntPtr packet);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetImage(IntPtr packet, out IntPtr image);

    #endregion




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



    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetClassificationsVector(IntPtr packet, out NativeClassificationResult value);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern void mp_tasks_c_components_containers_CppCloseClassificationResult(NativeClassificationResult data);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetNormalizedLandmarksVector(IntPtr packet, out NativeNormalizedLandmarksArray value);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern void mp_api_NormalizedLandmarksArray__delete(NativeNormalizedLandmarksArray data);








    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner_Create__PKc_i_PF(byte[] serializedConfig, int size,
        int callbackId, [MarshalAs(UnmanagedType.FunctionPtr)] Tasks.Core.TaskRunner.NativePacketsCallback packetsCallback,
        out IntPtr status, out IntPtr taskRunner);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner__Send__Ppm(IntPtr taskRunner, IntPtr inputs, out IntPtr status);

    [DllImport(MediaPipeLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner__Close(IntPtr taskRunner, out IntPtr status);
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    }

}
