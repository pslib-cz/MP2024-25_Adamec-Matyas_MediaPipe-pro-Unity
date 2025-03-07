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

namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
    public class Script : MyTaskApi<HandLandmarker>
    {
        public readonly HandLandmarkDetectionConfig config = new HandLandmarkDetectionConfig();

        private Experimental.TextureFramePool _textureFramePool;

        private static RawImage colorTextureImage;

        public override void Stop()
        {
            base.Stop();
            _textureFramePool?.Dispose();
            _textureFramePool = null;
        }


        private void Awake()
        {
            colorTextureImage = GameObject.Find("ColorImageDev").GetComponent<RawImage>();
        }
        protected override IEnumerator Run()
        {
            UnityEngine.Device.Application.targetFrameRate = 30;
            UnityEngine.Application.targetFrameRate = 30;

            yield return AssetLoader.PrepareAssetAsync(config.ModelPath);

            var options = config.GetHandLandmarkerOptions(OnHandLandmarkDetectionOutput);
            taskApi = HandLandmarker.CreateFromOptions(options);



            _textureFramePool = new Experimental.TextureFramePool(1280, 720, TextureFormat.RGBA32, 10);


            AsyncGPUReadbackRequest req = default;
            var waitUntilReqDone = new WaitUntil(() => req.done);

            Image image;
            IRealSenseManager _realsenseManager = RealSenseManagerFactory.GetManager();

            var tStart = DateTime.Now;
            var nFrames = 0;


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
                if (_realsenseManager != null)
                {
                    colorTextureImage.texture = texture;
                }

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

                FPSCounter.instance.frameRun();

                var tEnd = DateTime.Now;
                var elapsed = tEnd - tStart;

                ++nFrames;
                if (elapsed.TotalSeconds > 5.0)
                {
                    Debug.Log($"FPS: {nFrames / elapsed.TotalSeconds}");
                    tStart = tEnd;
                    nFrames = 0;
                }
            }
        }

        private void OnHandLandmarkDetectionOutput(HandLandmarkerResult result, Image image, long timestamp)
        {
            if (result.handedness == null || result.handLandmarks == null)
            {
                HandManager.Instance.hideLandmarksValuesR();
                HandManager.Instance.hideLandmarksValuesL();
                return;
            }

            else if (result.handedness.Count == 1)
            {
                var landmarks = result.handLandmarks[0].landmarks;
                if (result.handedness[0].categories[0].displayName == "Right")
                {
                    HandManager.Instance.adjustLandmarkPositionValuesR(landmarks);
                    HandManager.Instance.hideLandmarksValuesL();
                }
                else
                {
                    HandManager.Instance.adjustLandmarkPositionValuesL(landmarks);
                    HandManager.Instance.hideLandmarksValuesR();
                }
            }
            else if (result.handedness.Count == 2)
            {
                if (result.handedness[0].categories[0].displayName == "Right")
                {
                    var landmarksR = result.handLandmarks[0].landmarks;
                    var landmarksL = result.handLandmarks[1].landmarks;
                    HandManager.Instance.adjustLandmarkPositionValuesR(landmarksR);
                    HandManager.Instance.adjustLandmarkPositionValuesL(landmarksL);
                }
                else if (result.handedness[0].categories[0].displayName == "Left")
                {
                    var landmarksR = result.handLandmarks[1].landmarks;
                    var landmarksL = result.handLandmarks[0].landmarks;
                    HandManager.Instance.adjustLandmarkPositionValuesR(landmarksR);
                    HandManager.Instance.adjustLandmarkPositionValuesL(landmarksL);
                }
            }
        }
    }
}