using System;
using System.Collections;
using System.Collections.Generic;
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
    private int collideOld;
    private bool forward;
    public TMP_Text errorText;
    private Vector3 position;
    private  Vector3 rotation;

    private void Start()
    {
        emergencyStop.SetActive(false);
        collide = 0;
        collideOld = 0;
        forward = false;
        position = transform.position;
        rotation = transform.eulerAngles;
    }

    //private Direction _steerDirection = Direction.None;

    private void Update()
    {
        if (_isSteering)
        {
            if (collide != collideOld)
            {
                if ((Input.GetKeyDown(KeyCode.UpArrow)) && !forward)
                {
                    transform.position += transform.forward * steerSpeed * Time.deltaTime;
                    position += transform.forward * steerSpeed * Time.deltaTime;
                    forward = true;
                    errorText.text = "";
                    collide = 0;
                    collideOld = 0 ;
                }

                if (( Input.GetKeyDown(KeyCode.DownArrow)) && forward)
                {
                    transform.position += -transform.forward * steerSpeed * Time.deltaTime;
                    position += -transform.forward * steerSpeed * Time.deltaTime;
                    forward = false;
                    errorText.text = "";
                    collide = 0;
                    collideOld = 0;
                }
                else
                {
                    transform.position = position;
                    transform.eulerAngles = rotation;
                }
            }
            else
            {
                if ( Input.GetKeyDown(KeyCode.UpArrow))
                {
                    transform.position += transform.forward * steerSpeed * Time.deltaTime;
                    position += transform.forward * steerSpeed * Time.deltaTime;
                    forward = true;
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    transform.position += -transform.forward * steerSpeed * Time.deltaTime;
                    position += -transform.forward * steerSpeed * Time.deltaTime;
                    forward = false;

                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    transform.Rotate(0.0f, -rotateSpeed * Time.deltaTime, 0.0f);
                    rotation.y -= rotateSpeed * Time.deltaTime;
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    transform.Rotate(0.0f, rotateSpeed * Time.deltaTime, 0.0f);
                    rotation.y += rotateSpeed * Time.deltaTime;
                }
                else
                {
                    transform.position = position;
                    transform.eulerAngles = rotation;
                }
            }
        }
        else
        {
            transform.position = position;
            transform.eulerAngles = rotation;
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
