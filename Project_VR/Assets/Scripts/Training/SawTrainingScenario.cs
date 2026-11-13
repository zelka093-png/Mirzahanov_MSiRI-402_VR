using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Учебный сценарий работы на станке-пиле.
// Шаги идут строго по порядку: на каждом шаге разрешено только своё действие,
// неверные действия считаются ошибками и показываются в итоге.
public class SawTrainingScenario : MonoBehaviour
{
    // Номера шагов как в методичке: шаг 5 (ограждение) у пилы не используется и пропускается
    public enum Step
    {
        Briefing = 1,   // ознакомление с рабочим местом
        Inspection = 2, // проверка исходного состояния
        TakeBlank = 3,  // взять заготовку
        MountBlank = 4, // установить заготовку на станок
        Ready = 6,      // запуск
        Cutting = 7,    // идёт резка
        Unload = 8,     // снять готовую деталь
        Deliver = 81,   // отнести деталь на стол продукции (часть шага 8)
        Finished = 9    // результат
    }

    [Header("Заготовка и места")]
    public BlankGrabber blank;
    public Transform startPlace;      // где заготовка лежит в начале
    public Transform mountPlace;      // точное место заготовки на станке
    public Transform productPlace;    // точное место на столе продукции
    public DropZone mountZone;        // зона, куда надо отпустить заготовку
    public DropZone productZone;      // зона стола продукции

    [Header("Станок")]
    public MachineController machine; // вращение диска пилы
    public Transform sawHead;         // узел, который опускается при резке
    public float cutTime = 6f;
    public float cutDepth = 0.18f;   // на сколько опускается диск
    public float feedLength = 0.35f; // на сколько головка ходит вдоль стола
    public Color rawColor = new Color(0.6f, 0.6f, 0.62f);
    public Color cutColor = new Color(0.95f, 0.6f, 0.15f);

    [Header("Панель")]
    public TrainingPanelView panel;

    public Step Current { get; private set; }

    private Vector3 headStart;
    private Coroutine cutting;
    private bool mounted, cut, carried;
    private int seenRelease;
    private float startedAt;
    private float lastButton = -10f;

    // счётчик ошибок по видам: название вида -> количество
    private readonly Dictionary<string, int> mistakes = new Dictionary<string, int>();

    private void Start()
    {
        if (blank == null || startPlace == null || mountPlace == null || productPlace == null ||
            mountZone == null || productZone == null || sawHead == null || panel == null)
        {
            Debug.LogError("SawTrainingScenario: не все ссылки заполнены в Inspector");
            enabled = false;
            return;
        }
        blank.scenario = this;
        headStart = sawHead.localPosition;
        ResetAll();
    }

    private void Update()
    {
        // отпускание проверяется один раз, когда рука уже закончила движение
        if (!blank.InHand && blank.Releases != seenRelease && Time.time - blank.LastReleaseTime > 0.15f)
        {
            seenRelease = blank.Releases;
            CheckRelease();
        }
    }

    // ---------- кнопки панели ----------

    private bool Accept()
    {
        // защита от двойного срабатывания одной кнопки
        if (Time.unscaledTime - lastButton < 0.3f) return false;
        lastButton = Time.unscaledTime;
        return true;
    }

    public void OnNext()
    {
        if (!Accept()) return;
        switch (Current)
        {
            case Step.Briefing:
                GoTo(Step.Inspection);
                break;
            case Step.Inspection:
                string problem = InspectionProblem();
                if (problem != null) panel.ShowHint(problem);
                else GoTo(Step.TakeBlank);
                break;
            default:
                panel.ShowHint("«Далее» используется только на первых шагах. Сейчас: " + HintFor(Current));
                break;
        }
    }

    public void OnStart()
    {
        if (!Accept()) return;
        if (Current == Step.Cutting) { Mistake("Повторный пуск", "Станок уже режет, дождитесь окончания."); return; }
        if (Current != Step.Ready || !mounted || blank.InHand)
        {
            Mistake("Пуск без заготовки", "Пуск запрещён: сначала установите заготовку на станок.");
            return;
        }
        GoTo(Step.Cutting);
        cutting = StartCoroutine(CutRoutine());
    }

    public void OnStop()
    {
        if (!Accept()) return;
        if (Current != Step.Cutting) { panel.ShowHint("Станок не работает."); return; }
        StopCutting();
        GoTo(Step.Ready);
        panel.ShowHint("Резка остановлена. Заготовка на месте, нажмите «Пуск», чтобы начать заново.");
    }

    public void OnRestart()
    {
        if (!Accept()) return;
        if (blank.InHand) { panel.ShowHint("Сначала положите деталь, потом нажмите «Сначала»."); return; }
        ResetAll();
    }

    // ---------- вызовы от заготовки ----------

    public bool MayGrab()
    {
        return Current == Step.TakeBlank || Current == Step.MountBlank || Current == Step.Unload || Current == Step.Deliver;
    }

    public void BlankTaken()
    {
        if (Current == Step.TakeBlank) GoTo(Step.MountBlank);
        else if (Current == Step.Unload) { carried = true; GoTo(Step.Deliver); }
    }

    private void CheckRelease()
    {
        Vector3 p = blank.transform.position;
        if (Current == Step.MountBlank)
        {
            if (mountZone.Contains(p))
            {
                blank.PlaceAt(mountPlace);
                mounted = true;
                GoTo(Step.Ready);
            }
            else Mistake("Неверная установка", "Заготовка не на станке. Возьмите её и положите в зону станка.");
        }
        else if (Current == Step.Deliver && carried)
        {
            if (cut && productZone.Contains(p))
            {
                blank.PlaceAt(productPlace);
                GoTo(Step.Finished);
                panel.ShowResult(ResultText());
            }
            else Mistake("Деталь не на столе продукции", "Положите готовую деталь на стол продукции.");
        }
    }

    // ---------- резка ----------

    private IEnumerator CutRoutine()
    {
        if (machine != null) machine.StartMachine();     // диск начинает вращаться
        float t = 0f;
        while (t < cutTime)
        {
            t += Time.deltaTime;
            // головка с диском опускается на заготовку, ходит вдоль стола и поднимается
            float k = Mathf.Clamp01(t / cutTime);
            float down = Mathf.Clamp01(Mathf.Min(k, 1f - k) * 5f) * cutDepth;
            float feed = Mathf.Sin(k * Mathf.PI * 2f) * feedLength;
            sawHead.localPosition = headStart + Vector3.down * down + Vector3.right * feed;
            yield return null;
        }
        StopCutting();
        cut = true;
        blank.Paint(cutColor);
        GoTo(Step.Unload);
    }

    private void StopCutting()
    {
        if (cutting != null) { StopCoroutine(cutting); cutting = null; }
        if (machine != null) machine.StopMachine();
        sawHead.localPosition = headStart;
    }

    // ---------- вспомогательное ----------

    private string InspectionProblem()
    {
        if (blank.InHand) return "Заготовка должна лежать на столе, а не в руке.";
        if (Vector3.Distance(blank.transform.position, startPlace.position) > 0.3f) return "Заготовка должна лежать на столе заготовок.";
        if (mountZone.Contains(blank.transform.position)) return "Место на станке должно быть свободно.";
        if (sawHead.localPosition != headStart) return "Головка пилы должна быть в верхнем положении.";
        return null;
    }

    private void ResetAll()
    {
        StopCutting();
        mounted = cut = carried = false;
        mistakes.Clear();
        blank.ReturnTo(startPlace);
        blank.Paint(rawColor);
        seenRelease = blank.Releases;
        startedAt = Time.time;
        panel.ShowResult("");
        GoTo(Step.Briefing);
    }

    private void GoTo(Step s)
    {
        Current = s;
        panel.ShowStep(StepNumber(s), TitleFor(s));
        panel.ShowHint(HintFor(s));
    }

    private void Mistake(string kind, string text)
    {
        mistakes.TryGetValue(kind, out int n);
        mistakes[kind] = n + 1;
        panel.ShowHint(text);
    }

    private string ResultText()
    {
        int total = 0;
        foreach (var m in mistakes) total += m.Value;
        string s = $"Готово за {Time.time - startedAt:F0} с, ошибок: {total}";
        foreach (var m in mistakes) s += $"\n{m.Key}: {m.Value}";
        return s;
    }

    private static int StepNumber(Step s) => s == Step.Deliver ? 8 : (int)s;

    private static string TitleFor(Step s)
    {
        switch (s)
        {
            case Step.Briefing: return "Знакомство с рабочим местом";
            case Step.Inspection: return "Проверка станка";
            case Step.TakeBlank: return "Взять заготовку";
            case Step.MountBlank: return "Установить заготовку";
            case Step.Ready: return "Запуск пилы";
            case Step.Cutting: return "Резка";
            case Step.Unload: return "Снять деталь";
            case Step.Deliver: return "Отнести деталь";
            default: return "Итог";
        }
    }

    private static string HintFor(Step s)
    {
        switch (s)
        {
            case Step.Briefing: return "Посмотрите на пилу, стол заготовок и стол продукции. Нажмите «Далее».";
            case Step.Inspection: return "Заготовка на столе, станок свободен, головка пилы вверху. Нажмите «Далее».";
            case Step.TakeBlank: return "Возьмите заготовку со стола заготовок.";
            case Step.MountBlank: return "Положите заготовку в зону станка под пилой и отпустите.";
            case Step.Ready: return "Заготовка закреплена. Нажмите «Пуск».";
            case Step.Cutting: return "Идёт резка, дождитесь остановки пилы.";
            case Step.Unload: return "Возьмите готовую деталь со станка.";
            case Step.Deliver: return "Отнесите деталь на стол продукции и отпустите.";
            default: return "Обучение закончено. «Сначала» – пройти ещё раз.";
        }
    }
}
