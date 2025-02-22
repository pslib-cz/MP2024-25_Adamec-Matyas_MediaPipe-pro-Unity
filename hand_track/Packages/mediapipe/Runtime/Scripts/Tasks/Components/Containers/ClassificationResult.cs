using System.Collections.Generic;

namespace Mediapipe.Tasks.Components.Containers
{
  public readonly struct Classifications
  {
    public readonly List<Category> categories;

    public readonly int headIndex;
    public readonly string headName;

    internal Classifications(List<Category> categories, int headIndex, string headName)
    {
      this.categories = categories;
      this.headIndex = headIndex;
      this.headName = headName;
    }

    internal static void Copy(NativeClassifications source, ref Classifications destination)
    {
      var categories = destination.categories ?? new List<Category>((int)source.categoriesCount);
      categories.Clear();
      foreach (var nativeCategory in source.categories)
      {
        categories.Add(new Category(nativeCategory));
      }
      destination = new Classifications(categories, source.headIndex, source.headName);
    }

  }


  public readonly struct ClassificationResult
  {
    public readonly List<Classifications> classifications;

    public readonly long? timestampMs;

    internal ClassificationResult(List<Classifications> classifications, long? timestampMs)
    {
      this.classifications = classifications;
      this.timestampMs = timestampMs;
    }


    internal static void Copy(NativeClassificationResult source, ref ClassificationResult destination)
    {
      var classificationsList = destination.classifications ?? new List<Classifications>((int)source.classificationsCount);
      classificationsList.ResizeTo((int)source.classificationsCount);

      var i = 0;
      foreach (var nativeClassifications in source.classifications)
      {
        var classifications = classificationsList[i];
        Classifications.Copy(nativeClassifications, ref classifications);
        classificationsList[i++] = classifications;
      }

      destination = new ClassificationResult(classificationsList, source.hasTimestampMs ? (long?)source.timestampMs : null);
    }
  }
}
