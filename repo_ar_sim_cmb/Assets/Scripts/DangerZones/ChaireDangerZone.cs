using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaireDangerZone : DangerZone
{
    // Start is called before the first frame update
    void Start()
    {
        SetRanges(1.6f, 2);
        Vector3 zonePosition = transform.position;
        zonePosition.x += roomDecore.transform.position.x;
        SetZonePosition(zonePosition);
    }

}
