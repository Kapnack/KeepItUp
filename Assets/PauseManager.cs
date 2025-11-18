using Systems.EventSystem;
using UnityEngine;
using UnityEngine.UI;

public delegate void Pause();

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private GameObject pausePanel;

    private bool _isPaused;

    private void Awake()
    {
        returnButton.onClick.AddListener(SwitchPause);
        pauseButton.onClick.AddListener(SwitchPause);
        
        pausePanel?.SetActive(_isPaused);
    }

    private void OnEnable()
    {
        CentralizedEventSystem eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        eventSystem.AddListener<Pause>(SwitchPause);
    }

    private void OnDisable()
    {
        CentralizedEventSystem eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        eventSystem.RemoveListener<Pause>(SwitchPause);
    }

    private void SwitchPause()
    {
        _isPaused = !_isPaused;

        Time.timeScale = Time.timeScale == 0 ? 1.0f : 0;
        pausePanel?.SetActive(_isPaused);
    }
}