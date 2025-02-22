using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mediapipe
{
  public abstract class MpResourceHandle : DisposableObject
  {
    private IntPtr _ptr = IntPtr.Zero;
    protected IntPtr ptr
    {
      get => _ptr;
      set
      {
        if (value != IntPtr.Zero && OwnsResource())
        {
          throw new InvalidOperationException($"This object owns another resource");
        }
        _ptr = value;
      }
    }

    protected MpResourceHandle(bool isOwner = true) : this(IntPtr.Zero, isOwner) { }

    protected MpResourceHandle(IntPtr ptr, bool isOwner = true) : base(isOwner)
    {
      this.ptr = ptr;
    }

    public IntPtr mpPtr
    {
      get
      {
        ThrowIfDisposed();
        return ptr;
      }
    }

    public bool OwnsResource()
    {
      return isOwner && IsResourcePresent();
    }


    protected delegate MpReturnCode StringOutFunc(IntPtr ptr, out IntPtr strPtr);
    protected string MarshalStringFromNative(StringOutFunc f)
    {
      f(mpPtr, out var strPtr).Assert();
      GC.KeepAlive(this);

      return MarshalStringFromNative(strPtr);
    }

    protected static string MarshalStringFromNative(IntPtr strPtr)
    {
      var str = Marshal.PtrToStringAnsi(strPtr);
      UnsafeNativeMethods.delete_array__PKc(strPtr);

      return str;
    }
    protected static void AssertStatusOk(IntPtr statusPtr)
    {
      var ok = SafeNativeMethods.absl_Status__ok(statusPtr);
      if (!ok)
      {
        Debug.Log("Status is not ok");
      }
      else
      {
        UnsafeNativeMethods.absl_Status__delete(statusPtr);
      }
    }

    protected bool IsResourcePresent()
    {
      return ptr != IntPtr.Zero;
    }
  }
}
