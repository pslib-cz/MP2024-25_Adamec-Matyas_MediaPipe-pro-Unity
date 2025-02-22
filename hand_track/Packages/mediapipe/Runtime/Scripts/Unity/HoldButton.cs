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

    enum ButtonAnimStyle
    {
        Up,
        Down,
        Left,
        Right,
        Whole,
        Central
    }

    enum ButtonPressStyle
    {
        Left,
        Right,
        Both,
        Any
    }

    public class HoldButton : MonoBehaviour
    {
        [SerializeField]
        public float holdTime = 1f;

        [SerializeField]
        private ParticleSystem myParticleEffect;

        [SerializeField]
        private ButtonAnimStyle buttonAnimStyle;

        [SerializeField]
        private Color pressColor = Color.green;

        private float holdTimer = 0f;
        private bool isHolding = false;

        [SerializeField]
        private Image buttonImage;
        private Color originalColor;

        [SerializeField] public string buttonText = "";



        public void UpdateHoldState(Vector3 indexFingerPosition)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(indexFingerPosition);

            RectTransform rectTransform = GetComponent<RectTransform>();
            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            Camera uiCamera = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
            {
                uiCamera = canvas.worldCamera;
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, uiCamera))
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

            Text text = GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = buttonText;
            }

            if (buttonImage != null)
            {
                originalColor = buttonImage.color;
            }

            switch (buttonAnimStyle)
            {
                case ButtonAnimStyle.Whole:
                    buttonImage.fillAmount = 100;
                    break;

                case ButtonAnimStyle.Up:
                    buttonImage.fillMethod = Image.FillMethod.Vertical;
                    buttonImage.fillOrigin = (int)Image.OriginVertical.Bottom;
                    buttonImage.color = pressColor;
                    buttonImage.fillAmount = 0;
                    break;

                case ButtonAnimStyle.Down:
                    buttonImage.fillMethod = Image.FillMethod.Vertical;
                    buttonImage.fillOrigin = (int)Image.OriginVertical.Top;
                    buttonImage.color = pressColor;
                    buttonImage.fillAmount = 0;
                    break;

                case ButtonAnimStyle.Left:
                    buttonImage.fillMethod = Image.FillMethod.Horizontal;
                    buttonImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                    buttonImage.color = pressColor;
                    buttonImage.fillAmount = 0;
                    break;

                case ButtonAnimStyle.Right:
                    buttonImage.fillMethod = Image.FillMethod.Horizontal;
                    buttonImage.fillOrigin = (int)Image.OriginHorizontal.Right;
                    buttonImage.color = pressColor;
                    buttonImage.fillAmount = 0;

                    break;

                case ButtonAnimStyle.Central:
                    buttonImage.color = pressColor;
                    buttonImage.fillAmount = 100;
                    break;
            }
        }

        private void UpdateButtonVisuals(float progress)
        {
            if (buttonImage != null)
            {
                switch (buttonAnimStyle)
                {
                    case ButtonAnimStyle.Whole:
                        buttonImage.color = Color.Lerp(originalColor, pressColor, progress);
                        break;

                    case ButtonAnimStyle.Up:
                        buttonImage.fillAmount = progress;
                        break;

                    case ButtonAnimStyle.Down:
                        buttonImage.fillAmount = progress;
                        break;

                    case ButtonAnimStyle.Left:
                        buttonImage.fillAmount = progress;
                        break;

                    case ButtonAnimStyle.Right:
                        buttonImage.fillAmount = progress;
                        break;

                    case ButtonAnimStyle.Central:
                        buttonImage.rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
                        break;
                }
            }
        }

        private void OnHoldComplete()
        {
            //trigger the button click
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke(); // Invoke the onClick event
            }

            Debug.Log("Hold action triggered!");
            myParticleEffect.Play();
            StartCoroutine(StopParticleAfterDelay(0.5f));
        }


        private System.Collections.IEnumerator StopParticleAfterDelay(float delay)
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(delay);

            // Stop the particle system
            myParticleEffect.Stop();
        }
    }
}
