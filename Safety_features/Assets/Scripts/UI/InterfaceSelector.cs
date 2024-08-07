using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class InterfaceSelector : MonoBehaviour
{
    private MLInput.Controller controller;
    private LineRenderer beamLine;
    public Color startColor;
    public Color endColor; 
    public TMP_Text errorText;
    private Transform bottle;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        controller = MLInput.GetController(MLInput.Hand.Left);
        beamLine = GetComponent<LineRenderer>();
        beamLine.startColor = startColor;
        beamLine.endColor = endColor;
        target = GameObject.FindGameObjectWithTag("GoalZoneManager");
        bottle = target.GetComponent<GoalZoneManager>().getGrabbed().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = controller.Orientation;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
        {
            beamLine.useWorldSpace = true;
            beamLine.SetPosition(0, transform.position);
            beamLine.SetPosition(1, hit.point);
            if (hit.collider.transform.gameObject.layer == 9 && controller.IsBumperDown && hit.collider.transform.gameObject.GetComponent<GazeButton>() != null)
            {
                hit.collider.transform.gameObject.GetComponent<GazeButton>().Press();
            }
            /*else if (hit.collider.transform.gameObject.CompareTag("Botle") && controller.TriggerValue > 0.3f)
            {
                if (transform.parent.gameObject.GetComponent<RobodyMovement>().isStopped())
                {
                    errorText.text = "Error: Robody was stopped.\n You can't move him.";
                }
                else
                {
                    hit.collider.transform.gameObject.GetComponent<Grabbable>().SetGrabbed(true);
                    hit.collider.transform.parent = transform.parent;
                    hit.collider.transform.position = new Vector3(transform.position.x - 1, transform.position.y,
                        transform.position.z);
                    bottle = hit.collider.transform;
                }
            }
            else if (hit.collider.transform.gameObject.CompareTag("FoodTable") && controller.TriggerValue > 0.3f)
            {
                if (transform.parent.gameObject.GetComponent<RobodyMovement>().isStopped())
                {
                    errorText.text = "Error: Robody was stopped.\n You can't move him.";
                }
                else if(bottle != null) errorText.text = "Error: You have no water bottle grabed.";
                else if (target.GetComponent<GoalZoneManager>().getTarget() != 0) errorText.text = "Error: Wrong target";
                else
                {
                    bottle.parent = hit.collider.transform;
                    bottle.position = new Vector3(0f, -0.05f, 0);
                    bottle = null;
                    bottle.gameObject.GetComponent<Grabbable>().SetGrabbed(false);
                    hit.collider.transform.gameObject.GetComponent<GoalZone>().placed = true;

                }
            }
            else if (hit.collider.transform.gameObject.CompareTag("Table") && controller.TriggerValue > 0.3f)
            {
                if (transform.parent.gameObject.GetComponent<RobodyMovement>().isStopped())
                {
                    errorText.text = "Error: Robody was stopped.\n You can't move him.";
                }
                else if(bottle != null) errorText.text = "Error: You have no water bottle grabed.";
                else if (target.GetComponent<GoalZoneManager>().getTarget() != 1) errorText.text = "Error: Wrong target";
                else
                {
                    bottle.parent = hit.collider.transform;
                    bottle.position = new Vector3(0f, -0.1f, 0);
                    bottle = null;
                    bottle.gameObject.GetComponent<Grabbable>().SetGrabbed(false);
                    hit.collider.transform.gameObject.GetComponent<GoalZone>().placed = true;

                }
            }
            else
            {
                errorText.text = "Error:";
            }*/
        }
        else
        {
            beamLine.useWorldSpace = true;
            beamLine.SetPosition(0, transform.position);
            beamLine.SetPosition(1, transform.forward * 5);
        }
    }
}
