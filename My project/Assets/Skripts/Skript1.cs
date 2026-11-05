using UnityEngine;

public class Robot : MonoBehaviour
{
    public string robotName = "MyRobot";
    public int speed = 10;

    private int battery = 100;

    public int Battery
    {
        get { return battery; }
    }

    private void Start()
    {
        ShowInfo();
    }

    public void ShowInfo()
    {
        Debug.Log("Имя робота: " + robotName);
        Debug.Log("Скорость: " + speed);
        Debug.Log("Заряд: " + battery + "%");
    }

    public void DoTask()
    {
        if (battery >= 20)
        {
            battery -= 20;
            Debug.Log(robotName + " выполнил задание.");
            Debug.Log("Остаток заряда: " + battery + "%");
        }
        else
        {
            Debug.Log("Недостаточно заряда!");
        }
    }
}