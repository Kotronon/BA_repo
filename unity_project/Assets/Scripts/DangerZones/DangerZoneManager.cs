using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class DangerZoneManager : MonoBehaviour
{
    public GameObject[] dangerZones;
    public GameObject dangerMessage;
    public Transform robody;
    private RobodyMovement robodyM;
    private float outerRange = 4;
    public float innerRange = 3;
    private int dangerZoneCurrent;

    private Color color;
    // Start is called before the first frame update
    void Start()
    {
        robodyM = robody.GetComponent<RobodyMovement>();
        color = dangerZones[0].GetComponent<Light>().color;
        dangerZones = GameObject.FindGameObjectsWithTag("DangerZone");
        dangerZoneCurrent = -1;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < dangerZones.Length; i++)
        {
            DangerZone current = dangerZones[i].GetComponent<DangerZone>();
            float distance = Vector3.Distance(robody.transform.position, current.getPos());

            if (distance <= innerRange)
            {
                dangerZones[i].GetComponent<Light>().color = Color.red;
                if (!current.getUserWantsToMoveOn())
                {
                    StopRobot(i);

                }
            }
            else if (distance <= outerRange)
            {
                dangerZones[i].GetComponent<Light>().color = Color.red;
                robodyM.steerSpeed = 0.5f;
                current.setUserWantsToMoveOn(false);
            }
            
            //if distance <=  x -> halo gets red

            else
            {
                robodyM.steerSpeed = 1f;
                dangerZones[i].GetComponent<Light>().color = color;
            }
        }
       
    }
    public void StopRobot(int i)
    {
        dangerZoneCurrent = i;
        //pop up text and bottom
        dangerMessage.SetActive(true);

        //make stop 
        robodyM.Stop();
    }
    public void RestartRobot()
    {
        if (dangerZoneCurrent != -1)
        {
            dangerZones[dangerZoneCurrent].GetComponent<DangerZone>().setUserWantsToMoveOn(true);
            dangerMessage.SetActive(false);
            robodyM.Restart();

        }
    }
}
