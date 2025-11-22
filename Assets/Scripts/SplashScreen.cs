using System;
using System.Collections;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private GameObject logo;
    [SerializeField] private float duration = 5f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float minScale = 0.8f;

    [NonSerialized] public Action Callback;

    private Vector3 _originalLogoScale;

    private void Awake()
    {
        _originalLogoScale = logo.transform.localScale;
    }

    private void Start()
    {
        StartCoroutine(EscaladorLogo());
    }

    private IEnumerator EscaladorLogo()
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float factor = Mathf.PingPong(time, duration / 2f) / (duration / 2f);
            float scale = Mathf.Lerp(minScale, maxScale, factor);

            logo.transform.localScale = _originalLogoScale * scale;
            
            yield return null;
        }

        logo.transform.localScale = _originalLogoScale;
        
        gameObject.SetActive(false);
        Callback?.Invoke();
    }
}