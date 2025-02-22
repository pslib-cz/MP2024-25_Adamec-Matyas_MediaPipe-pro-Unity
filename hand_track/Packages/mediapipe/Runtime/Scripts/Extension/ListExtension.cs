using System.Collections.Generic;

namespace Mediapipe
{
  internal static class ListExtension
  {
    public static void ResizeTo<T>(this List<T> list, int size)
    {
      if (list.Count > size)
      {
        list.RemoveRange(size, list.Count - size);
      }

      var count = size - list.Count;
      for (var i = 0; i < count; i++)
      {
        list.Add(default);
      }
    }
  }
}
