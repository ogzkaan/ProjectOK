using UnityEngine;

public interface IDraggable
{
    Transform GetTransform();
    void OnStartDrag();
    void OnDrag(Vector3 position);
    void OnEndDrag();
}

