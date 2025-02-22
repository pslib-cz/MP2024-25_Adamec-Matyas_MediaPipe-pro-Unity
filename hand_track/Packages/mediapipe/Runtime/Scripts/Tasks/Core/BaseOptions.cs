namespace Mediapipe.Tasks.Core
{
  public sealed class BaseOptions
  {

    public string modelAssetPath { get; } = string.Empty;
    public byte[] modelAssetBuffer { get; } = null;

    public BaseOptions(string modelAssetPath = null, byte[] modelAssetBuffer = null)
    {
      this.modelAssetPath = modelAssetPath;
      this.modelAssetBuffer = modelAssetBuffer;
    }

    private Proto.Acceleration acceleration
    {
      get
      {
            return new Proto.Acceleration
            {
                Tflite = new InferenceCalculatorOptions.Types.Delegate.Types.TfLite { },
            };
        }
    }

    private Proto.ExternalFile modelAsset
    {
      get
      {
        var file = new Proto.ExternalFile { };

        if (modelAssetPath != null)
        {
          file.FileName = modelAssetPath;
        }
        if (modelAssetBuffer != null)
        {
          file.FileContent = Google.Protobuf.ByteString.CopyFrom(modelAssetBuffer);
        }

        return file;
      }
    }

    internal Proto.BaseOptions ToProto()
    {
      return new Proto.BaseOptions
      {
        ModelAsset = modelAsset,
        Acceleration = acceleration,
      };
    }
  }
}
