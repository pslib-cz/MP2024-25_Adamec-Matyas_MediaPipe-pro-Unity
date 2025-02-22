using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Packages.mediapipe.Runtime.Scripts.Unity
{
    internal class PickableObject : MonoBehaviour
    {
        public bool isPickedUp = false;

        public void PickUp()
        {
            isPickedUp = true;
            Debug.Log($"{gameObject.name} has been picked up.");
        }

        public void Drop()
        {
            isPickedUp = false;
            Debug.Log($"{gameObject.name} has been dropped.");
        }

        public void MoveToPosition(Vector3 newPosition)
        {
            if (isPickedUp)
            {
                transform.position = newPosition;
            }
        }
    }
}
