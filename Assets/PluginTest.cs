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
    private PluginManager _pluginManager;

#if UNITY_ANDROID && !UNITY_EDITOR
    private const string DefaultText = "No logs";
#else
    private const string DefaultText = "LOGS are not available for this version of the game";
#endif

    public void SetUp()
    {
        _pluginManager = ServiceProvider.GetService<PluginManager>();

        sendButton.onClick.AddListener(_pluginManager.SendLog);
        updateButton.onClick.AddListener(OnUpdateLogs);
        clearButton.onClick.AddListener(OnClear);
        returnButton.onClick.AddListener(ReturnButton);

        label.text = DefaultText;

        OnUpdateLogs();
    }

    private void OnClear()
    {
        _pluginManager.ClearLogs();
        label.text = DefaultText;
    }

    private void OnUpdateLogs()
    {
        string logs = _pluginManager.UpdateLogs();
        label.text = logs != "" ? logs : DefaultText;
    }

    private void ReturnButton()
    {
        gameObject.SetActive(false);
    }
}