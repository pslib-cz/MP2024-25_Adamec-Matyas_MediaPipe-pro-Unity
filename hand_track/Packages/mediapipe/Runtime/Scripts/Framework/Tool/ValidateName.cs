using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Mediapipe
{
  public static partial class Tool
  {
    private const string _NameRegex = "[a-z_][a-z0-9_]*";
    private const string _NumberRegex = "(0|[1-9][0-9]*)";
    private const string _TagRegex = "[A-Z_][A-Z0-9_]*";
    private static readonly string _TagAndNameRegex = $"({_TagRegex}:)?{_NameRegex}";
    private static readonly string _TagIndexNameRegex = $"({_TagRegex}:({_NumberRegex}:)?)?{_NameRegex}";
    private static readonly string _TagIndexRegex = $"({_TagRegex})?(:{_NumberRegex})?";

    public static void ParseTagAndName(string tagAndName, out string tag, out string name)
    {
      var v = tagAndName.Split(':');

      tag = v[0];
      name = v[1];
    }

    public static void ParseTagIndexName(string tagIndexName, out string tag, out int index, out string name)
    {
      var theIndex = 0;
      var v = tagIndexName.Split(':');

      tag = v[0];
      index = theIndex;
      name = v[1];
    }

    public static string ParseNameFromStream(string stream)
    {
        ParseTagIndexName(stream, out var _, out var _, out var name);
        return name;
    }
    }
}
