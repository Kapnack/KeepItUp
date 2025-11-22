using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct PlatformSettings
{
    public float MaxAngleRotation;
    public float StateSpeed;
    public Vector2 MinSize;
    public Vector2 MaxSize;

    public PlatformSettings(float maxAngleRotation, float stateSpeed, Vector2 minSize, Vector2 maxSize)
    {
        MaxAngleRotation = maxAngleRotation;
        StateSpeed = stateSpeed;
        MinSize = minSize;
        MaxSize = maxSize;
    }
}

public class DifficultyManager : MonoBehaviour
{
    public Action<PlatformSettings> Callback;

    [SerializeField] private TMP_Text difficultySelected;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button accept;
    [SerializeField] private Button rules;
    [SerializeField] private Button closeRules;
    [SerializeField] private GameObject rulesCanvas;
    
    private PlatformSettings _difficulty;
    
    private void Awake()
    {
        easyButton.onClick.AddListener(OnEasyDifficulty);
        normalButton.onClick.AddListener(OnNormalDifficulty);
        hardButton.onClick.AddListener(OnHardDifficulty);
        rules.onClick.AddListener(RulesOpen);
        closeRules.onClick.AddListener(RulesClose);
            
        accept.onClick.AddListener(AcceptDifficulty);

        OnEasyDifficulty();
    }

    private void OnEasyDifficulty()
    {
        _difficulty = new PlatformSettings(0.0f, 1.0f, new Vector2(3f, 1f),  new Vector2(5f, 1f));
        difficultySelected.text = "Difficulty Selected: Easy";
        
        easyButton.image.color = Color.gray;
        normalButton.image.color = Color.white;
        hardButton.image.color = Color.white;
    }

    private void OnNormalDifficulty()
    {
        _difficulty = new PlatformSettings(20.0f, 1.5f, new Vector2(2.5f, 1f),  new Vector2(4.5f, 1f));
        difficultySelected.text = "Difficulty Selected: Normal";
        
        easyButton.image.color = Color.white;
        normalButton.image.color = Color.gray;
        hardButton.image.color = Color.white;
    }

    private void OnHardDifficulty()
    {
        _difficulty = new PlatformSettings(60.0f, 2.0f, new Vector2(2f, 1f),  new Vector2(4f, 1f));
        difficultySelected.text = "Difficulty Selected: Hard";
        
        easyButton.image.color = Color.white;
        normalButton.image.color = Color.white;
        hardButton.image.color = Color.gray;
    }

    private void RulesOpen()
    {
        rulesCanvas.SetActive(true);
    }
    
    private void RulesClose()
    {
        rulesCanvas.SetActive(false);
    }
    
    private void AcceptDifficulty()
    {
        Callback?.Invoke(_difficulty);
    }
}