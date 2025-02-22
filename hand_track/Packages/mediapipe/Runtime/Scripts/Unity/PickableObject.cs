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
        private Quaternion handRotationOffset;

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
        }

        private void Update()
        {
            Debug.DrawRay(transform.position, transform.forward, UnityEngine.Color.yellow);
            Debug.DrawRay(transform.position, handRotationOffset * Vector3.forward, UnityEngine.Color.green);
        }
        public void PickUp(Quaternion handRotation)
        {
            isPickedUp = true;
            handRotationOffset = handRotation;  // Calculate rotation offset with 90 degrees on z-axis
            Debug.Log($"{gameObject.name} has been picked up.");
            // turn off rigidbody
            this.GetComponent<Rigidbody>().isKinematic = true;
        }

        public void Drop()
        {
            isPickedUp = false;
            Debug.Log($"{gameObject.name} has been dropped.");
            // turn on rigidbody
            this.GetComponent<Rigidbody>().isKinematic = false;
        }

        public void MoveToPosition(Vector3 newPosition, Quaternion handRotation)
        {
            if (isPickedUp)
            {
                transform.position = Vector3.Lerp(transform.position, newPosition, .8f);
                //transform.position = newPosition;


                // look at handrotation handRotation * Vector3.forward
                Quaternion newRot = Quaternion.LookRotation(handRotation * Vector3.forward, Vector3.up);


                transform.rotation = newRot;

                Debug.DrawRay(transform.position, handRotation * Vector3.forward, UnityEngine.Color.magenta);

                Debug.DrawRay(transform.position, newRot * Vector3.forward, UnityEngine.Color.red);


            }
        }
    }
}
