
using UnityEngine;

public class Robot : MonoBehaviour
{
    // Основные характеристики робота
    public string robotName = "Terminator";
    public int speed = 15;
    private int battery = 100;

    // Получение заряда батареи
    public int Battery => battery;

    // При запуске объекта выводим информацию
    private void Awake()
    {
        ShowInfo();
    }

    // Вывод информации о роботе
    public void ShowInfo()
    {
        Debug.Log("Робот: " + robotName);
        Debug.Log("Скорость передвижения: " + speed);
        Debug.Log("Уровень заряда: " + battery + "%");
    }

    // Выполнение задания роботом
    public void DoTask()
    {
        if (battery >= 20)
        {
            battery -= 20;
            Debug.Log(robotName + " успешно выполнил задание!");
            Debug.Log("Остаток заряда: " + battery + "%");
        }
        else
        {
            Debug.Log("Недостаточно заряда для выполнения задания!");
        }
    }
}
