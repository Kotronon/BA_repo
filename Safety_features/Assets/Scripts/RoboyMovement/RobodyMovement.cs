using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.XR.MagicLeap;

public class RobodyMovement : MonoBehaviour
{
    /// <summary>
    /// Steering speed. Unit per second.
    /// </summary>
    private float steerSpeed = 4f;

    /// <summary>
    /// Rotating speed. Euler Angle per second.
    /// </summary>
    public float rotateSpeed = 30f;

    private bool _isSteering = true;
    private Vector3 _steerDirection;
    private Vector3 oldDirection;
    public GameObject emergencyStop;
    private int collide;
    public TMP_Text errorText;
    private MLInput.Controller.TouchpadGesture.GestureDirection touchdirection;
    private MLInput.Controller.TouchpadGesture.GestureDirection oldtouchdirection;
    private MLInput.Controller controller;
    private bool stopped;
    private void Start()
    {
        emergencyStop.SetActive(false);
        collide = 0;
        stopped = false;
        controller = MLInput.GetController(MLInput.Hand.Left);
        touchdirection = MLInput.Controller.TouchpadGesture.GestureDirection.None;
        oldtouchdirection = MLInput.Controller.TouchpadGesture.GestureDirection.None;
    }

    //private Direction _steerDirection = Direction.None;

    private void Update()
    {
        if (_isSteering && !stopped)
        {
            MLInput.Controller.TouchpadGesture.GestureDirection current = MLInput.Controller.TouchpadGesture.GestureDirection.None;
            if (controller.CurrentTouchpadGesture.Direction != MLInput.Controller.TouchpadGesture.GestureDirection.None)
                current = controller.CurrentTouchpadGesture.Direction;
           
            switch (current)
            {
                case MLInput.Controller.TouchpadGesture.GestureDirection.Up:
                    if (collide == 0 || touchdirection != MLInput.Controller.TouchpadGesture.GestureDirection.Up)
                    {
                        transform.position += transform.forward * steerSpeed * Time.deltaTime;
                        oldtouchdirection = touchdirection;
                        touchdirection = current;
                    }
                    break;
                case MLInput.Controller.TouchpadGesture.GestureDirection.Down:
                    if (collide == 0 || touchdirection != MLInput.Controller.TouchpadGesture.GestureDirection.Down)
                    {
                        transform.position -= transform.forward * steerSpeed * Time.deltaTime;
                        oldtouchdirection = touchdirection;
                        touchdirection = current;
                    }
                    break;
                case MLInput.Controller.TouchpadGesture.GestureDirection.Left:
                    if (collide == 0)
                    {
                        transform.Rotate(0.0f, -rotateSpeed * Time.deltaTime, 0.0f);
                        oldtouchdirection = touchdirection;
                        touchdirection = current;
                    }
                    break;
                case MLInput.Controller.TouchpadGesture.GestureDirection.Right:
                    if (collide == 0)
                    {
                        transform.Rotate(0.0f, +rotateSpeed * Time.deltaTime, 0.0f);
                        oldtouchdirection = touchdirection;
                        touchdirection = current;
                    }
                    break;
                default:
                    break;
            }
            
        }

    }

    public void Steer(MLInput.Controller.TouchpadGesture.GestureDirection direction)
    {
        if (direction == MLInput.Controller.TouchpadGesture.GestureDirection.None)
        {
            _isSteering = false;
        }
        else
        {
            _isSteering = true;

        }

       // _steerDirection = direction;
       touchdirection = oldtouchdirection;
    }

    public void Stop()
    {
        oldDirection = _steerDirection;
        Steer(MLInput.Controller.TouchpadGesture.GestureDirection.None);
        _isSteering = false;
    }

    public void Restart()
    {
        emergencyStop.SetActive(false);
        Steer(oldtouchdirection);
        _isSteering = true;
        stopped = false;
    }

    public void EmergencyStop()
    {
        Stop();
        stopped = true;
        emergencyStop.SetActive(true);
    }

     void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("DangerZone"))
        {
            collide++;
            errorText.text = "Error: You are touching the human zone.\n Please move in the opposite direction.";
        }
        
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("DangerZone"))
        {
            collide = 0;
            errorText.text = "Error:";
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (steerSpeed.Equals(4f)) steerSpeed = 2;
        else steerSpeed = 4;
    }
}
