using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using mptcc = Mediapipe.Tasks.Components.Containers;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Drawing;


namespace Packages.mediapipe.Runtime.Scripts.Unity
{
    internal class HandExtension
    {

        HoldButton[] holdButtons;

        public HandExtension()
        {
            holdButtons = GameObject.FindObjectsOfType<HoldButton>();

        }

        public void UpdateLandmarks(GameObject[] Landmarks)
        {
            CheckForPickableObject(Landmarks);
            CheckForButtons(Landmarks);
        }

        private bool isObjectPickedUp = false;

        private void CheckForPickableObject(GameObject[] landmarks)
        {
            if (landmarks.Length > 8)  // Ensure the list has sufficient landmarks
            {
                Vector3 point4 = new Vector3(
                    landmarks[4].transform.position.x,
                    landmarks[4].transform.position.y,
                    landmarks[4].transform.position.z
                );
                Vector3 point8 = new Vector3(
                    landmarks[8].transform.position.x,
                    landmarks[8].transform.position.y,
                    landmarks[8].transform.position.z
                    );
                Vector3 midPoint = (point4 + point8) / 2;

                foreach (var pickableObject in GameObject.FindObjectsOfType<PickableObject>())
                {
                    float distance4 = Vector3.Distance(point4, pickableObject.transform.position);
                    float distance8 = Vector3.Distance(point8, pickableObject.transform.position);

                    if (distance4 + distance8 <= pickableObject.objectSize + pickableObject.objectMargin)
                    {
                        if (!isObjectPickedUp)
                        {
                            isObjectPickedUp = true;
                            pickableObject.PickUp();
                        }
                    }
                    else if (pickableObject.isPickedUp)
                    {
                        isObjectPickedUp = false;
                        pickableObject.Drop();
                    }

                    if (pickableObject.isPickedUp)
                    {
                        pickableObject.MoveToPosition(midPoint);
                    }
                }
            }

        }


        private void CheckForButtons(GameObject[] landmarks)
        {
            if (landmarks.Length > 8)
            {
                Vector3 indexFingerPosition = new Vector3(landmarks[8].transform.position.x, landmarks[8].transform.position.y, landmarks[8].transform.position.z);
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(indexFingerPosition);

                // Loop through each button.
                foreach (HoldButton btn in holdButtons)
                {
                    btn.UpdateHoldState(screenPoint);
                }
            }
        }
    }
}
