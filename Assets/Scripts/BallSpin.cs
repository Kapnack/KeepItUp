using UnityEngine;

public class BallSpin : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float dir;
    private void Awake()
    {
        _rb = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        dir = _rb.linearVelocityX;
        
        transform.Rotate(new Vector3(0, 0, -_rb.linearVelocityX));
    }
}