using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RaycastDebugger : MonoBehaviour
{
    [SerializeField] private bool doDebug = false;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    [SerializeField] private EventSystem m_EventSystem;

    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();

        m_PointerEventData = new PointerEventData(m_EventSystem);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && doDebug)
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and specified position from list
            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count > 0)
                Debug.Log($"Clicked on object \"{results[0].gameObject.name}\"");
        }
    }
}
