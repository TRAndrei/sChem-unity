using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var wheelDelt = Input.GetAxis("Mouse ScrollWheel");

        if (wheelDelt != 0)
        {
            var delt = Mathf.Max(-1, Mathf.Min(1, (wheelDelt)));

            Vector3 centerPoint = Camera.main.transform.position;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float xAdjust = (mousePosition.x - centerPoint.x) * delt;
            float yAdjust = (mousePosition.y - centerPoint.y) * delt;

            float cameraZoom = Camera.main.orthographicSize;

            float deltaZoom = - yAdjust * cameraZoom / (mousePosition.y - centerPoint.y);

            //Debug.Log("Camera " + centerPoint + " LMouse " + Input.mousePosition  + " WMouse " + mousePosition + " Zoom " + cameraZoom + " delta " + deltaZoom);

            if (!float.IsInfinity(Camera.main.orthographicSize + deltaZoom))
            {
                Camera.main.orthographicSize += deltaZoom;
                Camera.main.transform.Translate(xAdjust, yAdjust, 0);
            }
        }
    }
}
