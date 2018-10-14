using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Elevator_UIController : MonoBehaviour {

    public static Elevator_UIController THIS;

    int StagesCount;
    [SerializeField] Text StagesInput;

    [SerializeField]GameObject StageButtonPrefab;
    [SerializeField] GameObject ElevatorButtonPrefab;

    [SerializeField] GameObject StartScreen;
    [SerializeField] GameObject PlayScreen;

    [SerializeField] GameObject StagesButtonGrid;
    [SerializeField] GameObject ElevatorButtonGrid;

    GameObject[] StagesButtons;
    GameObject[] ElevatorsButtons;

    public static Elevator ElevatorObject;

    private void Start()
    {
        ElevatorObject = gameObject.AddComponent<Elevator>();
        THIS = this;
        StartScreen.SetActive(true);
        PlayScreen.SetActive(false);
    }

    public void EnterStageCount()
    {
        int.TryParse(StagesInput.text, out StagesCount);
        StagesCount = Mathf.Clamp(StagesCount, 1, int.MaxValue);

        StartScreen.SetActive(false);
        PlayScreen.SetActive(true);

        GenerateButtons();
        GenerateContentRect();
    }

    private void GenerateContentRect()
    {
        ConfigureContentRect(StagesButtonGrid, StagesCount * 100); // TODO: Remove hardcoding
        ConfigureContentRect(ElevatorButtonGrid, ((int)ElevatorsButtons.Length / (int)8 + 1) * 103);
    }

    void ConfigureContentRect(GameObject Grid, int Size)
    {
        RectTransform rectContent = Grid.GetComponent<RectTransform>();
        Vector2 coord = rectContent.sizeDelta;
        coord.y = Size;
        rectContent.sizeDelta = coord;
    }

    void GenerateButtons()
    {
        ConfigureButtonsList(StagesButtonGrid, StageButtonPrefab, out StagesButtons);
        ConfigureButtonsList(ElevatorButtonGrid, ElevatorButtonPrefab, out ElevatorsButtons, 1);
    }

    void ConfigureButtonsList(GameObject Grid, GameObject Prefab, out GameObject[] ButtonArray, int AdditionalButtonsNum = 0)
    {
        while (Grid.transform.childCount < StagesCount + AdditionalButtonsNum)
        {
            Instantiate(Prefab, Grid.transform);
        }

        ButtonArray = new GameObject[StagesCount];
        IButtonElevator[] a = new IButtonElevator[StagesCount];

        for (int i = 0; i < StagesCount; i++)
        {
            a[i] = GetComponent<IButtonElevator>();
            ButtonArray[i] = Grid.transform.GetChild(i).gameObject;
            ButtonArray[i].GetComponent<IButtonElevator>().ConfigureButton(StagesCount - i);
        }
    }

    public void ChangeElevatorIconVisibility(int Stage, bool isActive)
    {
        if (Stage < 1)
            return;

        StagesButtons[StagesCount - Stage].GetComponent<Elevator_UIButtonContoller>().ChangeElevatorIconVisibility(isActive);
    }
    public void ChangeDoorStatusVisibility(int Stage, Elevator.DoorState doorState)
    {
        if (Stage < 1)
            return;

        StagesButtons[StagesCount - Stage].GetComponent<Elevator_UIButtonContoller>().ChangeDoorStatusVisibility(doorState);
    }


}
