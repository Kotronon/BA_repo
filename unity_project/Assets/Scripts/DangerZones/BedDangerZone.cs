using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedDangerZone : DangerZone
{
    
    // Start is called before the first frame update
    void Start()
    {
        SetRanges(3, 4);
        Vector3 zonePosition = transform.position;
        zonePosition.x -= roomDecore.transform.position.x;
        SetZonePosition(zonePosition);
    }
    
}