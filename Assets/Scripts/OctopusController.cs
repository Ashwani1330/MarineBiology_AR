using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class OctopusController : MonoBehaviour
{
    public FixedJoystick joystick;
    public float moveSpeed = 0.5f;
    
    private void Update()
    {
        Vector2 input = joystick.Direction;

        // Only move if there is a meaningful impact
        if (input.magnitude > 0.1f)
        {
            Vector3 moveDir = new Vector3(input.x, 0, input.y);
            transform.Translate(moveDir * (moveSpeed * Time.deltaTime), Space.World);
            
            // Rotate octopus towards movement direction
            if (moveDir != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 5f * Time.deltaTime);
            }
        }
    }
}