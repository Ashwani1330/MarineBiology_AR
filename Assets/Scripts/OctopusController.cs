using UnityEngine;
using UnityEngine.Splines;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody))]
public class OctopusController : MonoBehaviour
{
    public FixedJoystick joystick;
    public float moveSpeed = 2f;
    public float laneSwitchCooldown = 0.5f;
    public float laneSwitchDuration = 0.5f; // duration of smooth lane transition
    public SplineContainer[] splineContainers;

    private int currentSplineIndex = 4;
    private float splinePosition = 0f;

    private float lastLaneSwitchTime = -999f;
    private float laneLerpTime = 0f;

    private SplineContainer currentSpline;
    private SplineContainer targetSpline;
    private bool isSwitchingLanes = false;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        currentSpline = splineContainers[currentSplineIndex];
        targetSpline = currentSpline;
    }

    private void FixedUpdate()
    {
        Vector2 input = joystick.Direction;

        if (input.magnitude > 0.1f)
        {
            // Forward/backward
            float forwardInput = input.y;
            splinePosition += forwardInput * moveSpeed * Time.fixedDeltaTime;
            splinePosition = Mathf.Clamp01(splinePosition);

            // Handle lane switch input
            float horizontalInput = input.x;
            if (Mathf.Abs(horizontalInput) > 0.6f && Time.time - lastLaneSwitchTime > laneSwitchCooldown)
            {
                int desiredIndex = currentSplineIndex;

                if (horizontalInput > 0 && currentSplineIndex < splineContainers.Length - 1)
                    desiredIndex++;
                else if (horizontalInput < 0 && currentSplineIndex > 0)
                    desiredIndex--;

                if (desiredIndex != currentSplineIndex)
                {
                    targetSpline = splineContainers[desiredIndex];
                    currentSplineIndex = desiredIndex;
                    isSwitchingLanes = true;
                    laneLerpTime = 0f;
                    lastLaneSwitchTime = Time.time;
                }
            }

            // Evaluate positions on splines
            Vector3 posCurrent = currentSpline.EvaluatePosition(splinePosition);
            Vector3 posTarget = targetSpline.EvaluatePosition(splinePosition);
            Vector3 tangentTarget = targetSpline.EvaluateTangent(splinePosition);

            Vector3 finalPos;

            if (isSwitchingLanes)
            {
                laneLerpTime += Time.fixedDeltaTime / laneSwitchDuration;
                finalPos = Vector3.Lerp(posCurrent, posTarget, laneLerpTime);

                if (laneLerpTime >= 1f)
                {
                    currentSpline = targetSpline;
                    isSwitchingLanes = false;
                }
            }
            else
            {
                finalPos = posTarget;
            }

            // Move and rotate
            _rb.MovePosition(finalPos);

            if (tangentTarget != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(tangentTarget, Vector3.up);
                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, toRotation, 5f * Time.fixedDeltaTime));
            }
        }
    }
}