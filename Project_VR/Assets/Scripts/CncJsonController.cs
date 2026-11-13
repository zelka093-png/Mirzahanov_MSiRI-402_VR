using System;
using System.Collections;
using System.Globalization;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

// Клиент сервера станка (занятие 09.11), перенесён в VR-проект для панели (занятие 11.11).
// Раз в interval секунд делает GET-запрос, достаёт из JSON координаты X, Y, Z и
// передаёт их на панель. Узлы станка двигаются, только если они назначены.
public class CncJsonController : MonoBehaviour
{
    public string url = "https://cnc.kovalev.team/get/3";
    public int channel = 1;
    public float interval = 0.5f;

    public Transform nodeX;
    public Transform nodeY;
    public Transform nodeZ;
    public ApiStatusPanel panel;

    private Vector3 startX, startY, startZ;

    private IEnumerator Start()
    {
        if (nodeX) startX = nodeX.localPosition;
        if (nodeY) startY = nodeY.localPosition;
        if (nodeZ) startZ = nodeZ.localPosition;

        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 10;
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        Apply(request.downloadHandler.text);
                    }
                    catch (Exception error)
                    {
                        Debug.LogWarning("Ошибка чтения JSON: " + error.Message);
                        if (panel) panel.ShowProblem("Ошибка в данных сервера");
                    }
                }
                else
                {
                    Debug.LogWarning("Ошибка запроса: " + request.error);
                    if (panel) panel.ShowProblem("Нет связи с сервером");
                }
            }
            yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, interval));
        }
    }

    private void Apply(string json)
    {
        JObject data = JObject.Parse(json);
        float x = GetCoordinate(data, "X");
        float y = GetCoordinate(data, "Y");
        float z = GetCoordinate(data, "Z");
        if (panel) panel.ShowAxes(x, y, z);

        // в метрах для Unity
        if (nodeX) nodeX.localPosition = startX + Vector3.right * x * 0.001f;
        if (nodeY) nodeY.localPosition = startY + Vector3.forward * y * 0.001f;
        if (nodeZ) nodeZ.localPosition = startZ + Vector3.up * z * 0.001f;
    }

    private float GetCoordinate(JObject data, string axis)
    {
        foreach (JToken section in data["data"])
        {
            if ((string)section[0] != "Оси Канала " + channel) continue;
            foreach (JToken row in section[1])
            {
                if ((string)row[0] != "Перемещение по оси " + axis) continue;
                string text = ((string)row[1]).Replace("мм", "").Trim().Replace(',', '.');
                return float.Parse(text, CultureInfo.InvariantCulture);
            }
        }
        throw new FormatException("Не найдена координата " + axis);
    }
}
