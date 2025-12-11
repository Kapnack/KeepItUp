using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class PluginManager : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private const string PluginClassname = "com.example.wavelogger.WaveLogger";

    private AndroidJavaObject _pluginInstance;
#endif

    private void Awake()
    {
        ServiceProvider.SetService(this);

#if UNITY_ANDROID && !UNITY_EDITOR
    Application.logMessageReceived += ForwardUnityLog;

    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    {
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        using (var pluginClass = new AndroidJavaClass(PluginClassname))
        {
            _pluginInstance = pluginClass.CallStatic<AndroidJavaObject>("GetInstance", activity);
        }
    }

    UpdateLogs();
#endif
    }

    public void ForwardUnityLog(string logString, string stackTrace, LogType type)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_pluginInstance == null)
            return;
    
        _pluginInstance.Call("SendLog", $"{type}: {logString}");
#endif
    }

    public void SendLog()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("Unity -  SendLog");
        _pluginInstance.Call("SendLog", Time.time.ToString(CultureInfo.InvariantCulture));
#endif
    }

    public string UpdateLogs()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return _pluginInstance.Call<string>("GetLogs");
#else
        return "";
#endif
    }

    public void ClearLogs()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            _pluginInstance.Call("ClearLogs", activity);
        }
#endif
    }
}