using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Elevator_UIButtonContoller : MonoBehaviour, IButtonElevator
{

    public int StageID;
    public Text StageText;
    public Button ButtonUp;
    public Button ButtonDown;
    public GameObject Elevator;
    public Text ElevatorDoorStatus;

    public void CallButtonUp()
    {
        ButtonAction(true);
    }

    public void CallButtonDown()
    {
        ButtonAction(false);
    }

    public void ButtonAction(bool isUp)
    {
        Elevator_UIController.ElevatorObject.AddStageToQueue(StageID: StageID, isUp: isUp);
    }

    public void ConfigureButton(int ID)
    {
        StageID = ID;
        StageText.text = "Stage\n" + ID;
        if (ID != 1)
            Elevator.SetActive(false);
    }

    public void ChangeElevatorIconVisibility(bool isActive)
    {
        Elevator.SetActive(isActive);
    }

    public void ChangeDoorStatusVisibility(ElevatorMotor.DoorState doorState)
    {
        switch (doorState)
        {
            case ElevatorMotor.DoorState.Closed:
                ElevatorDoorStatus.text = "[ | ]";
                break;
            case ElevatorMotor.DoorState.Closing:
                ElevatorDoorStatus.text = "[| |]";
                break;
            case ElevatorMotor.DoorState.Open:
                ElevatorDoorStatus.text = "[   ]";
                break;
            case ElevatorMotor.DoorState.Opening:
                ElevatorDoorStatus.text = "[| |]";
                break;
        }
    }

    public void ButtonAction()
    {
        throw new System.NotImplementedException();
    }
}
