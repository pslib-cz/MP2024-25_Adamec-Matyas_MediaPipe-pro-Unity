using System.Collections;
using UnityEngine;

namespace Mediapipe.Unity.Sample
{
    public abstract class MyTaskApi<TTask> : BaseRunner where TTask : Tasks.Vision.Core.BaseVisionTaskApi
    {
        private Coroutine _coroutine;
        protected TTask taskApi;

        public override void Play()
        {
            if (_coroutine != null)
            {
                Stop();
            }
            base.Play();
            _coroutine = StartCoroutine(Run());
        }

        public override void Pause()
        {
            base.Pause();
        }

        public override void Resume()
        {
            base.Resume();
        }

        public override void Stop()
        {
            base.Stop();
            StopCoroutine(_coroutine);
            taskApi?.Close();
            taskApi = null;
        }

        protected abstract IEnumerator Run();

    }
}
