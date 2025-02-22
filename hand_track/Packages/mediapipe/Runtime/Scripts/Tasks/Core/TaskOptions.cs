using UnityEngine;

namespace Mediapipe.Tasks.Core
{
  internal interface ITaskOptions
  {
    CalculatorOptions ToCalculatorOptions();
  }
}
