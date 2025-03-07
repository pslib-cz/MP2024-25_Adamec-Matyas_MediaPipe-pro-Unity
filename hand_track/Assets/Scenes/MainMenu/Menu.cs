using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity;
using Mediapipe.Unity.Experimental;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Rendering;
using RealSenseManager;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System;

using Debug = UnityEngine.Debug;
using System.Timers;
using Packages.mediapipe.Runtime.Scripts.Unity;

namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
    public class Menu : MyTaskApi<HandLandmarker>
    {
        public readonly HandLandmarkDetectionConfig config = new HandLandmarkDetectionConfig();
        private Experimental.TextureFramePool _textureFramePool;
        public override void Stop()
        {
            base.Stop();
            _textureFramePool?.Dispose();
            _textureFramePool = null;
        }

        private void Awake()
        {
            Debug.Log("Menu Awake");
        }

        protected override IEnumerator Run()
        {
            yield return AssetLoader.PrepareAssetAsync(config.ModelPath);

            var options = config.GetHandLandmarkerOptions(OnHandLandmarkDetectionOutput);
            taskApi = HandLandmarker.CreateFromOptions(options);
            _textureFramePool = new Experimental.TextureFramePool(1280, 720, TextureFormat.RGBA32, 10);
            AsyncGPUReadbackRequest req = default;
            var waitUntilReqDone = new WaitUntil(() => req.done);
            Image image;
            IRealSenseManager _realsenseManager = RealSenseManagerFactory.GetManager();

            while (true)
            {
                if (isPaused)
                {
                    yield return new WaitWhile(() => isPaused);
                }

                if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                var texture = _realsenseManager.GetCurrentRealSenseTexture();

                req = textureFrame.ReadTextureAsync(texture, true, false);
                yield return waitUntilReqDone;

                if (req.hasError)
                {
                    Debug.LogError($"Failed to read texture from the image source, exiting...");
                    break;
                }
                image = textureFrame.BuildCPUImage();
                textureFrame.Release();

                taskApi.DetectAsync(image, GetCurrentTimestampMillisec());
            }
        }

        private void OnHandLandmarkDetectionOutput(HandLandmarkerResult result, Image image, long timestamp)
        {
            Debug.Log("HandLandmarkDetectionResult");
            if (result.handedness == null || result.handLandmarks == null)
            {
                MenuHandManager.Instance.hideLandmarksValuesR();
                MenuHandManager.Instance.hideLandmarksValuesL();
                return;
            }

            else if (result.handedness.Count == 1)
            {
                var landmarks = result.handLandmarks[0].landmarks;
                if (result.handedness[0].categories[0].displayName == "Right")
                {
                    MenuHandManager.Instance.adjustLandmarkPositionValuesR(landmarks);
                    MenuHandManager.Instance.hideLandmarksValuesL();
                }
                else
                {
                    MenuHandManager.Instance.adjustLandmarkPositionValuesL(landmarks);
                    MenuHandManager.Instance.hideLandmarksValuesR();
                }
            }
            else if (result.handedness.Count == 2)
            {
                if (result.handedness[0].categories[0].displayName == "Right")
                {
                    var landmarksR = result.handLandmarks[0].landmarks;
                    var landmarksL = result.handLandmarks[1].landmarks;
                    MenuHandManager.Instance.adjustLandmarkPositionValuesR(landmarksR);
                    MenuHandManager.Instance.adjustLandmarkPositionValuesL(landmarksL);
                }
                else if (result.handedness[0].categories[0].displayName == "Left")
                {
                    var landmarksR = result.handLandmarks[1].landmarks;
                    var landmarksL = result.handLandmarks[0].landmarks;
                    MenuHandManager.Instance.adjustLandmarkPositionValuesR(landmarksR);
                    MenuHandManager.Instance.adjustLandmarkPositionValuesL(landmarksL);
                }
            }
        }
    }
}