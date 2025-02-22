using System.Collections;

namespace Mediapipe.Unity
{
  public interface IResourceManager
  {
    public IEnumerator PrepareAssetAsync(string name, string uniqueKey, bool overwriteDestination = true);
  }
}
