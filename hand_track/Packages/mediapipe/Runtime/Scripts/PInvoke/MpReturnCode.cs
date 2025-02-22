namespace Mediapipe
{
  public enum MpReturnCode : int
  {
    Success = 0,
    StandardError = 1,
    UnknownError = 70,
    Unset = 128
  }

  public static class MpReturnCodeExtension
  {
    public static void Assert(this MpReturnCode code)
    {
      switch (code)
      {
        case MpReturnCode.Success: return;
        case MpReturnCode.StandardError:
          {
            throw new System.Exception($"Exception is thrown in Unmanaged Code");
          }
        case MpReturnCode.UnknownError:
          {
            throw new System.Exception($"Unknown exception is thrown in Unmanaged Code");
          }
        case MpReturnCode.Unset:
          {
            // Bug
            throw new System.Exception($"Failed to call a native function, but the reason is unknown");
          }
        default:
          {
            throw new System.Exception($"Failed to call a native function, but the reason is undefined");
          }
      }
    }
  }
}
