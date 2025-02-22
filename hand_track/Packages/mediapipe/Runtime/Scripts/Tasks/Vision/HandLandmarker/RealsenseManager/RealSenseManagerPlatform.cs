using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RealSenseManager
{
    public static class RealSenseManagerFactory
    {

        public static Platform selectedPlatform = Platform.Android;
        public static IRealSenseManager GetManager(Platform platform)
        {
            switch (platform)
            {
                case Platform.Android:
                    if (RealSenseManagerAndroid.Instance == null)
                    {
                        Debug.Log("Initializing RealSenseManagerAndroid");
                        return (RealSenseManagerAndroid)RealSenseManagerAndroid.InitializeInstance();
                    }
                    else
                        return (RealSenseManagerAndroid)RealSenseManagerAndroid.Instance;
                case Platform.Computer:
                    if (RealSenseManagerPc.Instance == null)
                    {
                        Debug.Log("Initializing RealSenseManagerPc");
                        return RealSenseManagerPc.InitializeInstance();
                    }
                    else
                        return RealSenseManagerPc.Instance;
                    return null;
                case Platform.Bag:
                    if (RealSenseManagerBag.Instance == null)
                    {
                        Debug.Log("Initializing RealSenseManagerBag");
                        return RealSenseManagerBag.InitializeInstance();
                    }
                    else
                        return RealSenseManagerBag.Instance;
                    return null;
                default:
                    return null;
            }
        }

        public static IRealSenseManager GetManager() => GetManager(selectedPlatform);
    }

    public enum Platform
    {
        Android,
        Computer,
        Bag
    }
}
