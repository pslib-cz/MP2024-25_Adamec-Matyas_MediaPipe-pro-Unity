using System.Collections.Generic;
using Mediapipe.Tasks.Components.Containers;

namespace Mediapipe.Tasks.Vision.HandLandmarker
{

    public readonly struct HandLandmarkerResult
    {
        public readonly List<Classifications> handedness;
        public readonly List<NormalizedLandmarks> handLandmarks;
        internal HandLandmarkerResult(List<Classifications> handedness, List<NormalizedLandmarks> handLandmarks)
        {
            this.handedness = handedness;
            this.handLandmarks = handLandmarks;
        }
        public static HandLandmarkerResult Alloc(int capacity)
        {
            var handedness = new List<Classifications>(capacity);
            var handLandmarks = new List<NormalizedLandmarks>(capacity);
            var handWorldLandmarks = new List<Landmarks>(capacity);
            return new HandLandmarkerResult(handedness, handLandmarks);
        }
        public void CloneTo(ref HandLandmarkerResult destination)
        {
            if (handLandmarks == null)
            {
                destination = default;
                return;
            }

            var dstHandedness = destination.handedness ?? new List<Classifications>(handedness.Count);
            dstHandedness.Clear();
            dstHandedness.AddRange(handedness);

            var dstHandLandmarks = destination.handLandmarks ?? new List<NormalizedLandmarks>(handLandmarks.Count);
            dstHandLandmarks.Clear();
            dstHandLandmarks.AddRange(handLandmarks);


            destination = new HandLandmarkerResult(dstHandedness, dstHandLandmarks);
        }
    }
}
