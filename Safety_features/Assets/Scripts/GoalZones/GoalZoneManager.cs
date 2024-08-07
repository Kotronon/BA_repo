using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalZoneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] foodTables;
    public GameObject[] tables;
    public GameObject goalChooser;
    public GameObject[] bottle;
    public TMP_Text text;
    public GameObject roboy;
    private int targetID;
    private int targetPlace;
    public int bottleCount;
    void Start()
    {
        foodTables = GameObject.FindGameObjectsWithTag("FoodTable");
        tables = GameObject.FindGameObjectsWithTag("Table");
        bottle = GameObject.FindGameObjectsWithTag("Bottle");
        targetID = 0;
        targetPlace = 0;
        bottleCount = 0;
    }

    void Update()
    {
        if (targetPlace == 0 && targetID == foodTables.Length ||
            targetPlace == 1 && targetID == tables.Length)
        {
            //text.text = "Task: finished.";
            text.SetText("Task: finished");
            text.ForceMeshUpdate(true);
            //text.SetCharArray("Task: finished.".ToCharArray());
            //text.SetAllDirty();
        }
        else if (bottle[bottleCount].GetComponent<Grabbable>().isPlaced)
        {
            if (targetID == 0)
            {
                for (int i = 0; i < foodTables.Length; i++)
                {
                    if (foodTables[i].GetComponent<GoalZone>().placed && foodTables[i].transform.GetChild(1).gameObject.activeInHierarchy)
                    {
                        //foodTables[i].GetComponent<Light>().enabled = false;
                        foodTables[i].transform.GetChild(1).gameObject.SetActive(false);
                        //text.text = "Task: get next water bottle";
                        text.SetText("Task: get next water bottle");
                        text.ForceMeshUpdate(true);
                        bottleCount++;
                        bottle[bottleCount].transform.GetChild(0).gameObject.SetActive(true);
                        break;
                    }
                }
            }

            if (targetID == 1)
            {
                for (int i = 0; i < tables.Length; i++)
                {
                    if (tables[i].GetComponent<GoalZone>().placed && tables[i].transform.GetChild(1).gameObject.activeInHierarchy)
                    {
                        //foodTables[i].GetComponent<Light>().enabled = false;
                        tables[i].transform.GetChild(1).gameObject.SetActive(false);
                        //text.text = "Task: get next water bottle";
                        text.SetText("Task: get next water bottle");
                        text.ForceMeshUpdate(true);
                        bottleCount++;
                        bottle[bottleCount].transform.GetChild(0).gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }

        else if (!bottle[bottleCount].GetComponent<Grabbable>().isGrabed())
        {
            text.SetText("Task: get next water bottle");
            text.ForceMeshUpdate(true);
        }
        else
        {
            if (targetID == 0)
            {
                //text.text = "Task: bring water bottle to a food table";
                text.SetText("Task: get next water bottle");
                text.ForceMeshUpdate(true);
            }

            if (targetID == 1)
            {
                //text.text = "Task: bring water bottle to a table";
                text.SetText("Task: bring water bottle to a table");
                text.ForceMeshUpdate(true);
            }
        }
    }

    public void FoodTables()
    {
        goalChooser.SetActive(false);
        for (int i = 0; i < foodTables.Length; i++)
        {
            //foodTables[i].GetComponent<Light>().enabled = true;
            foodTables[i].transform.GetChild(1).gameObject.SetActive(true);
        }

        if (bottle[bottleCount].GetComponent<Grabbable>().isGrabed())
        {
            text.SetText("Task: bring water bottle to a food table");
            text.ForceMeshUpdate(true);
        }
        else
        {
            text.SetText("Task: Get first water bottle");
            text.ForceMeshUpdate(true);
        bottle[bottleCount].transform.GetChild(0).gameObject.SetActive(true);
        }

    }
    public void Tables()
    {
        goalChooser.SetActive(false);
        for (int i = 0; i < tables.Length; i++)
        {
            //tables[i].GetComponent<Light>().enabled = true;
            tables[i].transform.GetChild(1).gameObject.SetActive(true);
        }
        targetPlace = 1;
        if (bottle[bottleCount].GetComponent<Grabbable>().isGrabed())
        {
            text.SetText("Task: bring water bottle to a table");
            text.ForceMeshUpdate(true);
        }
        else
        {
            text.SetText("Task: Get first water bottle");
            text.ForceMeshUpdate(true);
            bottle[bottleCount].transform.GetChild(0).gameObject.SetActive(true);
        }

    }

    public int getTarget()
    {
        return targetID;
    }

    public GameObject getGrabbed()
    {
        if (bottle[bottleCount].GetComponent<Grabbable>().isGrabed()) return bottle[bottleCount];
        else return null;
    }
}
