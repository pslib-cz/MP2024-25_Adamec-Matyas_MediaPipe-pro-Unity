using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Packages.mediapipe.Runtime.Scripts.Unity
{
    internal class PickableObject : MonoBehaviour
    {
        [SerializeField] public float objectSize = .2f;
        [SerializeField] public float objectMargin = .1f;
        public bool isPickedUp = false;

        [SerializeField] public GameObject indicator;

        private void Start()
        {
            if (objectSize <= 0)
            {
                objectSize = .2f;
            }
            if (objectMargin <= 0)
            {
                objectMargin = .1f;
            }

            transform.localScale = new Vector3(objectSize, objectSize, objectSize);
            indicator.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }

        public void PickUp()
        {
            isPickedUp = true;
        }

        public void Drop()
        {
            isPickedUp = false;
        }

        public void MoveToPosition(Vector3 newPosition)
        {
            if (isPickedUp)
            {
                transform.position = Vector3.Lerp(transform.position, newPosition, .8f);
                transform.position = newPosition;
                indicator.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            }
        }
    }
}
