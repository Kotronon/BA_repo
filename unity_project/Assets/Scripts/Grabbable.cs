using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public bool isInRangeLeft = false;
    public bool isInRangeRight = false;

    public float range = 0.2f;
    public GameObject roboy;
    private HandController hand;
    private FingerController finger;

    public Transform scene;
    private bool _isGrabbedLeft = false;
    private bool _isGrabbedRight = false;
    public bool isPlaced = false;

    void Start()
    {
        hand = roboy.GetComponent<HandController>();
        finger = roboy.GetComponent<FingerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(hand.leftHandIKTarget.position, transform.position) <= range)
        {
            isInRangeLeft = true;
        }
        else
        {
            isInRangeLeft = false;
        }

        if (Vector3.Distance(hand.rightHandIKTarget.position, transform.position) <= range)
        {
            isInRangeRight = true;
        }
        else
        {
            isInRangeRight = false;
        }

        if (finger.isLeftGrab && isInRangeLeft && (!_isGrabbedLeft && !_isGrabbedRight))
        {
            Debug.Log("Left Grab");
            // transform.position = HandIK.Instance.leftGripPivot.position;
            transform.SetParent(finger.leftGripPivot);
            // GetComponent<Rigidbody>().useGravity = false;
            // GetComponent<Rigidbody>().velocity = Vector3.zero;
            _isGrabbedLeft = true;
        }

        else if (finger.isRightGrab && isInRangeRight && (!_isGrabbedLeft && !_isGrabbedRight))
        {
            Debug.Log("Right Grab");
            // transform.position = HandIK.Instance.rightGripPivot.position;
            transform.SetParent(finger.rightGripPivot);

            // GetComponent<Rigidbody>().useGravity = false;
            // GetComponent<Rigidbody>().velocity = Vector3.zero;
            _isGrabbedRight = true;
        }

        if (!finger.isLeftGrab && _isGrabbedLeft)
        {
            Debug.Log("Left Release");
            transform.SetParent(scene);
            _isGrabbedLeft = false;
        }

        else if (!finger.isRightGrab && _isGrabbedRight)
        {
            Debug.Log("Right Release");
            // GetComponent<Rigidbody>().useGravity = true;
            transform.SetParent(scene);
            _isGrabbedRight = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if ((other.gameObject.CompareTag("Table") || other.gameObject.CompareTag("FoodTable")) && other.bounds.Contains(transform.position))
            isPlaced = true;
    }

}
