using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RealSenseManager
{
    public interface IRealSenseManager : IColorSource
    {
        void Initialize();
        Texture2D GetCurrentRealSenseTexture();
        void Dispose();
        Texture2D GetDepthTexture();
    }
    public interface IColorSource
    {
        public struct Intrin
        {
            public float width;
            public float height;
            public float ppx;
            public float ppy;
            public float fx;
            public float fy;
        }

        Intrin GetIntrin();
    }
}
