using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediaPipeHandTrackingSkeleton : MonoBehaviour
{
    public Transform[] handPoints;
    public Transform[] handLines;
    public Camera camera;
    //difference from camera corner to max/min hand position
    private float offset = 4;
    Vector3[] handPointsTmp = new Vector3[21];

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLines(handLines, handPoints);
    }

    public void SetPointLocalPosition(int i, Vector3 localPosition)
    {
        handPoints[i].localPosition = localPosition;
    }

    void UpdateLines(Transform[] handLines, Transform[] handPoints)
    {
        for (int i = 0; i < 21; i++)
        {
            // if (handPoints[i].position[0].position == float.NaN) return;

            if (i == 4 || i == 16)
            {
                float length = Vector3.Distance(handPoints[0].position, handPoints[i + 1].position);
                Vector3 forward = (handPoints[i + 1].position - handPoints[0].position).normalized;

                handLines[i].position = handPoints[0].position;
                if (forward != Vector3.zero) handLines[i].forward = forward;
                handLines[i].localScale = new Vector3(length, length, length);
            }

            else if (i == 8 || i == 12)
            {
                float length = Vector3.Distance(handPoints[i - 3].position, handPoints[i + 1].position);
                Vector3 forward = (handPoints[i + 1].position - handPoints[i - 3].position).normalized;

                handLines[i].position = handPoints[i - 3].position;
                if (forward != Vector3.zero) handLines[i].forward = forward;
                handLines[i].localScale = new Vector3(length, length, length);

            }

            else if (i == 20)
            {
                float length = Vector3.Distance(handPoints[13].position, handPoints[17].position);
                Vector3 forward = (handPoints[17].position - handPoints[13].position).normalized;

                handLines[i].position = handPoints[13].position;
                if (forward != Vector3.zero) handLines[i].forward = forward;
                handLines[i].localScale = new Vector3(length, length, length);

            }

            else
            {
                float length = Vector3.Distance(handPoints[i].position, handPoints[i + 1].position);
                Vector3 forward = (handPoints[i + 1].position - handPoints[i].position).normalized;

                handLines[i].position = handPoints[i].position;
                if (forward != Vector3.zero) handLines[i].forward = forward;
                handLines[i].localScale = new Vector3(length, length, length);

            }
        }
    }

    public bool CanMoveHand(Vector3[] handPointsTranslations, GameObject ZoneManager, GameObject Roboy)
    {
        if (Roboy.GetComponent<RobodyMovement>().emergencyStop.activeInHierarchy)
        {
            Roboy.GetComponent<RobodyMovement>().errorText.text = "Error: Can't move hand. Emergency stop is still activated.";
            return false;
        }
        //get left-down and up-right edge of camera view
        Vector3 leftEdge = camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 rightEdge = camera.ViewportToWorldPoint(new Vector3(1, 1, 0));


        for (int i = 0; i < handPointsTranslations.Length; i++)
        {
            // hand won't move outside camera view
            if (handPointsTranslations[i].x < leftEdge.x || handPointsTranslations[i].x > rightEdge.x ||
                handPointsTranslations[i].y < leftEdge.y || handPointsTranslations[i].y < rightEdge.y)
            {
                Roboy.GetComponent<RobodyMovement>().errorText.text = "Error: Can't move hand out of the camera view.";
                return false;
            }

            //hand won't move if a finger tries to open fist while grabbable not at target zone
            if (!canMoveFinger(i, handPointsTranslations, ZoneManager))
            {
                Roboy.GetComponent<RobodyMovement>().errorText.text = "Error: Can't open hand until object was placed";
                return false;
            }


        }

        Roboy.GetComponent<RobodyMovement>().errorText.text = "";
        return true;
    }

    private bool canMoveFinger(int fingerIndex, Vector3[] handPointsTranslations, GameObject ZoneManager)
    {
        bool ret = true;
        //is he holding something that is not placed at the correct place?
        if (ZoneManager.GetComponent<GoalZoneManager>().bottle[ZoneManager.GetComponent<GoalZoneManager>().bottleCount].GetComponent<Grabbable>().isGrabed()
            && !ZoneManager.GetComponent<GoalZoneManager>().bottle[ZoneManager.GetComponent<GoalZoneManager>().bottleCount].GetComponent<Grabbable>().isPlaced)
        {
            //distance between old and new position
            Vector3 distance = new Vector3(handPointsTranslations[fingerIndex].x - handPoints[fingerIndex].transform.position.x,
                handPointsTranslations[fingerIndex].y - handPoints[fingerIndex].transform.position.y,
                handPointsTranslations[fingerIndex].z - handPoints[fingerIndex].transform.position.z);
            //angle between old and new position
            float angle = Vector3.Angle(handPointsTranslations[fingerIndex], handPoints[fingerIndex].transform.position);
            for (int i = 0; i < 21; i++)
            {
                //check if complete hand is moving in same direction, while holding something
                //if not, don't move the finger in this direction
                Vector3 current = new Vector3(handPointsTranslations[i].x - handPoints[i].transform.position.x,
                    handPointsTranslations[i].y - handPoints[i].transform.position.y,
                    handPointsTranslations[i].z - handPoints[i].transform.position.z);
                float currentAngle = Vector3.Angle(handPointsTranslations[i], handPoints[i].transform.position);

                if ((angle == 0 || angle == 180) && !distance.Equals(current))
                {
                    //hand moving in horizontal or vertical way -> all distances need to be the same
                    //otherwise fist will open
                    return false;
                }
                //moves with angle -> the points need to move in the same angle

                else if (!angle.Equals(currentAngle)) return false;
            }
        }

        return ret;
    }
}
