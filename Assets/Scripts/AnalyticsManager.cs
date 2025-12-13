using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    private void Awake()
    {
        ServiceProvider.SetService(this);
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
    }

    public void SendData(CustomEvent dataEvent)
    {
        AnalyticsService.Instance.RecordEvent(dataEvent);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            AnalyticsService.Instance.Flush();
    }

    private void OnApplicationQuit()
    {
        AnalyticsService.Instance.Flush();
    }
}