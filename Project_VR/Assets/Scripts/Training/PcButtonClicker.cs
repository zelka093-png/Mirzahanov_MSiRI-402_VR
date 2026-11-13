using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

// Проверка без шлема: кнопки панелей нажимаются левой кнопкой мыши.
// Когда шлем подключён, скрипт ничего не делает – кнопки нажимаются рукой.
public class PcButtonClicker : MonoBehaviour
{
    public float maxDistance = 15f;

    private void Update()
    {
        if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Camera cam = Camera.main;
        if (cam == null) return;
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, maxDistance))
        {
            Button button = hit.collider.GetComponent<Button>();
            if (button != null && button.interactable) button.onClick.Invoke();
        }
    }
}
