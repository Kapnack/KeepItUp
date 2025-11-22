using System.Collections;
using Systems.EventSystem;
using Systems.SceneLoader;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SceneLoader
{
    public class LoadingScreen : MonoBehaviour
    {
        private ILoadingData _data;

        private bool _isLoading;

        [FormerlySerializedAs("_canvas")] [Header("Canvas")] [SerializeField]
        private Canvas canvas;

        [SerializeField] private Slider slider;

        private void Start()
        {
            _data = ServiceProvider.GetService<ILoadingData>();
            
            slider.minValue = 0;
            slider.maxValue = 100;
        }

        public void StartLoadingScreen()
        {
            canvas?.gameObject.SetActive(true);

            _isLoading = true;
            StartCoroutine(UpdateLoading());
        }

        private IEnumerator UpdateLoading()
        {
            float progress = 0;

            while (_isLoading || !Mathf.Approximately(progress, 100))
            {
                if (_data != null)
                {
                    progress = _data.GetCurrentLoadingProgress() != 0 ? _data.GetCurrentLoadingProgress() * 100.0f : 0;

                    slider.value = progress;
                }

                yield return null;
            }

            EndLoadingScreen();
        }

        public void EndLoadingScreen()
        {
            _isLoading = false;
            canvas?.gameObject.SetActive(false);
        }
    }
}