using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private Button returnButton;

    private void Awake()
    {
        returnButton.onClick.AddListener(OnReturn);
    }
    
    private void OnReturn()
    {
        gameObject.SetActive(false);
    }
}