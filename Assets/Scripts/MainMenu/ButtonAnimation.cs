using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour
{

    private float maxSize = 1.1f;

    private bool hover;

    private void Awake()
    {

        EventTrigger trigger;

        if (!GetComponent<EventTrigger>())
        {
            trigger = gameObject.AddComponent(typeof(EventTrigger)) as EventTrigger;
        }
        else
        {
            trigger = GetComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { SwapHover(); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { SwapHover(); });
        trigger.triggers.Add(entry);

    }

    private void OnDisable()
    {

        transform.localScale = Vector3.one;
        hover = false;

    }

    private void Update()
    {

        if (!hover)
        {
            DecreaseSize();
        }
        else
        {
            IncreaseSize();
        }

    }

    private void DecreaseSize()
    {

        if (transform.localScale != Vector3.one)
        {

            transform.localScale -= Vector3.one * Time.unscaledDeltaTime;

            if (transform.localScale.y < 1) { transform.localScale = Vector3.one; }

        }

    }

    private void IncreaseSize()
    {

        if (transform.localScale.y < maxSize)
        {

            transform.localScale += Vector3.one * Time.unscaledDeltaTime;

        }

    }

    private void SwapHover()
    {

        hover = !hover;

    }

}