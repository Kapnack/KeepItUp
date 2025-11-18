using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PluginTest : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Button sendButton;
    [SerializeField] private Button updateButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button returnButton;

#if UNITY_ANDROID && !UNITY_EDITOR
    private const string PluginClassname = "com.example.wavelogger.WaveLogger";

    private AndroidJavaClass _pluginClass;
    private AndroidJavaObject _pluginInstance;
    private const string DefaultText = "No logs Updated";
#endif

    private void Awake()
    {
        sendButton.onClick.AddListener(SendLog);
        updateButton.onClick.AddListener(UpdateLogs);
        returnButton.onClick.AddListener(ReturnButton);
        clearButton.onClick.AddListener(ClearLogs);

#if UNITY_ANDROID && !UNITY_EDITOR
        label.text = DefaultText;
        
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

        Application.logMessageReceived += ForwardUnityLog;

        _pluginInstance = _pluginClass.CallStatic<AndroidJavaObject>("GetInstance");
#else
        label.text = "LOGS are not available for this version of the game";
#endif
    }

    private void ForwardUnityLog(string logString, string stackTrace, LogType type)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    if (_pluginInstance != null)
    {
        _pluginInstance.Call("SendLog", $"{type}: {logString}");
    }
#endif
    }

    private void SendLog()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("Unity -  SendLog");
        _pluginInstance.Call("SendLog", Time.time.ToString(CultureInfo.InvariantCulture));
#endif
    }

    private void UpdateLogs()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("Unity -  UpdateLogs");
        label.text = _pluginInstance.Call<string>("GetLogs");
        LayoutRebuilder.ForceRebuildLayoutImmediate(label.rectTransform);
#endif
    }

    private void ClearLogs()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        _pluginInstance.Call("ClearLogs");
        label.text = DefaultText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(label.rectTransform);
#endif
    }

    private void ReturnButton()
    {
        gameObject.SetActive(false);
    }
}