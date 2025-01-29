using UnityEngine;

public class DiscardTile : MonoBehaviour
{
    TileDraggable tileDraggable;
    private void OnTriggerEnter(Collider other)
    {
        tileDraggable = other.gameObject.GetComponent<TileDraggable>();
        tileDraggable.SetOnDiscard(true);
    }
    private void OnTriggerExit(Collider other)
    {
        tileDraggable.SetOnDiscard(false);
        
    }
}
