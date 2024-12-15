using UnityEngine;

public class PickableComponent : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private float groundY;
    private bool attached = false;

    void Start()
    {
        mainCamera = Camera.main;
        groundY = transform.position.y;
    }

    void OnMouseDown()
    {
        attached = !attached;
        offset = transform.position - GetMouseWorldPosition();
    }

    void Update()
    {
        if (attached)
        {
            Vector3 targetPosition = GetMouseWorldPosition() + offset;
            transform.position = new Vector3(targetPosition.x, groundY, targetPosition.z);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, groundY, 0));

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }
}
