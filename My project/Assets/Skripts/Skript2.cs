using UnityEngine;

public class RobotController : MonoBehaviour
{
    public string operatorName = "Зелимхан";

    private Robot robot;

    private void Start()
    {
        robot = GetComponent<Robot>();

        if (robot == null)
        {
            Debug.LogError("Компонент Robot не найден!");
            return;
        }

        ControlRobot();
    }

    public void ControlRobot()
    {
        Debug.Log("Оператор: " + operatorName);
        Debug.Log("Робот: " + robot.robotName);
        Debug.Log("Скорость: " + robot.speed);
        Debug.Log("Заряд до задания: " + robot.Battery + "%");

        robot.DoTask();
        robot.ShowInfo();
    }
}