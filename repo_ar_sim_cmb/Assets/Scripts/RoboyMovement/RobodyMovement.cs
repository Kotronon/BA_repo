using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
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
    private float rotateSpeed = 30f;

    private bool _isSteering = false;
    private Direction _steerDirection = Direction.None;
    private Direction _oldSteerDirection = Direction.None;

    public GameObject emergencyStop;
    private int collide;
    private int collideOld;
    private bool forward;
    public TMP_Text errorText;

    private void Start()
    {
        emergencyStop.SetActive(false);
        collide = 0;
        collideOld = 0;
        forward = false;
    }

    private void Update()
    {
        if (_isSteering)
        {
            switch (_steerDirection)
            {
                //on�y moves if it is allowed to move
                case Direction.Forward:
                    if (!forward && collide == collideOld)
                    {
                        transform.position += transform.forward * steerSpeed * Time.deltaTime;
                        forward = true;
                    }
                    break;
                case Direction.Back:
                    if (forward && collide == collideOld)
                    {
                        transform.position -= transform.forward * steerSpeed * Time.deltaTime;
                        forward = true;
                    }
                    break;
                case Direction.Left:
                    if (collide == collideOld)
                    {
                        transform.Rotate(0.0f, -rotateSpeed * Time.deltaTime, 0.0f);
                    }                    
                    break;
                case Direction.Right:
                    if (collide == collideOld)
                    {
                        transform.Rotate(0.0f, rotateSpeed * Time.deltaTime, 0.0f);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void Steer(Direction direction)
    {
        if (direction == Direction.None)
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
        _oldSteerDirection = _steerDirection;
        _steerDirection = Direction.None;
        Steer(Direction.None);
    }

    public void Restart()
    {
        emergencyStop.SetActive(false);
        Steer(_oldSteerDirection);
    }

    public void EmergencyStop()
    {
        Stop();
        emergencyStop.SetActive(true);
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("DangerZone"))
        {
            collideOld = collide;
            collide++;
            errorText.text = "Error: You are touching the human zone. Please move in the opposite direction.";
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (steerSpeed.Equals(4f)) steerSpeed = 2;
        else steerSpeed = 4;
    }
}
