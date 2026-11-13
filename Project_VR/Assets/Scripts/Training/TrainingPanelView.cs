using TMPro;
using UnityEngine;

// Панель обучения: список всех шагов (текущий подсвечен), подсказка и итог.
// Логики здесь нет, панель только показывает то, что передаёт сценарий.
public class TrainingPanelView : MonoBehaviour
{
    public TMP_Text stepList;     // слева: все шаги по порядку
    public TMP_Text stepLabel;    // в заголовке: текущий шаг
    public TMP_Text hintLabel;    // что делать сейчас
    public TMP_Text resultLabel;  // итог в конце

    private static readonly string[] Names =
    {
        "Знакомство", "Проверка", "Взять заготовку", "Установка",
        "Ограждение (нет)", "Пуск", "Резка", "Снять деталь", "Итог"
    };

    public void ShowStep(int number, string title)
    {
        stepLabel.text = $"Шаг {number}/9: {title}";
        string list = "";
        for (int i = 0; i < Names.Length; i++)
        {
            int n = i + 1;
            if (n < number) list += $"<color=#3a9a4a>+ {n}. {Names[i]}</color>\n";
            else if (n == number) list += $"<b><color=#e07a10>► {n}. {Names[i]}</color></b>\n";
            else list += $"<color=#8a8f98>  {n}. {Names[i]}</color>\n";
        }
        stepList.text = list;
    }

    public void ShowHint(string text)
    {
        hintLabel.text = text;
    }

    public void ShowResult(string text)
    {
        resultLabel.text = text;
    }
}
