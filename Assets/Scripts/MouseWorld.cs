using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, mousePlaneLayerMask)) return;

        transform.position = raycastHit.point;
    }

    public Vector3 GetPosition() => transform.position;
}
