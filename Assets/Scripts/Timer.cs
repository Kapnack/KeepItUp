using Systems.EventSystem;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour, ITimer
{
    private TMP_Text _timerText;
    private string _timerFormat;

    [SerializeField] private float timerTime;
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
    }

    private void Update()
    {
        if (timerTime < 0)
        {
            TimeIsUp?.Invoke();
            return;
        }

        TimerTime -= Time.deltaTime;
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
    }
}

public delegate void TimeIsUp();

public interface ITimer
{
    public event TimeIsUp TimeIsUp;
    public void SetUpTimer(float time);
}