using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Mediapipe
{
    public class Image : MpResourceHandle
    {
        public Image(IntPtr imagePtr, bool isOwner = true) : base(imagePtr, isOwner) { }

        public Image(ImageFormat.Types.Format format, int width, int height, int widthStep, IntPtr pixelData, ImageFrame.Deleter deleter) : base()
        {
            UnsafeNativeMethods.mp_Image__ui_i_i_i_Pui8_PF(format, width, height, widthStep, pixelData, deleter, out var ptr).Assert();
            this.ptr = ptr;
        }

        public unsafe Image(ImageFormat.Types.Format format, int width, int height, int widthStep, NativeArray<byte> pixelData, ImageFrame.Deleter deleter)
          : this(format, width, height, widthStep, (IntPtr)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(pixelData), deleter)
        { }

        public Image(ImageFormat.Types.Format format, int width, int height, int widthStep, NativeArray<byte> pixelData)
              : this(format, width, height, widthStep, pixelData, _VoidDeleter)
        { }

        // TODO: detect format from the texture
        public Image(ImageFormat.Types.Format format, Texture2D texture) :
            this(format, texture.width, texture.height, 4 * texture.width, texture.GetRawTextureData<byte>())
        { }


        private static readonly ImageFrame.Deleter _VoidDeleter = ImageFrame.VoidDeleter;


    }

}
