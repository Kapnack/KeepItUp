using Systems.EventSystem;
using UnityEngine;
using UnityEngine.UI;

public delegate void Pause();

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private GameObject pausePanel;

    private bool _isPaused;

    CentralizedEventSystem _eventSystem;
    
    private void Awake()
    {
        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();
        
        returnButton.onClick.AddListener(SwitchPause);
        pauseButton.onClick.AddListener(SwitchPause);
        mainMenuButton.onClick.AddListener(OnMainMenu);
        
        pausePanel?.SetActive(_isPaused);
    }

    private void OnEnable()
    {
        _eventSystem.AddListener<Pause>(SwitchPause);
    }

    private void OnDisable()
    {
        _eventSystem.RemoveListener<Pause>(SwitchPause);
    }

    private void SwitchPause()
    {
        _isPaused = !_isPaused;

        Time.timeScale = Time.timeScale == 0 ? 1.0f : 0;
        pausePanel?.SetActive(_isPaused);
    }

    private void OnMainMenu()
    {
        _eventSystem.Get<LoadMainMenu>()?.Invoke();
    }
}