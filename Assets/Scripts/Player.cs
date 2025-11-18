using Systems.EventSystem;
using UnityEngine;

public delegate void AddPoint(int points);

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;

    private IPlayerController _controller;
    private Rigidbody2D _rb;
    private CircleCollider2D _collider;
    private CentralizedEventSystem _eventSystem;
    
    private bool _isMoving;
    
    private void Awake()
    {
        _controller = GetComponent<IPlayerController>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();
    }

    private void OnEnable()
    {
        CentralizedEventSystem eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        eventSystem.AddListener<Swipe>(Jump);
        eventSystem.AddListener<Holding>(IsMoving);
        eventSystem.AddListener<StopHolding>(IsNotMoving);
    }

    private void OnDisable()
    {
        CentralizedEventSystem eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        eventSystem.RemoveListener<Swipe>(Jump);
        eventSystem.RemoveListener<Holding>(IsMoving);
        eventSystem.RemoveListener<StopHolding>(IsNotMoving);
    }

    private void IsMoving() => _isMoving = true;
    private void IsNotMoving() => _isMoving = false;

    private void Update()
    {
        if (!_isMoving)
            return;

        float dir = _controller.GetInputPos().x > Screen.width * 0.5f ? 1 : -1;

        _rb.AddForce(new Vector2(movementSpeed * dir * Time.deltaTime, 0),
            ForceMode2D.Force);
    }

    private void Jump(Vector2 dir)
    {
        if (dir.y > 0 && CanJump())
            _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private bool CanJump()
    {
        return Physics2D.CircleCast(transform.position, _collider.radius, Vector2.down, 1.0f,
            LayerMask.GetMask("Floor"));
    }

    public void AddPoints(int score)
    {
        _eventSystem.Get<AddPoint>()?.Invoke(score);
    }
}