using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOnObjects : MonoBehaviour
{
    public Timer timer;

    public string actionName; // e.g. "Exercise", "Farm"

    void OnMouseDown()
    {


        if (timer == null) return;

        switch (actionName)
        {
            case "Exercise": timer.DoExercise(); break;
            case "Farm": timer.DoFarm(); break;
            case "Project": timer.DoElectricalProject(); break;
            default: Debug.Log("Unknown action: " + actionName); break;
        }
    }
}
