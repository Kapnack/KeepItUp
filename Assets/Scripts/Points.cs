using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Points : MonoBehaviour, IPointsSetUp
{
    [SerializeField] private int score;
    [SerializeField] private int duration;

    private Action _onReturn;

    private float _totalLifeTime;

    private void Awake()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        
        col.isTrigger = true;
    }
    
    private void Update()
    {
        if (Time.time > _totalLifeTime)
            _onReturn?.Invoke();
    }

    public void SetUp(Action onReturn = null)
    {
        _onReturn = onReturn;
        _totalLifeTime = Time.time + duration;
    }

    public void SetUp(int lifeTime, Action onReturn = null)
    {
        duration = lifeTime;

        SetUp(onReturn);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !collision.TryGetComponent(out Player player)) 
            return;
        
        player.AddPoints(score);
        _onReturn?.Invoke();
    }
}

public interface IPointsSetUp
{
    public void SetUp(Action onReturn = null);
    
    public void SetUp(int lifeTime, Action onReturn = null);
}