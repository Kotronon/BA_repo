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
    private int dangerZoneCurrent;
    private Vector3 dangerPos = new Vector3(-1f, -1f, -1f);
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        robodyM = robody.GetComponent<RobodyMovement>();
        //color = dangerZones[0].GetComponent<Light>().color;
        dangerZones = GameObject.FindGameObjectsWithTag("DangerZone");
        dangerZoneCurrent = -1;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < dangerZones.Length; i++)
        {
            /* DangerZone current = dangerZones[i].GetComponent<DangerZone>();
             Vector3 roboyPos = robody.transform.position;
             roboyPos.y = 0;
             Vector3 currentPos = current.getPos();
             currentPos.y = 0;
             float distance = Vector3.Distance(roboyPos, currentPos);

             if (distance <= current.GetInnerRange())
             {
                 //can't enter danger zone
                 //if(dangerPos.Equals(new Vector3(-1, -1, -1))) dangerPos = roboyPos;
                 //dangerZones[i].GetComponent<Light>().color = Color.red;
                /* if (!current.getUserWantsToMoveOn())
                 {
                     StopRobot(i);

                 }
               // robody.transform.position = dangerPos;
             }
             /*if (current.getCollided())
             {
                 dangerZones[i].GetComponent<Light>().color = Color.red;
                 if (!current.getUserWantsToMoveOn())
                 {
                     StopRobot(i);

                 }
             }
             else if (distance <= current.GetOuterRange())
             {
                 dangerZones[i].GetComponent<Light>().color = Color.red;
                 robodyM.steerSpeed = 2f;
                 current.setUserWantsToMoveOn(false);
             }

             //if distance <=  x -> halo gets red

             else
             {
                 robodyM.steerSpeed = 4f;
                 dangerZones[i].GetComponent<Light>().color = color;
                 current.setUserWantsToMoveOn(false);
             }*/
            /*if (dangerZones[i].GetComponent<DangerZone>().collide > dangerZones[i].GetComponent<DangerZone>().collideOld)
            {
                robodyM.steerSpeed = 0f;
            }
            else
            {
                robodyM.steerSpeed = 4f;
            }*/
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
