using System;

namespace Mediapipe.Tasks.Vision.Core
{
  public class BaseVisionTaskApi : IDisposable
  {
    private readonly Tasks.Core.TaskRunner _taskRunner;
    private bool _isClosed = false;

    protected BaseVisionTaskApi(
      CalculatorGraphConfig graphConfig,
      Tasks.Core.TaskRunner.PacketsCallback packetsCallback)
    {
      var (callbackId, nativePacketsCallback) = Tasks.Core.PacketsCallbackTable.Add(packetsCallback);

      _taskRunner = Tasks.Core.TaskRunner.Create(graphConfig, callbackId, nativePacketsCallback);
    }

    protected void SendLiveStreamData(PacketMap inputs)
    {
      _taskRunner.Send(inputs);
    }

    protected void ConfigureNormalizedRect(NormalizedRect target)
    {
        target.Rotation = 0;
        target.XCenter = 0.5f;
        target.YCenter = 0.5f;
        target.Width = 1;
        target.Height = 1;
    }

    public void Close()
    {
      _taskRunner.Close();
      _isClosed = true;
    }

    void IDisposable.Dispose()
    {
      if (!_isClosed)
      {
        Close();
      }
      _taskRunner.Dispose();
    }
  }
}
