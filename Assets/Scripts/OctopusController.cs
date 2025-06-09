using UnityEngine;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody))]
public class OctopusController : MonoBehaviour
{
    public FixedJoystick joystick;
    public float moveSpeed = 2f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void FixedUpdate()
    {
        Vector2 input = joystick.Direction;

        if (input.magnitude > 0.1f)
        {
            Vector3 moveDir = new Vector3(input.x, 0, input.y).normalized;
            Vector3 targetPosition = _rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(targetPosition);

            if (moveDir != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, toRotation, 5f * Time.fixedDeltaTime));
            }
        }
    }
}