using UnityEngine;
using System;
using System.Linq;
using Mediapipe;
using UnityEngine.UIElements;
using Intel.RealSense;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

namespace RealSenseManager
{
    public class RealSenseManagerAndroid : IRealSenseManager
    {
        IColorSource.Intrin colorIntrin;
        public IColorSource.Intrin GetIntrin()
        {
            return colorIntrin;
        }

        private AndroidJavaObject _rsContext;
        private AndroidJavaObject _glRenderer;
        private AndroidJavaObject _pipeline;
        private AndroidJavaObject _colorizer;
        private AndroidJavaObject _colorEnum;
        private AndroidJavaObject _depthEnum;
        private AndroidJavaObject _align;

        private static RealSenseManagerAndroid _instance;
        public static RealSenseManagerAndroid Instance
        {
            get
            {
                if (_instance == null)
                {
                    return null;
                }
                return _instance;
            }
            private set { _instance = value; }
        }
        private int _colorWidth = 1280;
        private int _colorHeight = 720;

        private int _depthWidth = 1280;
        private int _depthHeight = 720;

        public static IRealSenseManager InitializeInstance()
        {
            if (_instance != null)
            {
                Debug.LogError("RealSenseManager instance is already initialized.");
                return null;
            }
            _instance = new RealSenseManagerAndroid();
            return _instance;
        }

        public RealSenseManagerAndroid()
        {
            Debug.Log("RealSenseManager constructor called");
            Initialize();
        }

        public void Initialize()
        {
            Debug.Log("Initializing RealSense Manager");
            if (_pipeline != null)
            {
                Dispose();
            }

            try
            {
                using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaClass streamTypeClass = new AndroidJavaClass("com.intel.realsense.librealsense.StreamType"))
                using (AndroidJavaClass streamFormatClass = new AndroidJavaClass("com.intel.realsense.librealsense.StreamFormat"))
                {
                    _colorEnum = streamTypeClass.GetStatic<AndroidJavaObject>("COLOR");
                    _depthEnum = streamTypeClass.GetStatic<AndroidJavaObject>("DEPTH");
                    AndroidJavaObject colorFormat = streamFormatClass.GetStatic<AndroidJavaObject>("RGB8");

                    _align = new AndroidJavaObject("com.intel.realsense.librealsense.Align", _colorEnum);

                    _glRenderer = new AndroidJavaObject("com.intel.realsense.librealsense.GLRenderer");
                    _rsContext = new AndroidJavaObject("com.intel.realsense.librealsense.RsContext");
                    _rsContext.CallStatic("init", GetUnityContext());

                    // Initialize Pipeline and Colorizer
                    _pipeline = new AndroidJavaObject("com.intel.realsense.librealsense.Pipeline");
                    _colorizer = new AndroidJavaObject("com.intel.realsense.librealsense.Colorizer");
                    AndroidJavaObject config = new AndroidJavaObject("com.intel.realsense.librealsense.Config");
                    config.Call("enableStream", _colorEnum, _colorWidth, _colorHeight, colorFormat); // COLOR
                    config.Call("enableStream", _depthEnum, _depthWidth, _depthHeight); // DEPTH
                                                                                        // Start the pipeline
                    AndroidJavaObject pipelineProfile = _pipeline.Call<AndroidJavaObject>("start", config);

                    AndroidJavaObject framesN = _pipeline.Call<AndroidJavaObject>("waitForFrames");
                    if (framesN != null)
                    {
                        try
                        {
                            AndroidJavaObject depthFrameSet = framesN.Call<AndroidJavaObject>("first", _depthEnum);
                            using (AndroidJavaClass extensionClass = new AndroidJavaClass("com.intel.realsense.librealsense.Extension"))
                            using (AndroidJavaClass utilsClass = new AndroidJavaClass("com.intel.realsense.librealsense.Utils"))
                            {
                                AndroidJavaObject depthFrameExtension = extensionClass.GetStatic<AndroidJavaObject>("DEPTH_FRAME");
                                AndroidJavaObject depthFrameN = depthFrameSet.Call<AndroidJavaObject>("as", depthFrameExtension);

                                AndroidJavaObject depthProfile = depthFrameN.Call<AndroidJavaObject>("getProfile");
                                AndroidJavaObject depthFrameIntrinsic = depthProfile.Call<AndroidJavaObject>("getIntrinsic");

                                colorIntrin.width = depthFrameIntrinsic.Call<int>("getWidth");
                                colorIntrin.height = depthFrameIntrinsic.Call<int>("getHeight");
                                colorIntrin.ppx = depthFrameIntrinsic.Call<float>("getPpx");
                                colorIntrin.ppy = depthFrameIntrinsic.Call<float>("getPpy");
                                colorIntrin.fx = depthFrameIntrinsic.Call<float>("getFx");
                                colorIntrin.fy = depthFrameIntrinsic.Call<float>("getFy");
                                Debug.Log(colorIntrin.width + " - " + colorIntrin.height + " - " + colorIntrin.ppx + " - " + colorIntrin.ppy + " - " + colorIntrin.fx + " - " + colorIntrin.fy);
                            }


                            framesN.Call("close");
                            depthFrameSet.Call("close");
                        }
                        catch (AndroidJavaException e)
                        {
                            Debug.LogError("Error: " + e.Message);
                        }


                    }

                    Debug.Log($"RealSense Pipeline succesfull - D-{_depthWidth}x{_depthHeight} | C-{_colorWidth}x{_colorHeight}");
                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to start RealSense Pipeline: {ex.Message}");
            }
        }

        private Texture2D _depthTexture = new Texture2D(1280, 720, TextureFormat.R16, false, true);
        private Texture2D _colorTexture = new Texture2D(1280, 720, TextureFormat.RGB24, false, true);

        public Texture2D GetDepthTexture()
        {
            return _depthTexture;
        }

        private byte[] data = new byte[2764800];
        private byte[] flippedData = new byte[2764800];
        private int stride = 3840;

        private AndroidJavaObject frames;
        private AndroidJavaObject processed;
        private AndroidJavaObject aligned;
        private AndroidJavaObject colorFrame;
        private AndroidJavaObject depthFrame;
        public Texture2D GetCurrentRealSenseTexture()
        {

            if (_pipeline == null)
            {
                Debug.LogError("RealSense pipeline not initialized");
                return null;
            }
            frames = _pipeline.Call<AndroidJavaObject>("waitForFrames");
            if (frames != null)
            {

                colorFrame = frames.Call<AndroidJavaObject>("first", _colorEnum);
                depthFrame = frames.Call<AndroidJavaObject>("first", _depthEnum);

                frames.Call("close");

                if (depthFrame != null)
                {

                    byte[] mydata = depthFrame.Call<byte[]>("getMyData");
                    Debug.Log($"Stride: {stride * _colorHeight}");

                    _depthTexture.LoadRawTextureData(mydata);
                    _depthTexture.Apply();
                    depthFrame.Call("close");
                }

                if (colorFrame != null)
                {
                    data = colorFrame.Call<byte[]>("getMyData");

                    for (int y = 0; y < _colorHeight; y++)
                    {
                        Buffer.BlockCopy(data, y * stride, flippedData, (_colorHeight - y - 1) * stride, stride);
                    }

                    _colorTexture.LoadRawTextureData(flippedData);
                    _colorTexture.Apply();

                    colorFrame.Call("close");

                    return _colorTexture;
                }
                return null;
            }
            return null;
        }

        public void Dispose()
        {
            Debug.Log("Disposing RealSense Manager");
            _pipeline.Call("close");
            _pipeline = null;
            _instance = null;
        }


        private AndroidJavaObject GetUnityContext()
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }
    }
}