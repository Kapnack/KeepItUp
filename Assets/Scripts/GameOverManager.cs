using Systems.EventSystem;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private Button replayButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        CentralizedEventSystem eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();
        
        replayButton.onClick.AddListener(() => eventSystem.Get<LoadGameplayScene>()?.Invoke());
        mainMenuButton.onClick.AddListener(() => eventSystem.Get<LoadMainMenu>()?.Invoke());
    }
}