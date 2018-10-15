using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Elevator_UIInElevatorButtonController : MonoBehaviour, IButtonElevator
{
    public int StageID;
    public Text StageText;

    public void ButtonAction()
    {
        if (StageID == 0)
        {
            Elevator_UIController.ElevatorObject.Stop();
            return;
        }

        Elevator_UIController.ElevatorObject.AddStageToQueue(StageID: StageID, bothDirection: true);
    }

    public void ConfigureButton(int ID)
    {
        StageID = ID;
        if (StageText)
            StageText.text = ID.ToString();
    }
}
