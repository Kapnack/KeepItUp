using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class PluginManager : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private const string PluginClassname = "com.example.wavelogger.WaveLogger";

    private AndroidJavaClass _pluginClass;
    private AndroidJavaObject _pluginInstance;
#endif

    private void Awake()
    {
        ServiceProvider.SetService(this);

#if UNITY_ANDROID && !UNITY_EDITOR
        Application.logMessageReceived += ForwardUnityLog;
        
        Debug.Log("Unity - " + PluginClassname);

        _pluginClass = new AndroidJavaClass(PluginClassname);

        if (_pluginClass == null)
        {
            Debug.LogError("pluginClass is null! Check your class name or .aar setup.");
        }
        else
        {
            Debug.Log("pluginClass created successfully.");
        }

        _pluginInstance = _pluginClass.CallStatic<AndroidJavaObject>("GetInstance");
        
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
        Debug.Log("Unity -  UpdateLogs");
        return _pluginInstance.Call<string>("GetLogs");
#else
        return "";
#endif
    }

    public void ClearLogs()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        _pluginInstance.Call("ClearLogs");
#endif
    }
}