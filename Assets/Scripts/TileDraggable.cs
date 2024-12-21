using UnityEngine;

public class TileDraggable : MonoBehaviour, IDraggable
{
    private Vector3 originalPosition;
    private Transform originalParent;
    private TileSlotManager currentSlot;

    public Transform GetTransform() => transform;

   public void OnStartDrag()
    {
        if (CompareTag("Tile"))
        {
            originalPosition = transform.position;
            originalParent = transform.parent;
            transform.SetParent(TileDragManager.Instance.transform);
        }
    }

    public void OnDrag(Vector3 position)
    {
        if (CompareTag("Tile"))
        {
            transform.position = position;

            // Find the closest slot while dragging
            TileSlotManager nearestSlot = FindNearestSlot();
            if (nearestSlot != null)
            {
                // Highlight the nearest slot
                nearestSlot.Highlight(true);

                // If there was a previously highlighted slot, remove its highlight
                if (currentSlot != null && currentSlot != nearestSlot)
                {
                    currentSlot.Highlight(false);
                }

                // Update the current slot reference
                currentSlot = nearestSlot;
            }
            else
            {
                // Clear highlight from the previously highlighted slot if no nearest slot is found
                if (currentSlot != null)
                {
                    currentSlot.Highlight(false);
                    currentSlot = null;
                }
            }
        }

    }

    public void OnEndDrag()
    {
        if (CompareTag("Tile"))
        {
            TileSlotManager targetSlot = FindNearestSlot();

            if (targetSlot != null && targetSlot.CanAcceptTile())
            {
                // Snap to new slot
                PlaceInSlot(targetSlot);
            }
            else
            {
                // Return to original position
                ReturnToOriginalPosition();
            }

            // Clear highlights
            if (currentSlot != null)
            {
                currentSlot.Highlight(false);
            }
        }
        
    }

    private TileSlotManager FindNearestSlot()
    {
        TileSlotManager[] slots = FindObjectsOfType<TileSlotManager>();
        TileSlotManager nearest = null;
        float minDistance = float.MaxValue;

        foreach (var slot in slots)
        {
            float distance = Vector3.Distance(transform.position, slot.transform.position);
            if (distance < minDistance && slot.CanAcceptTile())
            {
                minDistance = distance;
                nearest = slot;
            }
        }

        return minDistance <= TileDragManager.Instance.snapThreshold ? nearest : null;
    }

    private void PlaceInSlot(TileSlotManager slot)
    {
        transform.SetParent(slot.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.localRotation = Quaternion.identity;
    }
}