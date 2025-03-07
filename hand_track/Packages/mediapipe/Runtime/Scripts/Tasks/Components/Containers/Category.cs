namespace Mediapipe.Tasks.Components.Containers
{

  public readonly struct Category
  {

    public readonly int index;

    public readonly float score;

    public readonly string categoryName;

    public readonly string displayName;

    internal Category(int index, float score, string categoryName, string displayName)
    {
      this.index = index;
      this.score = score;
      this.categoryName = categoryName;
      this.displayName = displayName;
    }

    internal Category(NativeCategory nativeCategory) : this(
      nativeCategory.index,
      nativeCategory.score,
      nativeCategory.categoryName,
      nativeCategory.displayName)
    {
    }

  }
}
