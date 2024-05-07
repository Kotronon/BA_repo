using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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
    private Camera camera;
    //difference from camera corner to max/min hand position
    private float offset = 4;
    Vector3[] handPointsTmp = new Vector3[21];
    public GameObject[] dangerZones;

    private void Start()
    {
        kalmanFilter = new Vector3(Q, R);
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        /*string data = GlobalVariableManager.Instance.HandLandmarksRaw;
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

        for (int i = 0; i < 21; i++)
        {
            handPointsTranslations[i] *= handScale;
            HandPoints[i].transform.localPosition = handPointsTranslations[i];
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
        //only move hand iff all lines of hand are inside the camera view
        if(CanMoveHand()){
            for (int i = 0; i < 21; i++)
            {
                if (i == 4 || i == 16)
                {
                    float length = Vector3.Distance(handPointsTranslations[0], handPointsTranslations[i + 1]);
                    Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[0]).normalized;
                    setPosition(i, forward, length);
                }

                else if (i == 8 || i == 12)
                {
                    float length = Vector3.Distance(handPointsTranslations[i - 3], handPointsTranslations[i + 1]);
                    Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[i - 3]).normalized;
                    setPosition(i - 3, forward, length);

                }

                else if (i == 20)
                {
                    float length = Vector3.Distance(handPointsTranslations[13], handPointsTranslations[17]);
                    Vector3 forward = (handPointsTranslations[17] - handPointsTranslations[13]).normalized;
                    setPosition(13, forward, length);
                }

                else
                {
                    float length = Vector3.Distance(handPointsTranslations[i], handPointsTranslations[i + 1]);
                    Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[i]).normalized;
                    setPosition(i, forward, length);

                }
            }
        }
    }

    private void setPosition(int i, Vector3 forward, float length)
    {
        HandLines[i].transform.localPosition = handPointsTranslations[0];
        HandLines[i].transform.forward = forward;
        HandLines[i].transform.localScale = new Vector3(length, length, length);
    }

    private bool CanMoveHand()
    {
        //get left-down and up-right edge of camera view
        Vector3 leftEdge= Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        

        for (int i = 0; i < handPointsTranslations.Length; i++)
        {
            // hand won't move outside camera view
            if (handPointsTranslations[i].x < leftEdge.x || handPointsTranslations[i].x > rightEdge.x ||
                handPointsTranslations[i].y < leftEdge.y || handPointsTranslations[i].y < rightEdge.y) return false;
            //Too close to patient; check all possible dangerZones
            for (i = 0; i < dangerZones.Length; i++)
            {
                Vector3 position = dangerZones[i].transform.position;
                position.x += dangerZones[i].GetComponent<DangerZone>().roomDecore.transform.position.x;
                if (Vector3.Distance(handPointsTranslations[i], position) <= dangerZones[i].GetComponent<DangerZone>().GetInnerRange())
                {
                    DangerZoneManager.GetComponent<DangerZoneManager>().StopRobot(i);

                    return false;
                }
            }
           

        }
        return true;
    }
}
