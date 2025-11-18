using System.Collections;
using Systems.EventSystem;
using Systems.SceneLoader;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SceneLoader
{
    public delegate void OnLoadingStarted();

    public delegate void OnLoadingEnding();

    public class LoadingScreen : MonoBehaviour
    {
        private ILoadingData _data;

        private bool _isLoading;

        [FormerlySerializedAs("_canvas")] [Header("Canvas")] [SerializeField]
        private Canvas canvas;

        [SerializeField] private Slider slider;

        private void Awake()
        {
            slider.minValue = 0;
            slider.maxValue = 100;

            StartCoroutine(GetLoadingData());
        }

        private IEnumerator GetLoadingData()
        {
            while (!ServiceProvider.TryGetService(out _data))
                yield return null;

            CentralizedEventSystem eventSystem;
            while (!ServiceProvider.TryGetService(out eventSystem))
                yield return null;
            
            eventSystem.AddListener<OnLoadingStarted>(StartLoadingScreen);

            eventSystem.AddListener<OnLoadingEnding>(EndLoadingScreen);
        }

        private void StartLoadingScreen()
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
                progress = _data.GetCurrentLoadingProgress() != 0 ? _data.GetCurrentLoadingProgress() * 100.0f : 0;

                slider.value = progress;

                yield return null;
            }

            EndLoadingScreen();
        }

        private void EndLoadingScreen()
        {
            _isLoading = false;
            canvas?.gameObject.SetActive(false);
        }
    }
}