using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Intel.RealSense;
using UnityEngine;

namespace RealSenseManager
{
    public class RealSenseManagerBag : IColorSource, IRealSenseManager
    {
        IColorSource.Intrin colorIntrin;
        public IColorSource.Intrin GetIntrin()
        {
            return colorIntrin;
        }


        private Pipeline _pipeline;
        private Intel.RealSense.Align _align;


        private static RealSenseManagerBag _instance;
        public static RealSenseManagerBag Instance
        {
            get
            {
                if (_instance == null)
                {
                    //throw new InvalidOperationException("RealSenseManager instance has not been initialized.");
                    return null;
                }
                return _instance;
            }
            private set { _instance = value; }
        }
        private int _colorWidth = 1280;
        private int _colorHeight = 720;

        public static RealSenseManagerBag InitializeInstance()
        {
            if (_instance != null)
            {
                Debug.LogError("RealSenseManager instance is already initialized.");
                return _instance;
            }
            _instance = new RealSenseManagerBag();
            return _instance;
        }

        public RealSenseManagerBag()
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

            _pipeline = new Pipeline();
            var cfg = new Config();
            cfg.DisableAllStreams();
            cfg.EnableDeviceFromFile("C:\\Users\\adame\\Documents\\20250207_092114.bag");
            //cfg.EnableStream(Stream.Depth, 1280, 720, Format.Z16, 30);
            //cfg.EnableStream(Stream.Color, 1280, 720, Format.Rgb8, 30);
            try
            {
                _pipeline.Start(cfg);
                _align = new Intel.RealSense.Align(Stream.Color);

                using (var frames = _pipeline.WaitForFrames())
                using (var alignedFrames = _align.Process(frames))
                {
                    FrameSet frameset = alignedFrames.AsFrameSet();
                    Frame colorFrame = frameset.ColorFrame;
                    var intrin = colorFrame.Profile.As<VideoStreamProfile>().GetIntrinsics();
                    colorIntrin.width = intrin.width;
                    colorIntrin.height = intrin.height;
                    colorIntrin.ppx = intrin.ppx;
                    colorIntrin.ppy = intrin.ppy;
                    colorIntrin.fx = intrin.fx;
                    colorIntrin.fy = intrin.fy;
                    frameset.Dispose();
                    colorFrame.Dispose();
                }
                Debug.Log("RealSense Pipeline started successfully");
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
        // 2764800
        private byte[] data = new byte[2764800];
        private byte[] flippedData = new byte[2764800];

        public Texture2D GetCurrentRealSenseTexture()
        {
            if (_pipeline == null)
            {
                Debug.LogError("RealSense pipeline not initialized");
                return null;
            }
            using (var frames = _pipeline.WaitForFrames())
            //using (var alignedFrames = _align.Process(frames))
            //using (var frameset = alignedFrames.AsFrameSet())
            using (var colorFrame = frames.ColorFrame)
            using (var depthFrame = frames.DepthFrame)
            {

                if (depthFrame != null)
                {
                    _depthTexture.LoadRawTextureData(depthFrame.Data, depthFrame.Stride * depthFrame.Height);
                    _depthTexture.Apply();
                    depthFrame.Dispose();
                }

                if (colorFrame != null)
                {
                    colorFrame.CopyTo(data);

                    for (int y = 0; y < colorFrame.Height; y++)
                    {
                        Buffer.BlockCopy(data, y * colorFrame.Stride, flippedData, (colorFrame.Height - y - 1) * colorFrame.Stride, colorFrame.Stride);
                    }

                    _colorTexture.LoadRawTextureData(flippedData);

                    _colorTexture.Apply();
                    colorFrame.Dispose();
                    return _colorTexture;
                }
                return _colorTexture;
            }
        }


        public void Dispose()
        {
            Debug.Log("Disposing RealSense Manager");
            _pipeline?.Stop();
            _pipeline?.Dispose();
            _pipeline = null;
            _instance = null;
        }

    }
}
