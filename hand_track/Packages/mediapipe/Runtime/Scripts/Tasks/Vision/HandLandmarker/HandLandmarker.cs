
using System.Collections.Generic;
using Mediapipe.Tasks.Components.Containers;
using UnityEngine;

namespace Mediapipe.Tasks.Vision.HandLandmarker
{
  public sealed class HandLandmarker : Core.BaseVisionTaskApi
  {
    private const string _IMAGE_IN_STREAM_NAME = "image_in";
    private const string _IMAGE_OUT_STREAM_NAME = "image_out";
    private const string _IMAGE_TAG = "IMAGE";
    private const string _NORM_RECT_STREAM_NAME = "norm_rect_in";
    private const string _NORM_RECT_TAG = "NORM_RECT";
    private const string _HANDEDNESS_STREAM_NAME = "handedness";
    private const string _HANDEDNESS_TAG = "HANDEDNESS";
    private const string _HAND_LANDMARKS_STREAM_NAME = "landmarks";
    private const string _HAND_LANDMARKS_TAG = "LANDMARKS";
    private const string _HAND_WORLD_LANDMARKS_STREAM_NAME = "world_landmarks";
    private const string _HAND_WORLD_LANDMARKS_TAG = "WORLD_LANDMARKS";
    private const string _TASK_GRAPH_NAME = "mediapipe.tasks.vision.hand_landmarker.HandLandmarkerGraph";

    private const int _MICRO_SECONDS_PER_MILLISECOND = 1000;

#pragma warning disable IDE0052 // Remove unread private members
    private readonly Tasks.Core.TaskRunner.PacketsCallback _packetCallback;
#pragma warning restore IDE0052

    private readonly NormalizedRect _normalizedRect = new NormalizedRect();

    private HandLandmarker(
      CalculatorGraphConfig graphConfig,
      Tasks.Core.TaskRunner.PacketsCallback packetCallback) : base(graphConfig, packetCallback)
    {
      _packetCallback = packetCallback;
    }

    public static HandLandmarker CreateFromOptions(HandLandmarkerOptions options)
    {
      var taskInfo = new Tasks.Core.TaskInfo<HandLandmarkerOptions>(
        taskGraph: _TASK_GRAPH_NAME,
        inputStreams: new List<string> {
          string.Join(":", _IMAGE_TAG, _IMAGE_IN_STREAM_NAME),
          string.Join(":", _NORM_RECT_TAG, _NORM_RECT_STREAM_NAME),
        },
        outputStreams: new List<string> {
          string.Join(":", _HANDEDNESS_TAG, _HANDEDNESS_STREAM_NAME),
          string.Join(":", _HAND_LANDMARKS_TAG, _HAND_LANDMARKS_STREAM_NAME),
          string.Join(":", _HAND_WORLD_LANDMARKS_TAG, _HAND_WORLD_LANDMARKS_STREAM_NAME),
          string.Join(":", _IMAGE_TAG, _IMAGE_OUT_STREAM_NAME),
        },
        taskOptions: options);

      return new HandLandmarker(
        taskInfo.GenerateGraphConfig(),
        BuildPacketsCallback(options));
    }

    public void DetectAsync(Image image, long timestampMillisec)
    {
      ConfigureNormalizedRect(_normalizedRect);
      var timestampMicrosec = timestampMillisec * _MICRO_SECONDS_PER_MILLISECOND;

      var packetMap = new PacketMap();
      packetMap.Emplace(_IMAGE_IN_STREAM_NAME, Packet.CreateImageAt(image, timestampMicrosec));
      packetMap.Emplace(_NORM_RECT_STREAM_NAME, Packet.CreateProtoAt(_normalizedRect, timestampMicrosec));

      SendLiveStreamData(packetMap);
    }

    private static Tasks.Core.TaskRunner.PacketsCallback BuildPacketsCallback(HandLandmarkerOptions options)
    {
      var resultCallback = options.resultCallback;
      if (resultCallback == null)
      {
        return null;
      }

      var handLandmarkerResult = HandLandmarkerResult.Alloc(options.numHands);

      return (PacketMap outputPackets) =>
      {
        using var outImagePacket = outputPackets.At<Image>(_IMAGE_OUT_STREAM_NAME);
        if (outImagePacket == null || outImagePacket.IsEmpty())
        {
          return;
        }

        using var image = outImagePacket.Get();
        var timestamp = outImagePacket.TimestampMicroseconds() / _MICRO_SECONDS_PER_MILLISECOND;

        if (TryBuildHandLandmarkerResult(outputPackets, ref handLandmarkerResult))
        {
          resultCallback(handLandmarkerResult, image, timestamp);
        }
        else
        {
          resultCallback(default, image, timestamp);
        }
      };
    }

    private static bool TryBuildHandLandmarkerResult(PacketMap outputPackets, ref HandLandmarkerResult result)
    {
      using var handLandmarksPacket = outputPackets.At<List<NormalizedLandmarks>>(_HAND_LANDMARKS_STREAM_NAME);
      if (handLandmarksPacket.IsEmpty())
      {
        return false;
      }

      var handLandmarks = result.handLandmarks ?? new List<NormalizedLandmarks>();
      handLandmarksPacket.Get(handLandmarks);

      using var handednessPacket = outputPackets.At<List<Classifications>>(_HANDEDNESS_STREAM_NAME);
      var handedness = result.handedness ?? new List<Classifications>();
      handednessPacket.Get(handedness);


      result = new HandLandmarkerResult(handedness, handLandmarks);
      return true;
    }
  }
}
