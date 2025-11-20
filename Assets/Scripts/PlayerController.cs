using System.Collections;
using CustomInput;
using Systems.EventSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;

public class PlayerController : MonoBehaviour, IPlayerController
{
    private CustomInputSystem _inputSystem;

    public event Swipe OnSwipePerformed;
    public event Holding OnHolding;
    public event StopHolding OnStopHolding;
    public event Tilting OnTilting;

    private CentralizedEventSystem _eventSystem;

    [SerializeField] private float swipeResistance = 100;
    [SerializeField] private float waitingTillSwipe = 1.5f;

    private bool _isHolding;
    private Vector2 _initialPos;
    private float _updatePosTime;

    private void Awake()
    {
        _inputSystem = new CustomInputSystem();


#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        TouchSimulation.Enable();
#endif
    }

    private void Start()
    {
        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        _eventSystem.Register(OnSwipePerformed);
        _eventSystem.Register(OnHolding);
        _eventSystem.Register(OnStopHolding);
        _eventSystem.Register(OnTilting);
    }

    private void OnEnable()
    {
        _inputSystem.Player.Enable();

        var gyroscope = UnityEngine.InputSystem.Gyroscope.current;


        if (gyroscope != null)
        {
            Debug.Log($"GYROSCOPE: {gyroscope}");
            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
        }

        _inputSystem.Player.PrimaryContact.started += OnPressStarted;
        _inputSystem.Player.PrimaryContact.canceled += OnPressEnded;
        _inputSystem.Player.Pause.started += OnPauseStarted;
    }

    private void OnDisable()
    {
        _inputSystem.Player.Disable();

        var gyroscope = UnityEngine.InputSystem.Gyroscope.current;


        if (gyroscope != null)
        {
            Debug.Log($"GYROSCOPE: {gyroscope}");
            gyroscope.samplingFrequency = 16;
            InputSystem.DisableDevice(UnityEngine.InputSystem.Gyroscope.current);
        }

        _inputSystem.Player.PrimaryContact.started -= OnPressStarted;
        _inputSystem.Player.PrimaryContact.canceled -= OnPressEnded;
        _inputSystem.Player.Pause.started -= OnPauseStarted;
    }

    private void Update()
    {
#if UNITY_ANDROID
        float tilt = Mathf.Clamp(UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue().x * 3f, -1, 1f);
        _eventSystem?.Get<Tilting>()?.Invoke(tilt);
        Debug.Log("tilt: " + _inputSystem.Player.Tilt.ReadValue<Vector2>());
#endif

        if (!_isHolding)
            return;

        if (Time.time < _updatePosTime)
            return;

        _initialPos = GetInputPos();
        _updatePosTime = Time.time + waitingTillSwipe;
    }

    public Vector2 GetInputPos() =>
        _inputSystem.Player.PrimaryPosition.ReadValue<Vector2>();


    private void OnPressStarted(InputAction.CallbackContext context)
    {
        _initialPos = GetInputPos();
        _isHolding = true;

        _updatePosTime = Time.time + waitingTillSwipe;

        _eventSystem?.Get<Holding>()?.Invoke();
    }

    private void OnPressEnded(InputAction.CallbackContext context)
    {
        Vector2 currentPos = GetInputPos();
        Vector2 delta = currentPos - _initialPos;
        Vector2 direction = Vector2.zero;

        if (currentPos.y > _initialPos.y + 50)
        {
            if (Mathf.Abs(delta.y) > swipeResistance)
                direction.y = Mathf.Sign(delta.y);

            _eventSystem?.Get<Swipe>()?.Invoke(direction);
        }

        _eventSystem?.Get<StopHolding>()?.Invoke();
        _isHolding = false;
    }

    private void OnPauseStarted(InputAction.CallbackContext context)
    {
        _eventSystem?.Get<Pause>()?.Invoke();
    }
}

public delegate void Tilting(float tiltDir);

public delegate void Swipe(Vector2 direction);

public delegate void Holding();

public delegate void StopHolding();

public interface IPlayerController
{
    event Swipe OnSwipePerformed;
    event Holding OnHolding;
    event StopHolding OnStopHolding;
    event Tilting OnTilting;

    Vector2 GetInputPos();
}