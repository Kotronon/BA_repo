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
    void Start()
    {
        foodTables = GameObject.FindGameObjectsWithTag("FoodTable");
        tables = GameObject.FindGameObjectsWithTag("Table");
    }

    void Update()
    {
        for (int i = 0; i < foodTables.Length; i++)
        {
            if (Vector3.Distance(bottel.transform.position, foodTables[i].transform.position) <= 0.2
                && bottel.transform.position.y.Equals(foodTables[i].transform.position.y))
            {
                foodTables[i].GetComponent<Light>().enabled = false;
                if (roboy.GetComponent<FingerController>().isRightGrab)
                    roboy.GetComponent<FingerController>().isRightGrab = false;
                if (roboy.GetComponent<FingerController>().isLeftGrab)
                    roboy.GetComponent<FingerController>().isLeftGrab = false;
                break;
            }
        }

        for (int i = 0; i < tables.Length; i++)
        {
            if (Vector3.Distance(bottel.transform.position, tables[i].transform.position) <= 0.2 
                && bottel.transform.position.y.Equals(tables[i].transform.position.y))
            {
                tables[i].GetComponent<Light>().enabled = false;
                if (roboy.GetComponent<FingerController>().isRightGrab)
                    roboy.GetComponent<FingerController>().isRightGrab = false;
                if (roboy.GetComponent<FingerController>().isLeftGrab)
                    roboy.GetComponent<FingerController>().isLeftGrab = false;
                break;
            }
        }
    }

    public void FoodTables()
    {
        goalChooser.SetActive(false);
        for (int i = 0; i < foodTables.Length; i++)
        {
            foodTables[i].GetComponent<Light>().enabled = true;
        }

        text.text = "Task: Bring Water to food table";
    }
    public void Tables()
    {
        goalChooser.SetActive(false);
        for (int i = 0; i < tables.Length; i++)
        {
            tables[i].GetComponent<Light>().enabled = true;
        }
        text.text = "Task: Bring Water to table";
    }
}
