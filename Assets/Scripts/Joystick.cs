using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    public CharacterController player;
    public float speed;
    public float rotationSpeed = 100f;
    public float gravity = -9.81f;

    private Vector3 velocity; // Track downward velocity

    void Start()
    {
        player = GetComponent<CharacterController>();
    }

    void Update()
    {
        // left joystick
        var joystickAxis = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.LTouch);
        Vector3 position = (transform.right * joystickAxis.x + transform.forward * joystickAxis.y) * speed * Time.deltaTime;
        player.Move(position);

        // right joystick
        var rightJoystick = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.RTouch);
        float rotationAmount = rightJoystick.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotationAmount, 0);

        // gravity
        if (player.isGrounded)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        player.Move(velocity * Time.deltaTime);
    }
}