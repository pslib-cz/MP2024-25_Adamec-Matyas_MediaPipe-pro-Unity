using System;
using System.Collections.Generic;

using Mathf = UnityEngine.Mathf;

namespace Mediapipe.Tasks.Components.Containers
{

  public readonly struct Landmark : IEquatable<Landmark>
  {
    private const float _LandmarkTolerance = 1e-6f;

    public readonly float x;
    public readonly float y;
    public readonly float z;

    public readonly float? visibility;

    public readonly float? presence;

    public readonly string name;


    internal Landmark(float x, float y, float z, float? visibility, float? presence, string name)
    {
      this.x = x;
      this.y = y;
      this.z = z;
      this.visibility = visibility;
      this.presence = presence;
      this.name = name;
    }

    internal Landmark(NativeLandmark nativeLandmark) : this(
      nativeLandmark.x, nativeLandmark.y, nativeLandmark.z,
#pragma warning disable IDE0004 // for Unity 2020.3.x
      nativeLandmark.hasVisibility ? (float?)nativeLandmark.visibility : null,
      nativeLandmark.hasPresence ? (float?)nativeLandmark.presence : null,
#pragma warning restore IDE0004 // for Unity 2020.3.x
      nativeLandmark.name
    )
    {
    }

#nullable enable
    public override bool Equals(object? obj) => obj is Landmark other && Equals(other);
#nullable disable

    bool IEquatable<Landmark>.Equals(Landmark other)
    {
      return Mathf.Abs(x - other.x) < _LandmarkTolerance &&
        Mathf.Abs(y - other.y) < _LandmarkTolerance &&
        Mathf.Abs(z - other.z) < _LandmarkTolerance;
    }

    public static bool operator ==(in Landmark lhs, in Landmark rhs) => lhs.Equals(rhs);
    public static bool operator !=(in Landmark lhs, in Landmark rhs) => !(lhs == rhs);


  }

  public readonly struct NormalizedLandmark : IEquatable<NormalizedLandmark>
  {
    private const float _LandmarkTolerance = 1e-6f;

    public readonly float x;
    public readonly float y;
    public readonly float z;
    public readonly float? visibility;
    public readonly float? presence;
    public readonly string name;

    internal NormalizedLandmark(float x, float y, float z, float? visibility, float? presence, string name)
    {
      this.x = x;
      this.y = y;
      this.z = z;
      this.visibility = visibility;
      this.presence = presence;
      this.name = name;
    }

    internal NormalizedLandmark(NativeNormalizedLandmark nativeLandmark) : this(
      nativeLandmark.x, nativeLandmark.y, nativeLandmark.z,
#pragma warning disable IDE0004 // for Unity 2020.3.x
      nativeLandmark.hasVisibility ? (float?)nativeLandmark.visibility : null,
      nativeLandmark.hasPresence ? (float?)nativeLandmark.presence : null,
#pragma warning restore IDE0004 // for Unity 2020.3.x
      nativeLandmark.name
    )
    {
    }

#nullable enable
    public override bool Equals(object? obj) => obj is NormalizedLandmark other && Equals(other);
#nullable disable

    bool IEquatable<NormalizedLandmark>.Equals(NormalizedLandmark other)
    {
      return Mathf.Abs(x - other.x) < _LandmarkTolerance &&
        Mathf.Abs(y - other.y) < _LandmarkTolerance &&
        Mathf.Abs(z - other.z) < _LandmarkTolerance;
    }

    public static bool operator ==(in NormalizedLandmark lhs, in NormalizedLandmark rhs) => lhs.Equals(rhs);
    public static bool operator !=(in NormalizedLandmark lhs, in NormalizedLandmark rhs) => !(lhs == rhs);


  }

  public readonly struct Landmarks
  {
    public readonly List<Landmark> landmarks;

    internal Landmarks(List<Landmark> landmarks)
    {
      this.landmarks = landmarks;
    }

    internal static void Copy(NativeLandmarks source, ref Landmarks destination)
    {
      var landmarks = destination.landmarks ?? new List<Landmark>((int)source.landmarksCount);
      landmarks.Clear();

      foreach (var nativeLandmark in source.AsReadOnlySpan())
      {
        landmarks.Add(new Landmark(nativeLandmark));
      }
      destination = new Landmarks(landmarks);
    }

  }

  public readonly struct NormalizedLandmarks
  {
    public readonly List<NormalizedLandmark> landmarks;

    internal NormalizedLandmarks(List<NormalizedLandmark> landmarks)
    {
      this.landmarks = landmarks;
    }


    internal static void Copy(NativeNormalizedLandmarks source, ref NormalizedLandmarks destination)
    {
      var landmarks = destination.landmarks ?? new List<NormalizedLandmark>((int)source.landmarksCount);
      landmarks.Clear();

      foreach (var nativeLandmark in source.AsReadOnlySpan())
      {
        landmarks.Add(new NormalizedLandmark(nativeLandmark));
      }
      destination = new NormalizedLandmarks(landmarks);
    }

  }

  internal static class NativeLandmarksArrayExtension
  {


    public static void FillWith(this List<NormalizedLandmarks> target, NativeNormalizedLandmarksArray source)
    {
      target.ResizeTo(source.size);

      var i = 0;
      foreach (var nativeLandmarks in source.AsReadOnlySpan())
      {
        var landmarks = target[i];
        NormalizedLandmarks.Copy(nativeLandmarks, ref landmarks);
        target[i++] = landmarks;
      }
    }
  }
}
