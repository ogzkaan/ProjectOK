using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class TileDraggable : MonoBehaviour, IDraggable
{
    private Vector3 originalPosition;
    private Transform originalParent;
    private TileSlotManager currentSlot;

    public Transform GetTransform() => transform;

    private Vector3 initialPosition;
    private Vector3 initialRotation;
    private Vector3 initialScale;

    private Vector3 initalColliderSize;
    private Vector3 initalColliderCenter;

    BoxCollider boxCollider;

    [SerializeField]private bool onDiscard = false;

    private Tile tile;
    private void Awake()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localEulerAngles;
        initialScale = transform.localScale;

        boxCollider = GetComponent<BoxCollider>();

        initalColliderSize = boxCollider.size = new Vector3(0.09421441f, 0.007363497f, 0.06595007f);
        initalColliderCenter = boxCollider.center = Vector3.zero;

    }
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
            if (onDiscard)
            {
                this.transform.SetParent(null);
                this.gameObject.tag = "Untagged";
                this.gameObject.layer = 0;
                this.enabled = false;
                OkeyGameManager.instance.DiscardTile(this);
                if (currentSlot != null)
                {
                    currentSlot.Highlight(false);
                }

            }
            else if(!onDiscard)
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
        OkeyGameManager.instance.CheckSets();
        
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
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
    }
   public void OnHoverEnter()
    {
        transform.DOLocalMove(initialPosition + new Vector3(0, 0.02f, 0), 0.1f).SetEase(Ease.OutBack);
        SetColider();
    }

    public void OnHoverExit()
    {
        if (this.transform.CompareTag("Tile"))
        {
            transform.DOLocalMove(initialPosition, 0.1f).SetEase(Ease.InOutSine);
            ResetCollider();
        }
        
    }
    public void SetOnDiscard(bool onDiscard)
    {
        this.onDiscard = onDiscard;
    }
    public bool GetOnDiscard()
    {
        return this.onDiscard;
    }
    public Tile GetTile()
    {
        return tile;
    }
    public void SetTile(Tile newTile)
    {
        tile = newTile;
    }
    public void ResetCollider()
    {
        boxCollider.size = initalColliderSize;
        boxCollider.center = initalColliderCenter;
    }
    public void SetColider()
    {
        boxCollider.size = new Vector3(0.09421441f, 0.05025354f, 0.06595007f);
        boxCollider.center = new Vector3(-8.278369e-10f, -0.02144508f, -5.363562e-17f);
    }

}