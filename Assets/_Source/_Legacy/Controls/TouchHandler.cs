using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHandler : MonoBehaviour
{
    private Camera mainCamera;
    private Collider2D lastSelectedCollider;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButton(0)) // Зажата левая кнопка мыши
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            ProcessPointer(mousePos);
        }
        else
        {
            lastSelectedCollider = null;
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = mainCamera.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                ProcessPointer(touchPos);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                lastSelectedCollider = null;
            }
        }
        else
        {
            lastSelectedCollider = null;
        }
#endif
    }

    void ProcessPointer(Vector2 worldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && hit.collider != lastSelectedCollider)
        {
            lastSelectedCollider = hit.collider;
            var clickHandler = hit.collider.GetComponent<CardClickHandler>();
            if (clickHandler != null)
            {
                clickHandler.InvokeSelect();
            }
        }
    }
}
