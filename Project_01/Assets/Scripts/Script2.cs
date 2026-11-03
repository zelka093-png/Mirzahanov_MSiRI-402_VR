
using UnityEngine;

public class RobotController : MonoBehaviour
{
    // Данные оператора
    public string operatorName = "Зелимхан";
    private Robot robot;

    // Получаем компонент робота при запуске
    private void Start()
    {
        robot = GetComponent<Robot>();

        if (robot == null)
        {
            Debug.LogError("На объекте отсутствует компонент Robot!");
            return;
        }

        ControlRobot();
    }

    // Управление роботом
    public void ControlRobot()
    {
        Debug.Log("Оператор: " + operatorName);
        Debug.Log("Выбран робот: " + robot.robotName);
        Debug.Log("Скорость робота: " + robot.speed);
        Debug.Log("Заряд батареи перед заданием: " + robot.Battery + "%");

        robot.DoTask();

        Debug.Log("Информация после выполнения задания:");
        robot.ShowInfo();
    }
}
