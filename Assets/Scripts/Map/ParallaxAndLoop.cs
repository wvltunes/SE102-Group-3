using System.Runtime.CompilerServices;
using UnityEngine;

public class MapScrolling : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float parallaxSpeed = 0.1f;
    [SerializeField] float objectWidth = 18f;

    private Transform[] objects;
    private Vector3 lastCameraPosition;

    private void Start()
    {
        //Use 3 objects to create a seamless loop
        lastCameraPosition = cameraTransform.position;
        objects = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            objects[i] = transform.GetChild(i);
        }
    }

    private void LateUpdate()
    {
        float camDeltaX = cameraTransform.position.x - lastCameraPosition.x;
        transform.position += Vector3.right * camDeltaX * parallaxSpeed;
        lastCameraPosition = cameraTransform.position;

        foreach (Transform obj in objects)
        {
            float camToObjX = cameraTransform.position.x - obj.position.x;

            if (camToObjX > objectWidth * 1.5f)
            {
                //Move object n widths to the right to loop it back
                obj.position += Vector3.right * objectWidth * objects.Length;
            }
             else if (camToObjX < -objectWidth)
            {
                //Move object n widths to the left to loop it back
                obj.position -= Vector3.right * objectWidth * objects.Length;
            }
        }
    }
}

