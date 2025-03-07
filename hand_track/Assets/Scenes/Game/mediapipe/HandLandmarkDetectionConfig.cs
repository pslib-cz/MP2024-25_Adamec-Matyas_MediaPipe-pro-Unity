
using Mediapipe.Tasks.Vision.HandLandmarker;

namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
  public class HandLandmarkDetectionConfig
  {
    public string ModelPath => "hand_landmarker.bytes";

    public HandLandmarkerOptions GetHandLandmarkerOptions(HandLandmarkerOptions.ResultCallback resultCallback = null)
    {
      return new HandLandmarkerOptions(
        new Tasks.Core.BaseOptions(
        modelAssetPath: ModelPath),
        numHands: 2,
        minHandDetectionConfidence: 0.5f,
        minHandPresenceConfidence: 0.5f,
        minTrackingConfidence: 0.5f,
        resultCallback: resultCallback
      );
    }
  }
}
