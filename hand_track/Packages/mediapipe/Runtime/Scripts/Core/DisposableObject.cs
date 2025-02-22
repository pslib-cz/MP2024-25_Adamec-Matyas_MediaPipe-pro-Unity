using System;
using System.Threading;

namespace Mediapipe
{
  public abstract class DisposableObject : IDisposable
  {
    private volatile int _disposeSignaled = 0;
    private bool _isLocked;

    public bool isDisposed { get; protected set; }
    protected bool isOwner { get; private set; }

    protected DisposableObject(bool isOwner)
    {
      isDisposed = false;
      this.isOwner = isOwner;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_isLocked)
      {
        throw new InvalidOperationException("Cannot dispose a locked object, unlock it first");
      }

      if (Interlocked.Exchange(ref _disposeSignaled, 1) != 0)
      {
        return;
      }

      isDisposed = true;

    }

    protected void ThrowIfDisposed()
    {
      if (isDisposed)
      {
        throw new ObjectDisposedException(GetType().FullName);
      }
    }
  }
}
