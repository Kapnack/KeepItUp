using System;
using System.Collections;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private GameObject Logo;
    [SerializeField] private float duracion = 5f;
    [SerializeField] private float escalaMax = 1.2f;
    [SerializeField] private float escalaMin = 0.8f;

    [NonSerialized] public Action callback;

    private Vector3 escalaOriginal;

    private MainMenuManager mainMenuManager;

    private void Awake()
    {
        mainMenuManager = GetComponentInParent<MainMenuManager>();
        escalaOriginal = Logo.transform.localScale;
    }

    private void Start()
    {
        StartCoroutine(EscaladorLogo());
    }

    private IEnumerator EscaladorLogo()
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            float factor = Mathf.PingPong(tiempo, duracion / 2f) / (duracion / 2f);
            float escala = Mathf.Lerp(escalaMin, escalaMax, factor);

            Logo.transform.localScale = escalaOriginal * escala;

            yield return null;
        }

        Logo.transform.localScale = escalaOriginal;
        gameObject.SetActive(false);
        callback?.Invoke();
    }
}