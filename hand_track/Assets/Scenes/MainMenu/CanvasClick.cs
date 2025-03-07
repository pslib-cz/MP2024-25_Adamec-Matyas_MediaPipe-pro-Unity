using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class CanvasClick : MonoBehaviour
{
    public Canvas canvas;
    public static CanvasClick Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SimulateClickAtScreenPoint(Vector2 screenPoint)
    {
        Debug.Log("Simulating click at: " + screenPoint);
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPoint
        };

        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
        gr.Raycast(pointerData, results);

        if (results.Count > 0)
        {
            GameObject clickable = ExecuteEvents.GetEventHandler<IPointerClickHandler>(results[0].gameObject);

            if (clickable != null)
            {
                ExecuteEvents.Execute(clickable, pointerData, ExecuteEvents.pointerClickHandler);
                Debug.Log("Clicked on: " + clickable.name);
            }
            else
            {
                Debug.Log("No clickable object found at: " + screenPoint);
            }
        }
        else
        {
            Debug.Log("No UI element found at: " + screenPoint);
        }

    }
}
