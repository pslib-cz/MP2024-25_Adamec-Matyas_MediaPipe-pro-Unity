using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Packages.mediapipe.Runtime.Scripts.Unity
{


    public class HoldButton : MonoBehaviour
    {
        [SerializeField]
        public float holdTime = 4f; 
        
        [SerializeField]
        private Color pressColor = Color.green;

        private float holdTimer = 0f;
        private bool isHolding = false;

        [SerializeField]
        private Image buttonImage; 
        private Color originalColor;

        [SerializeField] public string buttonText = "";


        private RectTransform rectTransform;
        private Canvas canvas;

        public void UpdateHoldState(Vector3 screenPoint)
        {

            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, Camera.main))
            {
                if (!isHolding)
                {
                    isHolding = true;
                    holdTimer = 0f;
                }
                else
                {
                    holdTimer += Time.deltaTime;
                    UpdateButtonVisuals(holdTimer / holdTime);

                    if (holdTimer >= holdTime)
                    {
                        OnHoldComplete();
                        ResetHold();
                    }
                }
            }
            else
            {
                ResetHold();
            }
        }

        private void ResetHold()
        {
            isHolding = false;
            holdTimer = 0f;
            UpdateButtonVisuals(0f);
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = rectTransform.GetComponentInParent<Canvas>();

            Text text = GetComponentInChildren<Text>();
            text.text = buttonText;
            text.color = pressColor;

            if (buttonImage != null)
            {
                originalColor = buttonImage.color;

                buttonImage.fillAmount = 0;

                buttonImage.color = pressColor;
                buttonImage.fillMethod = Image.FillMethod.Radial360;

                buttonImage.fillOrigin = 2;
            };
        }


        private void UpdateButtonVisuals(float progress)
        {
            if (buttonImage == null) return;

            buttonImage.fillAmount = progress;
        }


        private void OnHoldComplete()
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke(); // Invoke the onClick event
            }
        }


    }
}
