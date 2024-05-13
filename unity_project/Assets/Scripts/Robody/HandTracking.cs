using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.XR;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class HandTracking : MonoBehaviour
{
    [SerializeField] public GameObject[] HandPoints;
    [SerializeField] public GameObject[] HandLines;
    public GameObject DangerZoneManager;

    public float Scale = 100;

    public Vector3 handRootTranslation;
    public Vector2 handRootScreenPos;
    public Vector3 palmForward;
    public Vector3 palmNorm;

    public float handDistance;
    public float handDistanceScale = 0.1f;

    public float palmLength = 10f; // in centimeter
    public float focalLength = 680f; // in centimeter

    public bool flipPalm = false;

    float trackedPalmLength;
    Vector3[] handPointsTranslations = new Vector3[21];
    // public Vector3 handRootRotation;

    public float Q = 0.0001f;
    public float R = 0.01f;
    Vector3 kalmanFilter = new Vector3();
    public Camera camera;
    //difference from camera corner to max/min hand position
    private float offset = 4;
    Vector3[] handPointsTmp = new Vector3[21];
    public GameObject[] dangerZones;
    public GameObject bottel;
    public GameObject Roboy;

    private void Start()
    {
        kalmanFilter = new Vector3(Q, R);
    }

    // Update is called once per frame
    void Update()
    {
       /* string data = GlobalVariableManager.Instance.HandLandmarksRaw;
        if (data.Length < 1) return;
        
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == '[')
            {
                trackedPalmLength = float.Parse(data.Substring(0, i));
                data = data.Substring(i, data.Length - i);
            }
        }

        data = data.Substring(1, data.Length - 2);

        string[] points = data.Split(',');

        for (int i = 0; i < 21; i++)
        {
            float x = float.Parse(points[i * 3]) / Scale;
            float y = float.Parse(points[i * 3 + 1]) / Scale;
            float z = float.Parse(points[i * 3 + 2]) / Scale;

            if (i == 0)
            {
                handRootTranslation = new Vector3(x, y, z);
                handRootScreenPos = new Vector2(x * Scale, y * Scale);
            }

            // HandPoints[i].transform.localPosition = new Vector3(x, y, z) - handRootTranslation;
            handPointsTranslations[i] = new Vector3(x, y, z) - handRootTranslation;
            
        }

        Vector3 palmVec1 = handPointsTranslations[5] - handPointsTranslations[0];
        Vector3 palmVec2 = handPointsTranslations[17] - handPointsTranslations[0];

        palmNorm = (Vector3.Cross(palmVec1, palmVec2) * (flipPalm ? -1 : 1)).normalized;
        palmForward = (palmVec1 + palmVec2).normalized;

        // trackedPalmLength = Vector3.Distance(handPointsTranslations[0], handPointsTranslations[5]);
        float handScale = palmLength / trackedPalmLength;

        if(CanMoveHand()){
            for (int i = 0; i < 21; i++)
            {
                bool[] canMoveFinger = this.canMoveFinger(i);
                Vector3 tmp = HandPoints[i].transform.localPosition;
                if (canMoveFinger[0])
                {
                    handPointsTranslations[i].x *= handScale;
                    tmp.x = handPointsTranslations[i].x;
                }
                if (canMoveFinger[1])
                {
                    handPointsTranslations[i].y *= handScale;
                    tmp.y = handPointsTranslations[i].y;
                }
                if (canMoveFinger[2])
                {
                    handPointsTranslations[i].z *= handScale;
                    tmp.z = handPointsTranslations[i].z;
                }
                HandPoints[i].transform.localPosition = tmp;
                
            }
        }

        handRootTranslation *= handScale;

        handRootTranslation = kalmanFilter.Update(handRootTranslation);

        float distance = focalLength * palmLength / trackedPalmLength;
        handDistance = distance * handDistanceScale;
        */
        UpdateLines();
    }

    void UpdateLines()
    {
        for (int i = 0; i < 21; i++)
        {
            if (i == 4 || i == 16)
            {
                float length = Vector3.Distance(handPointsTranslations[0], handPointsTranslations[i + 1]);
                Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[0]).normalized;

                HandLines[i].transform.localPosition = handPointsTranslations[0];
                HandLines[i].transform.forward = forward;
                HandLines[i].transform.localScale = new Vector3(length, length, length);
            }

            else if (i == 8 || i == 12)
            {
                float length = Vector3.Distance(handPointsTranslations[i - 3], handPointsTranslations[i + 1]);
                Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[i - 3]).normalized;

                HandLines[i].transform.localPosition = handPointsTranslations[i - 3];
                HandLines[i].transform.forward = forward;
                HandLines[i].transform.localScale = new Vector3(length, length, length);

            }

            else if (i == 20)
            {
                float length = Vector3.Distance(handPointsTranslations[13], handPointsTranslations[17]);
                Vector3 forward = (handPointsTranslations[17] - handPointsTranslations[13]).normalized;

                HandLines[i].transform.localPosition = handPointsTranslations[13];
                HandLines[i].transform.forward = forward;
                HandLines[i].transform.localScale = new Vector3(length, length, length);

            }

            else
            {
                float length = Vector3.Distance(handPointsTranslations[i], handPointsTranslations[i + 1]);
                Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[i]).normalized;

                HandLines[i].transform.localPosition = handPointsTranslations[i];
                HandLines[i].transform.forward = forward;
                HandLines[i].transform.localScale = new Vector3(length, length, length);

            }
        }
    }

  

    private bool CanMoveHand()
    {
        if (Roboy.GetComponent<RobodyMovement>().emergencyStop.activeInHierarchy) return false;
        //get left-down and up-right edge of camera view
        Vector3 leftEdge= camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 rightEdge = camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        

        for (int i = 0; i < handPointsTranslations.Length; i++)
        {
            // hand won't move outside camera view
            if (handPointsTranslations[i].x < leftEdge.x || handPointsTranslations[i].x > rightEdge.x ||
                handPointsTranslations[i].y < leftEdge.y || handPointsTranslations[i].y < rightEdge.y) return false;
            //Too close to patient; check all possible dangerZones
            for (i = 0; i < dangerZones.Length; i++)
            {
                Vector3 position = dangerZones[i].transform.position;
                if (Vector3.Distance(handPointsTranslations[i], position) <= dangerZones[i].GetComponent<DangerZone>().GetInnerRange())
                {
                    DangerZoneManager.GetComponent<DangerZoneManager>().StopRobot(i);

                    return false;
                }
            }
           

        }
        return true;
    }

    private bool[] canMoveFinger(int fingerIndex)
    {
        bool[] ret = {true, true, true};
        if (bottel.GetComponent<Grabbable>().hand.GetComponent<FingerController>().isRightGrab ||
            bottel.GetComponent<Grabbable>().hand.GetComponent<FingerController>().isLeftGrab)
        {
            Vector3 distance = new Vector3(handPointsTranslations[fingerIndex].x - HandPoints[fingerIndex].transform.position.x,
                handPointsTranslations[fingerIndex].y - HandPoints[fingerIndex].transform.position.y,
                handPointsTranslations[fingerIndex].z - HandPoints[fingerIndex].transform.position.z);
            for (int i = 0; i < 21; i++)
            {
                //check if complete hand is moving in same direction, while holding something
                //if not, don't move the finger in this direction
                Vector3 current = new Vector3(handPointsTranslations[i].x - HandPoints[i].transform.position.x,
                    handPointsTranslations[i].y - HandPoints[i].transform.position.y,
                    handPointsTranslations[i].z - HandPoints[i].transform.position.z);


                if (!distance.x.Equals(current.x))
                {
                    ret[0] = false;
                }

                if (!distance.y.Equals(current.y))
                {
                    ret[1] = false;
                }

                if (!distance.z.Equals(current.z))
                {
                    ret[2] = false;
                }
            }
        }

        return ret;
    }
}
