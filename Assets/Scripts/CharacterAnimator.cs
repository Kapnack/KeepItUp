using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private static readonly int Moving = Animator.StringToHash("Moving");

    private IPlayerController _controller;
    private Rigidbody2D _rb;
    private Animator _animator;

    private bool _isMoving;
    private float _dir;

    private bool _shouldAnimate;

    private bool ShouldAnimate
    {
        get => _shouldAnimate;

        set
        {
            if (_shouldAnimate == value)
                return;

            _shouldAnimate = value;
            _animator.SetBool(Moving, value);
        }
    }

    private void Awake()
    {
        _controller = GetComponentInParent<IPlayerController>();
        _rb = GetComponentInParent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _controller.OnHolding += OnMoving;
        _controller.OnStopHolding += OnStopMoving;
    }

    private void OnDisable()
    {
        _controller.OnHolding -= OnMoving;
        _controller.OnStopHolding -= OnStopMoving;
    }

    private void FixedUpdate()
    {
        ShouldAnimate = _isMoving || _rb.linearVelocityX != 0;

        if (!ShouldAnimate)
            return;

        if (_isMoving)
        {
            _dir = _controller.GetInputPos().x > Screen.width * 0.5f ? 0 : 180;

            transform.rotation = Quaternion.Euler(0, _dir, 0);
        }
        else if (_rb.linearVelocityX != 0)
        {
            _dir = _rb.linearVelocityX > 0 ? 0 : 180;

            transform.rotation = Quaternion.Euler(0, _dir, 0);
        }
    }
    
    private void OnMoving() => _isMoving = true;
    private void OnStopMoving() => _isMoving = false;
}