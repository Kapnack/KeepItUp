using Systems.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


public delegate void IncreaseDifficultyTime();

public class Timer : MonoBehaviour, ITimer
{
    private TMP_Text _timerText;
    private string _timerFormat;
    private CentralizedEventSystem _eventSystem;
    private float _changeDifficultyTime;
    [SerializeField] private float timerTime;
    [SerializeField] private float increaseDifficultyTimeLaps = 10.0f;
    public event TimeIsUp TimeIsUp;

    private float TimerTime
    {
        get => timerTime;

        set
        {
            if (Mathf.Approximately(timerTime, value))
                return;

            timerTime = value;

            if (value < 0f)
            {
                TimeIsUp?.Invoke();
                timerTime = 0;
            }

            UpdateTimerText();
        }
    }

    private void Awake()
    {
        _timerText = GetComponentInChildren<TMP_Text>();
        _timerFormat = _timerText.text;

        ServiceProvider.SetService<ITimer>(this, true);
        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();
    }

    private void Update()
    {
        if (timerTime < 0)
        {
            TimeIsUp?.Invoke();
            return;
        }

        TimerTime -= Time.deltaTime;

        if (timerTime < _changeDifficultyTime)
        {
            _changeDifficultyTime = timerTime - increaseDifficultyTimeLaps;
            _eventSystem.Get<IncreaseDifficultyTime>()?.Invoke();
        }
    }

    private void OnDestroy()
    {
        _eventSystem.Unregister<IncreaseDifficultyTime>();
    }

    private void UpdateTimerText()
    {
        timerTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(timerTime / 60);
        int seconds = Mathf.FloorToInt(timerTime % 60);

        _timerText.text = string.Format(_timerFormat, minutes, seconds);
    }

    public void SetUpTimer(float time)
    {
        timerTime = time;
        UpdateTimerText();
        _changeDifficultyTime = time - increaseDifficultyTimeLaps;
    }
}

public delegate void TimeIsUp();

public interface ITimer
{
    public event TimeIsUp TimeIsUp;
    public void SetUpTimer(float time);
}