using UnityEngine;
using UnityEngine.InputSystem;

public class TileDragManager : MonoBehaviour
{
    public static TileDragManager Instance;

    [SerializeField] private LayerMask draggableLayer;
    [SerializeField] private float dragHeight = 0.8f;
    [SerializeField] public float snapThreshold = 5f;

    private Camera mainCamera;
    private IDraggable currentDraggable;
    private IDraggable hoveredDraggable;
    private Vector3 dragOffset;
    private bool isDragging = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCamera = Camera.main;
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        if (Mouse.current == null) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryStartDrag();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            EndDrag();
        }
        else if (isDragging)
        {
            UpdateDrag();
        }
        else
        {
            HandleHover();
        }
    }
    private void TryStartDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, draggableLayer))
        {
            var draggable = hit.transform.GetComponent<IDraggable>();
            if (draggable != null)
            {
                currentDraggable = draggable;
                isDragging = true;
                dragOffset = hit.transform.position - GetMouseWorldPosition();
                currentDraggable.OnStartDrag();
               
            }
        }
    }
    public void StartDragging(IDraggable draggable)
    {
        if (draggable == null) return;

        currentDraggable = draggable;
        isDragging = true;
        dragOffset = Vector3.zero; // Start dragging without an offset
        draggable.OnStartDrag();
    }
    private void UpdateDrag()
    {
        if (currentDraggable != null)
        {
            Vector3 targetPosition = GetMouseWorldPosition() + dragOffset;
            targetPosition.y += dragHeight; // Lift the tile while dragging
            currentDraggable.OnDrag(targetPosition);
        }
    }
    private void EndDrag()
    {
        if (currentDraggable != null)
        {
            currentDraggable.OnEndDrag();
            currentDraggable = null;
        }
        isDragging = false;
    }
    private Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Camera.main.transform.forward, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }
    private void HandleHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, draggableLayer))
        {
            var draggable = hit.transform.GetComponent<IDraggable>();
            if (draggable != null && draggable != hoveredDraggable && hit.transform.CompareTag("Tile"))
            {
                if (hoveredDraggable != null)
                {
                    hoveredDraggable.OnHoverExit();
                }
                hoveredDraggable = draggable;
                hoveredDraggable.OnHoverEnter();
            }
        }
        else if (hoveredDraggable != null)
        {
            hoveredDraggable.OnHoverExit();
            hoveredDraggable = null;
        }
    }

}

