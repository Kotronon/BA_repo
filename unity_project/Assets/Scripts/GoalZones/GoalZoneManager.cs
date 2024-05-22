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
    public GameObject bottel;
    public TMP_Text text;
    public GameObject roboy;
    private int targetID;
    private int targetPlace;
    void Start()
    {
        foodTables = GameObject.FindGameObjectsWithTag("FoodTable");
        tables = GameObject.FindGameObjectsWithTag("Table");
        targetID = 0;
        targetPlace = 0;
    }

    void Update()
    {
        /*for (int i = 0; i < foodTables.Length; i++)
        {
             if (Vector3.Distance(bottel.transform.position, foodTables[i].transform.position) <= 0.2
                 && bottel.transform.position.y.Equals(foodTables[i].transform.position.y))
             {
                 //foodTables[i].GetComponent<Light>().enabled = false;
                 foodTables[i].transform.GetChild(1).gameObject.SetActive(false);
                 bottel.GetComponent<Grabbable>().isPlaced = true;
                 text.text = "Task: finished; leave the room";
                 break;
             }
         }

         for (int i = 0; i < tables.Length; i++)
         {
             if (Vector3.Distance(bottel.transform.position, tables[i].transform.position) <= 0.2 
                 && bottel.transform.position.y.Equals(tables[i].transform.position.y))
             {
                 //tables[i].GetComponent<Light>().enabled = false;
                 tables[i].transform.GetChild(1).gameObject.SetActive(false);
                 bottel.GetComponent<Grabbable>().isPlaced = true;
                 text.text = "Task: finished; leave the room";
                 break;
             }
        }*/

        if (targetPlace == 0 && targetID == foodTables.Length  ||
            targetPlace == 1 && targetID == tables.Length)
        {
            text.text = "Task: finished.";
        }
        else 
        {
            if (targetPlace == 0)
            {
                if (foodTables[targetID].GetComponent<GoalZone>().placed)
                {
                    targetID++;
                    foodTables[targetID].transform.GetChild(1).gameObject.SetActive(true);
                    text.text = "Task: bring bottle of water to next food table.";
                }
            }
            else
            {
                if (tables[targetID].GetComponent<GoalZone>().placed)
                {
                    targetID++;
                    tables[targetID].transform.GetChild(1).gameObject.SetActive(true);
                    text.text = "Task: bring bottle of water to table in next room.";
                }

            }
        }

    }

    public void FoodTables()
    {
        /*goalChooser.SetActive(false);
        for (int i = 0; i < foodTables.Length; i++)
        {
            //foodTables[i].GetComponent<Light>().enabled = true;
            foodTables[i].transform.GetChild(1).gameObject.SetActive(true);
        }

        text.text = "Task: Bring Water to food table";*/
        foodTables[targetID].transform.GetChild(1).gameObject.SetActive(true);
        text.text = "Task: bring bottle of water to first food table in first room.";
        goalChooser.SetActive(false);
    }
    public void Tables()
    {
        /*goalChooser.SetActive(false);
        for (int i = 0; i < tables.Length; i++)
        {
            //tables[i].GetComponent<Light>().enabled = true;
            tables[i].transform.GetChild(1).gameObject.SetActive(true);
        }
        text.text = "Task: Bring Water to table";*/
        targetPlace = 1;
        tables[targetID].transform.GetChild(1).gameObject.SetActive(true);
        text.text = "Task: bring bottle of water to table in first room.";
        goalChooser.SetActive(false);
    }
}
