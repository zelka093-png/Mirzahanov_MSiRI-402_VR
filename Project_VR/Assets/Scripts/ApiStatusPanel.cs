using TMPro;
using UnityEngine;

// Панель станка (занятие 11.11): состояние «Работает / Остановлен», координаты X, Y, Z
// из GET-запроса и состояние связи. Кнопки «Пуск» и «Стоп» вызывают StartPressed / StopPressed.
public class ApiStatusPanel : MonoBehaviour
{
    public TMP_Text stateText;
    public TMP_Text axesText;
    public TMP_Text linkText;
    public MachineController machine;     // вращение пилы по кнопкам
    public float freshSeconds = 3f;       // сколько секунд данные считаются свежими

    private float lastData = -100f;
    private bool hadData;

    private void Start()
    {
        stateText.text = "Остановлен";
        axesText.text = "X: –   Y: –   Z: –";
        linkText.text = "Ждём ответ сервера…";
    }

    private void Update()
    {
        if (hadData && Time.unscaledTime - lastData > freshSeconds)
            linkText.text = "Нет свежих данных";
    }

    public void ShowAxes(float x, float y, float z)
    {
        axesText.text = $"X: {x:F1} мм\nY: {y:F1} мм\nZ: {z:F1} мм";
        linkText.text = "Связь есть";
        hadData = true;
        lastData = Time.unscaledTime;
    }

    public void ShowProblem(string text)
    {
        linkText.text = text;
    }

    public void StartPressed()
    {
        stateText.text = "Работает";
        if (machine) machine.StartMachine();
    }

    public void StopPressed()
    {
        stateText.text = "Остановлен";
        if (machine) machine.StopMachine();
    }
}
