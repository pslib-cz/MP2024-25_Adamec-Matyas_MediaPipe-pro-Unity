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
using UnityEngine.UIElements;


namespace Packages.mediapipe.Runtime.Scripts.Unity
{
    internal class HandExtension
    {

        private PickableObject _handObject = null;
        private float momentumFactor = 10f;

        private GameObject swordR;
        private GameObject swordL;


        public HandExtension(GameObject swordObjectR, GameObject swordObjectL)
        {
            swordR = swordObjectR;
            swordL = swordObjectL;
        }


        public void UpdateLandmarks(IReadOnlyList<mptcc.NormalizedLandmark> Landmarks, string Hand)
        {
            var isRightHand = Hand == "R";

            CheckForPickableObject(Landmarks);
            CheckForButtons(Landmarks);
            adjustSwordPosition(Landmarks, isRightHand);
        }

        public void adjustSwordPosition(IReadOnlyList<mptcc.NormalizedLandmark> landmarks, bool isRight)
        {
            if (isRight)
            {
                if (!swordR || landmarks.Count <= 17) return;

                Vector3 point5 = new Vector3(landmarks[5].x / momentumFactor, landmarks[5].y / momentumFactor, landmarks[5].z / momentumFactor);
                Vector3 point17 = new Vector3(landmarks[17].x / momentumFactor, landmarks[17].y / momentumFactor, landmarks[17].z / momentumFactor);

                Vector3 midPoint = (point5 + point17) / 2;
                Vector3 direction = (point17 - point5).normalized;
                Quaternion currentHandRotation = Quaternion.LookRotation(direction, Vector3.up);

                swordR.transform.position = Vector3.Lerp(swordR.transform.position, midPoint, 0.8f);
                swordR.transform.rotation = currentHandRotation;

                swordR.transform.Rotate(90, 0, 0);
            }
            else
            {

                if (!swordL || landmarks.Count <= 17) return;

                Vector3 point5 = new Vector3(landmarks[5].x / momentumFactor, landmarks[5].y / momentumFactor, landmarks[5].z / momentumFactor);
                Vector3 point17 = new Vector3(landmarks[17].x / momentumFactor, landmarks[17].y / momentumFactor, landmarks[17].z / momentumFactor);

                Vector3 midPoint = (point5 + point17) / 2;
                Vector3 direction = (point17 - point5).normalized;
                Quaternion currentHandRotation = Quaternion.LookRotation(direction, Vector3.up);

                swordL.transform.position = Vector3.Lerp(swordL.transform.position, midPoint, 0.8f);
                swordL.transform.rotation = currentHandRotation;

                swordL.transform.Rotate(90, 0, 0);
            }
        }


        private void CheckForPickableObject(IReadOnlyList<mptcc.NormalizedLandmark> landmarks)
        {
            if (landmarks.Count > 8)
            {
                Vector3 point4 = new Vector3(landmarks[4].x / momentumFactor, landmarks[4].y / momentumFactor, landmarks[4].z / momentumFactor / 2);
                Vector3 point8 = new Vector3(landmarks[8].x / momentumFactor, landmarks[8].y / momentumFactor, landmarks[8].z / momentumFactor / 2);
                Vector3 midPoint = (point4 + point8) / 2;
                Vector3 direction = (point4 - point8).normalized;
                Quaternion currentHandRotation = Quaternion.LookRotation(direction, Vector3.up);


                foreach (var pickableObject in GameObject.FindObjectsOfType<PickableObject>())
                {
                    Debug.Log(pickableObject.name + " " + pickableObject.transform.position + " " + pickableObject.objectSize + " " + pickableObject.objectMargin);
                    UnityEngine.Debug.DrawLine(point4, point8, UnityEngine.Color.red);
                    UnityEngine.Debug.DrawLine(point4, pickableObject.transform.position, UnityEngine.Color.green);
                    UnityEngine.Debug.DrawLine(point8, pickableObject.transform.position, UnityEngine.Color.green);

                    float distance4 = Vector3.Distance(point4, pickableObject.transform.position);
                    float distance8 = Vector3.Distance(point8, pickableObject.transform.position);

                    Debug.Log(distance8 + " " + distance4);
                    if (distance4 + distance8 <= pickableObject.objectSize + pickableObject.objectMargin)
                    {
                        pickableObject.PickUp(currentHandRotation);
                        _handObject = pickableObject;
                    }
                    else if (pickableObject.isPickedUp)
                    {
                        pickableObject.Drop();
                        _handObject = null;
                    }

                    if (pickableObject.isPickedUp)
                    {
                        pickableObject.MoveToPosition(midPoint, currentHandRotation);
                    }
                }
            }
            if (_handObject != null)
            {
                _handObject.Drop();
                _handObject = null;
            }

        }


        private void CheckForButtons(IReadOnlyList<mptcc.NormalizedLandmark> landmarks)
        {
            if (landmarks.Count > 8)
            {
                int offset = 13;
                Vector3 indexFingerPosition = new Vector3(landmarks[8].x / offset, landmarks[8].y / offset, landmarks[8].z / offset);

                HoldButton[] holdButtons = GameObject.FindObjectsOfType<HoldButton>();
                foreach (var holdButton in holdButtons)
                {
                    holdButton.UpdateHoldState(indexFingerPosition);
                }
            }
        }
    }
}
