using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Tasks.Components.Containers;
using System.Threading;
using RealSenseManager;
using System;

public class HandManager : MonoBehaviour
{
    public static HandManager Instance;
    private IRealSenseManager realSenseManager;

    [SerializeField] private GameObject SwordL;
    [SerializeField] private GameObject SwordR;

    [SerializeField] private GameObject[] landmarksR;
    [SerializeField] private GameObject[] landmarksL;
    private List<NormalizedLandmark> _landmarksR = null;
    private List<NormalizedLandmark> _landmarksL = null;
    private float scaleFactor = 8f;
    private float movementFactor = 8f;
    private float yOffSet = 3.0f;

    private bool updateLandmarks = false;

    [SerializeField] private GameObject SingleLandMarkR;
    [SerializeField] private GameObject SingleLandMarkL;

    private void Awake()
    {
        Instance = this;
        realSenseManager = RealSenseManagerFactory.GetManager();

    }

    private void Start()
    {
        if (landmarksR.Length != 21 || landmarksL.Length != 21)
        {
            Debug.LogError("Invalid landmark data. Expected 21 landmarks.");
            return;
        }

    }

    public void adjustLandmarkPositionValuesR(List<NormalizedLandmark> landmarksR)
    {
        _landmarksR = landmarksR;
        updateLandmarks = true;
    }
    public void adjustLandmarkPositionValuesL(List<NormalizedLandmark> landmarksL)
    {
        _landmarksL = landmarksL;
        updateLandmarks = true;
    }

    public void hideLandmarksValuesR()
    {
        _landmarksR = null;
        updateLandmarks = true;
    }
    public void hideLandmarksValuesL()
    {
        _landmarksL = null;
        updateLandmarks = true;
    }


    private Vector3 AdjustLocation(NormalizedLandmark normalizedLandmark, (float x, float y, float z) wristLandMark, (float x, float y, float z) realsensePosition)
    {
        Vector3 position = new Vector3(
            (((normalizedLandmark.x - wristLandMark.x)) * scaleFactor) - realsensePosition.x * movementFactor,
            ((normalizedLandmark.y - wristLandMark.y) * scaleFactor - realsensePosition.y * movementFactor) + yOffSet,
            ((1 - (normalizedLandmark.z - wristLandMark.z)) * scaleFactor) - scaleFactor + 2.2f //- realsensePosition.z * movementFactor
            );
        return position;
    }

    private void HideSignleLandmark(GameObject landmarkObject)
    {
        if (landmarkObject != null)
        {
            landmarkObject.SetActive(false);
        }
    }

    private (float x, float y, float z) getPositionFromDepthTexture((float x, float y, float z) wrist, bool isRight)
    {
        var depthTexture = realSenseManager.GetDepthTexture();
        var colorIntrin = realSenseManager.GetIntrin();
        Debug.Log(colorIntrin.ppy + " " + colorIntrin.ppx + " " + colorIntrin.fx + " " + colorIntrin.fy + " " + colorIntrin.width + " " + colorIntrin.height);
        if (depthTexture == null)
        {
            Debug.LogError("Depth Texture is null");
            return (0, 0, 0);
        }

        // pixel coordinates of the wrist
        var x = 1280 - (int)(wrist.x * 1280);
        var y = 720 - (int)(wrist.y * 720);

        if (0 <= x && x < colorIntrin.width && 0 <= y && y < colorIntrin.height)
        {
            var vx = (x - colorIntrin.ppx) / colorIntrin.fx;
            var vy = (y - colorIntrin.ppy) / colorIntrin.fy;

            // Get the raw texture data
            byte[] rawData = depthTexture.GetRawTextureData();

            // Calculate the index of the pixel
            int index = (int)y * depthTexture.width * 2 + (int)x * 2;

            // Convert the byte data to ushort
            ushort pixelValue = BitConverter.ToUInt16(rawData, index);
            var vz = pixelValue * 0.001f;

            return (vx, vy, vz);
        }


        return (0, 0, 0);
    }

    (float x, float y, float z) realsensePosition = (0, 0, 0);

    float lastz = 0;
    private void AdjustLandmarkPosition(List<NormalizedLandmark> landmarks, GameObject[] landmarkObjects, bool isRight)
    {

        if (landmarks == null || landmarks.Count != 21)
        {
            Debug.LogError("Invalid landmark data. Expected 21 landmarks.");
            return;
        }


        var wristLandMark = (landmarks[0].x, landmarks[0].y, landmarks[0].z);
        realsensePosition = getPositionFromDepthTexture(wristLandMark, isRight);

        if (realsensePosition.z == 0)
        {
            realsensePosition.z = lastz;
        }
        else
        {
            lastz = realsensePosition.z;
        }


        for (int i = 0; i < landmarkObjects.Length; i++)
        {
            landmarkObjects[i].SetActive(true);
            if (landmarkObjects[i] == null)
            {
                Debug.LogError($"Landmark GameObject at index {i} is missing.");
                continue;
            }

            var normalizedLandmark = landmarks[i];
            Vector3 position = AdjustLocation(normalizedLandmark, wristLandMark, realsensePosition);

            landmarkObjects[i].transform.position = position;
        }
    }

    private void HideLandmarks(GameObject[] landmarks)
    {
        for (int i = 0; i < landmarks.Length; i++)
        {
            if (landmarks[i] != null)
            {
                landmarks[i].SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!updateLandmarks)
        {
            return;
        }

        if (_landmarksL == null)
        {

            HideLandmarks(landmarksL);
        }
        else
        {

            AdjustLandmarkPosition(_landmarksL, landmarksL, false);
        }

        if (_landmarksR == null)
        {

            HideLandmarks(landmarksR);
        }
        else
        {
            AdjustLandmarkPosition(_landmarksR, landmarksR, true);
        }

        updateLandmarks = false;
    }
}
