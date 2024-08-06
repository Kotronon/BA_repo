using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class InterfaceSelector : MonoBehaviour
{
    private MLInput.Controller controller;
    private LineRenderer beamLine;
    public Color startColor;
    public Color endColor;
    // Start is called before the first frame update
    void Start()
    {
        controller = MLInput.GetController(MLInput.Hand.Left);
        beamLine = GetComponent<LineRenderer>();
        beamLine.startColor = startColor;
        beamLine.endColor = endColor;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = controller.Orientation;
        RaycastHit hit;
        bool found = false;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
        {
            for (int i = 0; i < 8; i++)
            {
                if (hit.collider.transform.gameObject.layer == 9)
                {
                    found = true;
                    break;
                }
                //check if behind the collision object the button is, that the user wants to press
                if (Physics.Raycast(transform.position + transform.forward * (i+1), transform.forward, out hit, 100f))
                    continue;
                else break;

            }
        }
        if(found){

            beamLine.useWorldSpace = true;
            beamLine.SetPosition(0, transform.position);
            beamLine.SetPosition(1, hit.point);
            if (hit.collider.transform.gameObject.layer == 9 && controller.IsBumperDown && hit.collider.transform.gameObject.GetComponent<GazeButton>() != null)
            {
                hit.collider.transform.gameObject.GetComponent<GazeButton>().Press();
            }
        }
        else
        {
            beamLine.useWorldSpace = true;
            beamLine.SetPosition(0, transform.position);
            beamLine.SetPosition(1, transform.forward * 5);
        }
    }
}
