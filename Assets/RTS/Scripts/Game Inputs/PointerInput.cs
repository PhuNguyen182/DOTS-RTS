using UnityEngine;

public class PointerInput : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private Plane _plane;

    public static PointerInput Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _plane = new(Vector3.up, Vector3.zero);
    }

    public Vector3 GetMousePosition()
    {
        Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (_plane.Raycast(mouseRay, out float distance))
            return mouseRay.GetPoint(distance);

        return Vector3.zero;
    }
}
