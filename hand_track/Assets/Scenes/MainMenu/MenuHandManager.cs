using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Tasks.Components.Containers;
using System.Threading;
using RealSenseManager;
using System;
using Packages.mediapipe.Runtime.Scripts.Unity;
using Mediapipe.Unity.Sample;

public class MenuHandManager : MonoBehaviour
{
    public static MenuHandManager Instance;
    private IRealSenseManager realSenseManager;

    private NormalizedLandmark? _landmarkR;
    private NormalizedLandmark? _landmarkL;
    private NormalizedLandmark? _landmarkWristR;
    private NormalizedLandmark? _landmarkWristL;
    
    private NormalizedLandmark? _landmarkThumbR;
    private NormalizedLandmark? _landmarkThumbL;

    private float movementFactor = 200f;
    private float scaleFactor = 50f;
    private float yOffSet = 1.0f;

    private bool updateLandmarks = false;

    [SerializeField] private GameObject SingleLandMarkR;
    [SerializeField] private GameObject SingleLandMarkL;
    [SerializeField] private GameObject SingleLandMarkWristR;
    [SerializeField] private GameObject SingleLandMarkWristL;    
    [SerializeField] private GameObject SingleLandMarkThumbR;
    [SerializeField] private GameObject SingleLandMarkThumbL;


    private BaseRunner _baseRunner;
    private void Awake()
    {
        Instance = this;
        realSenseManager = RealSenseManagerFactory.GetManager();
        _baseRunner = GameObject.FindObjectOfType<BaseRunner>();
    }
    public void Start()
    {
        _baseRunner.Play();
        SingleLandMarkThumbL.SetActive(false);
        SingleLandMarkThumbR.SetActive(false);
        SingleLandMarkWristL.SetActive(false);
        SingleLandMarkWristR.SetActive(false);
    }


    public void adjustLandmarkPositionValuesR(List<NormalizedLandmark> landmarks)
    {
        var landmarkR = landmarks[8];
        var landmarkWrist = landmarks[0];
        var landmarkThumb = landmarks[4];
        _landmarkR = landmarkR;
        _landmarkWristR = landmarkWrist;
        _landmarkThumbR = landmarkThumb;
        updateLandmarks = true;
    }
    public void adjustLandmarkPositionValuesL(List<NormalizedLandmark> landmarks)
    {
        var landmarkL = landmarks[8];
        var landmarkWrist = landmarks[0];
        var landmarkThumb = landmarks[4];
        _landmarkL = landmarkL;
        _landmarkWristL = landmarkWrist;
        _landmarkThumbL = landmarkThumb;
        updateLandmarks = true;
    }

    public void hideLandmarksValuesR()
    {
        _landmarkR = null;
        _landmarkWristR = null;
        _landmarkThumbR = null;
        updateLandmarks = true;
    }
    public void hideLandmarksValuesL()
    {
        _landmarkL = null;
        _landmarkWristL = null;
        _landmarkThumbL = null;
        updateLandmarks = true;
    }


    private Vector3 AdjustLocation(NormalizedLandmark normalizedLandmark, (float x, float y, float z) wristLandMark, (float x, float y, float z) realsensePosition)
    {
        Vector3 position = new Vector3(
            (normalizedLandmark.x - wristLandMark.x)* scaleFactor - realsensePosition.x * movementFactor,
            ((normalizedLandmark.y - wristLandMark.y)* scaleFactor - realsensePosition.y * movementFactor) + yOffSet,
            90
            );
        return position;
    }


    private (float x, float y, float z) getPositionFromDepthTexture((float x, float y, float z) wrist)
    {
        var depthTexture = realSenseManager.GetDepthTexture();
        var colorIntrin = realSenseManager.GetIntrin();


        var x = 1280 - (int)(wrist.x * 1280);
        var y = 720 - (int)(wrist.y * 720);

        if (0 <= x && x < colorIntrin.width && 0 <= y && y < colorIntrin.height)
        {
            var vx = (x - colorIntrin.ppx) / colorIntrin.fx;
            var vy = (y - colorIntrin.ppy) / colorIntrin.fy;

            byte[] rawData = depthTexture.GetRawTextureData();

            int index = (int)y * depthTexture.width * 2 + (int)x * 2;

            ushort pixelValue = BitConverter.ToUInt16(rawData, index);
            var vz = pixelValue * 0.001f;

            return (vx, vy, vz);
        }


        return (0, 0, 0);
    }

    (float x, float y, float z) realsensePosition = (0, 0, 0);

    float lastz = 0;
    private void AdjustLandmarkPosition(NormalizedLandmark landmark, NormalizedLandmark landmarkWrist, GameObject landmarkObject, bool isRight)
    {

        (float x, float y, float z) wrist = (landmarkWrist.x, landmarkWrist.y, landmarkWrist.z);

        realsensePosition = getPositionFromDepthTexture(wrist);

        if (realsensePosition.z == 0)
        {
            realsensePosition.z = lastz;
        }
        else
        {
            lastz = realsensePosition.z;
        }


        if (landmarkObject== null)
        {
            Debug.LogError($"Landmark GameObject is missing.");
        }

        var normalizedLandmark = landmark;
        Vector3 position = AdjustLocation(normalizedLandmark, wrist, realsensePosition);

        landmarkObject.transform.position = position;
        
    }

    private bool leftCanClick = false;
    private bool rightCanClick = false;
    private void HandleClick(GameObject landmark, GameObject thumb, ref bool canClick)
    {
        float distance = Vector3.Distance(landmark.transform.position, thumb.transform.position);
        //Debug.Log(distance);
        if (distance < 0.5f)
        {
            if (canClick)
            {
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(landmark.transform.position);
                CanvasClick.Instance.SimulateClickAtScreenPoint(screenPoint);
                canClick = false; 
            }
        }
        else
        {
            canClick = true;
        }
    }

    private void Update()
    {
        if (!updateLandmarks)
        {
            return;
        }

        if (_landmarkL == null || _landmarkWristL == null)
        {
            SingleLandMarkL.SetActive(false);
            //SingleLandMarkWristL.SetActive(false);
            //SingleLandMarkThumbL.SetActive(false);
        }
        else
        {
            SingleLandMarkL.SetActive(true);
            //SingleLandMarkWristL.SetActive(true);
            //SingleLandMarkThumbL.SetActive(true);
            AdjustLandmarkPosition(_landmarkL.Value, _landmarkWristL.Value, SingleLandMarkL, false);
            //AdjustLandmarkPosition(_landmarkWristL.Value, _landmarkWristL.Value, SingleLandMarkWristL, false);
            AdjustLandmarkPosition(_landmarkThumbL.Value, _landmarkWristL.Value, SingleLandMarkThumbL, false);
        }

        if (_landmarkR == null || _landmarkWristR == null)
        {
            SingleLandMarkR.SetActive(false);
            //SingleLandMarkWristR.SetActive(false);
            //SingleLandMarkThumbR.SetActive(false);
        }
        else
        {
            SingleLandMarkR.SetActive(true);
            //SingleLandMarkWristR.SetActive(true);
            //SingleLandMarkThumbR.SetActive(true);
            AdjustLandmarkPosition(_landmarkR.Value, _landmarkWristR.Value, SingleLandMarkR, true);
            //AdjustLandmarkPosition(_landmarkWristR.Value, _landmarkWristR.Value, SingleLandMarkWristR, true);
            AdjustLandmarkPosition(_landmarkThumbR.Value, _landmarkWristR.Value, SingleLandMarkThumbR, true);
        }

        HandleClick(SingleLandMarkL, SingleLandMarkThumbL, ref leftCanClick);
        HandleClick(SingleLandMarkR, SingleLandMarkThumbR, ref rightCanClick);


        updateLandmarks = false;
    }

    private void OnApplicationQuit()
    {
        realSenseManager.Dispose();
    }

}
