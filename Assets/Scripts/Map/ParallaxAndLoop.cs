using System.Runtime.CompilerServices;
using UnityEngine;

public class ParallaxAndLoop : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float parallaxSpeed = 0.1f;
    [SerializeField] float objectWidth = 18f;

    private Transform[] objects;
    private Vector3 lastCameraPosition;

    private void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraPosition = cameraTransform.position;

        objects = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            objects[i] = transform.GetChild(i);
        }

        SpriteRenderer sr = objects[0].GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            objectWidth = sr.bounds.size.x;
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

