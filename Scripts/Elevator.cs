using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Elevator : ElevatorMotor
{
    public void AddStageToQueue(int StageID, bool isUp = true, bool bothDirection = false)
    {
        Queues.AddStageToQueue(isUp, StageID);

        if (bothDirection)
        {
            Queues.AddStageToQueue(!isUp, StageID);
        }

        if (!isInAction)
        {
            StartCoroutine(MoveElevator());
        } else
        {
            if (GetNewTargetStage())
                return;
        }
    }

    public void Stop()
    {
        EmergencyStop();
    }
}

public class ElevatorMotor : MonoBehaviour
{
    public enum DoorState { Opening, Open, Closing, Closed };
    DoorState DoorStatus = DoorState.Closed;

    float ActionTimeStep = 0.5f;
    protected QueuesManager Queues = new QueuesManager();
    protected int CurrentStage = 1;

    int TargetStage;

    bool isMovingUp = true;
    
    public bool isInAction { get; private set; }

   protected IEnumerator MoveElevator()
    {
        if (!GetNewTargetStage())
        { 
            isInAction = false;
            yield break;
        }

        isMovingUp = TargetStage > CurrentStage;

        while (CurrentStage != TargetStage)
        {
            isInAction = true;
            Elevator_UIController.THIS.ChangeElevatorIconVisibility(CurrentStage, false);
            CurrentStage += (isMovingUp) ? 1 : -1;
            Elevator_UIController.THIS.ChangeElevatorIconVisibility(CurrentStage, true);
            yield return new WaitForSeconds(ActionTimeStep);
        }

        if (isInAction)
        {
            Queues.RemoveStageFromQueue(Stage: CurrentStage, bothDirection: true);
            yield return StartCoroutine(DoorController());
            isInAction = false;
            StartCoroutine(MoveElevator());
        }/*
        else
        {
            yield return new WaitForSeconds(ActionTimeStep);
            isInAction = false;
            StartCoroutine(MoveElevator());
        }*/
    }

    protected bool GetNewTargetStage()
    {
        Queues.GetTargetStage(CurrentStage, ref isMovingUp, ref TargetStage);

        if (TargetStage != -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator DoorController()
    {
        yield return new WaitForSeconds(ActionTimeStep);
        DoorStatus = DoorState.Opening;
        Elevator_UIController.THIS.ChangeDoorStatusVisibility(CurrentStage, DoorStatus);
        yield return new WaitForSeconds(ActionTimeStep);
        DoorStatus = DoorState.Open;
        Elevator_UIController.THIS.ChangeDoorStatusVisibility(CurrentStage, DoorStatus);
        yield return new WaitForSeconds(ActionTimeStep);
        DoorStatus = DoorState.Closing;
        Elevator_UIController.THIS.ChangeDoorStatusVisibility(CurrentStage, DoorStatus);
        yield return new WaitForSeconds(ActionTimeStep);
        DoorStatus = DoorState.Closed;
        Elevator_UIController.THIS.ChangeDoorStatusVisibility(CurrentStage, DoorStatus);
        yield return new WaitForSeconds(ActionTimeStep);
    }

    protected void EmergencyStop()
    {
        Debug.Log("Elevator Emergency Stop " + CurrentStage);
        Queues = new QueuesManager();
        isInAction = false;
        TargetStage = CurrentStage;
    }
}

public class QueuesManager
{
    List<int> UpQueue = new List<int>();
    List<int> DownQueue = new List<int>();

    public void AddStageToQueue(bool isUp, int Stage)
    {
        if (isUp)
        {
            UpQueue.Add(Stage);
            UpQueue.Sort();
        }
        else
        {
            DownQueue.Add(Stage);
            DownQueue.Sort();
            DownQueue.Reverse();
        }
    }

    public void RemoveStageFromQueue(int Stage, bool isUp = true, bool bothDirection = false)
    {
        if (bothDirection || isUp)
        {
            UpQueue.Remove(Stage);
            UpQueue.Sort();
        }
        if(bothDirection || !isUp)
        {
            DownQueue.Remove(Stage);
            DownQueue.Sort();
            DownQueue.Reverse();
        }
    }

    public void GetTargetStage(int CurrentStage, ref bool isMovingUp, ref int TargetStage)
    {
        if(GetPerfectTargetStage(CurrentStage, ref isMovingUp, ref TargetStage))
        {
            return;
        }

        GetAnyTargetStage(CurrentStage, ref isMovingUp, ref TargetStage);
    } 

    bool GetPerfectTargetStage(int CurrentStage, ref bool isMovingUp, ref int TargetStage)
    {
        bool isAlreadyUp = isMovingUp;
        if (isMovingUp && PerfectTargetCalculate(UpQueue, CurrentStage, ref isMovingUp, ref TargetStage))
        {
            return true;
        }

        isMovingUp = false;

        if(PerfectTargetCalculate(DownQueue, CurrentStage, ref isMovingUp, ref TargetStage))
        {
            return true;
        }

        if (!isAlreadyUp)
        {
            isMovingUp = true;
            if (PerfectTargetCalculate(UpQueue, CurrentStage, ref isMovingUp, ref TargetStage))
            {
                return true;
            }
        }

        return false;
    }

    bool PerfectTargetCalculate(List<int> Queue, int CurrentStage, ref bool isMovingUp, ref int TargetStage)
    {
        for (int i = 0; i < Queue.Count; i++)
        {
            if (isMovingUp && Queue[i] > CurrentStage)
            {
                TargetStage = Queue[i];
                isMovingUp = true;
                return true;
            }
            if (!isMovingUp && Queue[i] < CurrentStage)
            {
                TargetStage = Queue[i];
                isMovingUp = false;
                return true;
            }
        }
        return false;
    }

    void GetAnyTargetStage(int CurrentStage, ref bool isMovingUp, ref int TargetStage)
    {
        if (isMovingUp)
        {
            try
            {
                TargetStage = UpQueue[0];
                isMovingUp = true;
                return;
            }
            catch { }
        }

        isMovingUp = false;

        try
        {
            TargetStage = DownQueue[0];
            isMovingUp = false;
            return;
        }
        catch { }
    }
}



