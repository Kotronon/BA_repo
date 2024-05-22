using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class RobodyMovement : MonoBehaviour
{
    /// <summary>
    /// Steering speed. Unit per second.
    /// </summary>
    public float steerSpeed = 4f;

    /// <summary>
    /// Rotating speed. Euler Angle per second.
    /// </summary>
    public float rotateSpeed = 30f;

    private bool _isSteering = true;
    private Vector3 _steerDirection;
    private Vector3 oldDirection;
    public GameObject emergencyStop;
    private int collide;
    private int collideOld;

    private void Start()
    {
        emergencyStop.SetActive(false);
        collide = 0;
        collideOld = 0;
    }

    //private Direction _steerDirection = Direction.None;

    private void Update()
    {
        if (_isSteering)
        {

            if (_steerDirection == Vector3.forward || Input.GetKeyDown(KeyCode.UpArrow) && collide == collideOld)
            {
                transform.position += transform.forward * steerSpeed * Time.deltaTime;
                collide = 0;
                collideOld = 0;
            }
            if (_steerDirection == Vector3.back || Input.GetKeyDown(KeyCode.DownArrow))
                transform.position += -transform.forward * steerSpeed * Time.deltaTime;
            if (_steerDirection == Vector3.left || Input.GetKeyDown(KeyCode.LeftArrow))
                transform.Rotate(0.0f, -rotateSpeed * Time.deltaTime, 0.0f);
            if (_steerDirection == Vector3.right || Input.GetKeyDown(KeyCode.RightArrow))
                transform.Rotate(0.0f, rotateSpeed * Time.deltaTime, 0.0f);
        }

    }

    public void Steer(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            _isSteering = false;
        }
        else
        {
            _isSteering = true;

        }

        _steerDirection = direction;
    }

    public void Stop()
    {
        oldDirection = _steerDirection;
        Steer(Vector3.zero);
        _isSteering = false;
    }

    public void Restart()
    {
        emergencyStop.SetActive(false);
        Steer(oldDirection);
        _isSteering = true;
    }

    public void EmergencyStop()
    {
        Stop();
        emergencyStop.SetActive(true);
    }

    public void OnCollisionEnter(Collision other)
    {
        collideOld = collide;
        collide++;
    }
}
