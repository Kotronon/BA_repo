using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    private Vector3 zonePosition;
    public Transform roomDecore;
    //change values
    private float outerRange;
    private float innerRange;
    private bool userWantsToMoveOn = false;
    // Start is called before the first frame update
    public void SetRanges(float inner, float outer)
    {
        outerRange = outer;
        innerRange = inner;
    }

    public void SetZonePosition(Vector3 position)
    {
        zonePosition = position;
    }

    public float GetInnerRange()
    {
        return innerRange;
    }
    public float GetOuterRange() {  return outerRange; }
    public Vector3 getPos()
    {
        return zonePosition;
    }

    public void setUserWantsToMoveOn(bool userWantsToMoveOn)
    {
        this.userWantsToMoveOn = userWantsToMoveOn;
    }
    public bool getUserWantsToMoveOn() { return userWantsToMoveOn; }
}
