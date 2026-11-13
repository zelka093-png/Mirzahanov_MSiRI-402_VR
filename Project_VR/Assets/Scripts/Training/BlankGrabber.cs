using UnityEngine;
using Valve.VR.InteractionSystem;

// Заготовка, которую берут рукой SteamVR. Используется вместо Throwable:
// перед захватом спрашивает сценарий, можно ли сейчас брать деталь.
[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Rigidbody))]
public class BlankGrabber : MonoBehaviour
{
    [HideInInspector] public SawTrainingScenario scenario;
    public Renderer look;   // что перекрашивается после резки

    public bool InHand { get; private set; }
    public int Releases { get; private set; }
    public float LastReleaseTime { get; private set; }

    private Interactable interactable;
    private Rigidbody body;

    // рука тянет деталь физикой, поэтому деталь не проходит сквозь станок
    private const Hand.AttachmentFlags Flags =
        Hand.AttachmentFlags.DetachFromOtherHand |
        Hand.AttachmentFlags.VelocityMovement |
        Hand.AttachmentFlags.TurnOffGravity;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        body = GetComponent<Rigidbody>();
        if (look == null) look = GetComponentInChildren<Renderer>();
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes grab = hand.GetGrabStarting();
        if (grab == GrabTypes.None || InHand) return;
        if (scenario == null || !scenario.MayGrab()) return;   // сейчас брать нельзя
        hand.HoverLock(interactable);
        hand.AttachObject(gameObject, grab, Flags);
    }

    private void HandAttachedUpdate(Hand hand)
    {
        if (!hand.IsGrabEnding(gameObject)) return;
        hand.DetachObject(gameObject);
        hand.HoverUnlock(interactable);
    }

    private void OnAttachedToHand(Hand hand)
    {
        InHand = true;
        body.isKinematic = false;
        scenario.BlankTaken();
    }

    private void OnDetachedFromHand(Hand hand)
    {
        InHand = false;
        body.isKinematic = false;
        body.useGravity = true;
        body.linearVelocity = Vector3.zero;   // деталь не бросается, а просто кладётся
        body.angularVelocity = Vector3.zero;
        LastReleaseTime = Time.time;
        Releases++;
    }

    // поставить точно в точку и закрепить
    public void PlaceAt(Transform point)
    {
        body.isKinematic = true;
        Teleport(point);
    }

    // вернуть на стол заготовок при «Повторить»
    public void ReturnTo(Transform point)
    {
        InHand = false;
        transform.SetParent(null);
        body.isKinematic = true;
        Teleport(point);
        body.isKinematic = false;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }

    public void Paint(Color c)
    {
        if (look != null) look.material.color = c;
    }

    // двигаем и Transform, и Rigidbody, иначе интерполяция вернёт деталь назад
    private void Teleport(Transform point)
    {
        transform.SetPositionAndRotation(point.position, point.rotation);
        body.position = point.position;
        body.rotation = point.rotation;
    }
}
