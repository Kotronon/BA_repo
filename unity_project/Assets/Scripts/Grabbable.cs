using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public bool isInRangeLeft = false;
    public bool isInRangeRight = false;

    public float range = 0.2f;

    public Transform scene;
    private bool _isGrabbedLeft = false;
    private bool _isGrabbedRight = false;
    public GameObject hand;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(hand.GetComponent<HandController>().leftHandIKTarget.position, transform.position) <= range)
        {
            isInRangeLeft = true;
        }
        else
        {
            isInRangeLeft = false;
        }

        if (Vector3.Distance(hand.GetComponent<HandController>().rightHandIKTarget.position, transform.position) <= range)
        {
            isInRangeRight = true;
        }
        else
        {
            isInRangeRight = false;
        }

        if (hand.GetComponent<FingerController>().isLeftGrab && isInRangeLeft && (!_isGrabbedLeft && !_isGrabbedRight))
        {
            Debug.Log("Left Grab");
            // transform.position = HandIK.Instance.leftGripPivot.position;
            transform.SetParent(hand.GetComponent<FingerController>().leftGripPivot);
            // GetComponent<Rigidbody>().useGravity = false;
            // GetComponent<Rigidbody>().velocity = Vector3.zero;
            _isGrabbedLeft = true;
        }

        else if (hand.GetComponent<FingerController>().isRightGrab && isInRangeRight && (!_isGrabbedLeft && !_isGrabbedRight))
        {
            Debug.Log("Right Grab");
            // transform.position = HandIK.Instance.rightGripPivot.position;
            transform.SetParent(hand.GetComponent<FingerController>().rightGripPivot);

            // GetComponent<Rigidbody>().useGravity = false;
            // GetComponent<Rigidbody>().velocity = Vector3.zero;
            _isGrabbedRight = true;
        }

        if (!hand.GetComponent<FingerController>().isLeftGrab && _isGrabbedLeft)
        {
            Debug.Log("Left Release");
            transform.SetParent(scene);
            _isGrabbedLeft = false;
        }

        else if (!hand.GetComponent<FingerController>().isRightGrab && _isGrabbedRight)
        {
            Debug.Log("Right Release");
            // GetComponent<Rigidbody>().useGravity = true;
            transform.SetParent(scene);
            _isGrabbedRight = false;
        }
    }


}
