namespace Mediapipe.Tasks.Vision.HandLandmarker
{
  public sealed class HandLandmarkerOptions : Tasks.Core.ITaskOptions
  {

    public delegate void ResultCallback(HandLandmarkerResult handLandmarksResult, Image image, long timestampMillisec);

    public Tasks.Core.BaseOptions baseOptions { get; }

    public int numHands { get; }

    public float minHandDetectionConfidence { get; }

    public float minHandPresenceConfidence { get; }

    public float minTrackingConfidence { get; }

    public ResultCallback resultCallback { get; }

    public HandLandmarkerOptions(
      Tasks.Core.BaseOptions baseOptions,
      int numHands = 1,
      float minHandDetectionConfidence = 0.5f,
      float minHandPresenceConfidence = 0.5f,
      float minTrackingConfidence = 0.5f,
      ResultCallback resultCallback = null)
    {
      this.baseOptions = baseOptions;
      this.numHands = numHands;
      this.minHandDetectionConfidence = minHandDetectionConfidence;
      this.minHandPresenceConfidence = minHandPresenceConfidence;
      this.minTrackingConfidence = minTrackingConfidence;
      this.resultCallback = resultCallback;
    }

    internal Proto.HandLandmarkerGraphOptions ToProto()
    {
      var baseOptionsProto = baseOptions.ToProto();
      baseOptionsProto.UseStreamMode = true;

      return new Proto.HandLandmarkerGraphOptions
      {
        BaseOptions = baseOptionsProto,
        HandDetectorGraphOptions = new HandDetector.Proto.HandDetectorGraphOptions
        {
          NumHands = numHands,
          MinDetectionConfidence = minHandDetectionConfidence,
        },
        HandLandmarksDetectorGraphOptions = new Proto.HandLandmarksDetectorGraphOptions
        {
          MinDetectionConfidence = minHandPresenceConfidence,
        },
        MinTrackingConfidence = minTrackingConfidence,
      };
    }

    CalculatorOptions Tasks.Core.ITaskOptions.ToCalculatorOptions()
    {
      var options = new CalculatorOptions();
      options.SetExtension(Proto.HandLandmarkerGraphOptions.Extensions.Ext, ToProto());
      return options;
    }
  }
}
