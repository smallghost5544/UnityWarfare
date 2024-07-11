using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    public Camera myCamera;
    public Slider slider;
    public float dragSpeed = 0.02f; 

    private Vector3 dragOrigin;
    void Start()
    {
        
    }

   
    void Update()
    {
        myCamera.orthographicSize = slider.value;
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = myCamera.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        myCamera.transform.Translate(move, Space.World);
    }
    void MoveCameraByTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragOrigin = touch.position;
                return;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 touchPosition = touch.position;
                Vector3 direction = myCamera.ScreenToWorldPoint(dragOrigin) - myCamera.ScreenToWorldPoint(touchPosition);
                Debug.Log("move");
                myCamera.transform.position += direction * dragSpeed;
                dragOrigin = touch.position;
            }
        }
    }
}
