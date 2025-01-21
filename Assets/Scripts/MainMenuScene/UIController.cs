using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // Ana kamera
    [SerializeField] private Camera uiCamera; // UI elementlerini çeken kamera
    [SerializeField] private RenderTexture renderTexture; // UI'ın yansıtıldığı render texture
    [SerializeField] private Canvas uiCanvas; // UI elementlerinin bulunduğu canvas
    [SerializeField] private MeshRenderer planeRenderer; // Render texture'ın uygulandığı plane
    [SerializeField] private GameObject pointer;

    private PointerColisionHandle pointerColisionHandle;
    private Button button;
    CameraAnimationController cameraAnimationController;


    private void Start()
    {
        if (uiCanvas == null)
        {
            Debug.LogError("Canvas referansı eksik!");
            return;
        }
        pointerColisionHandle = pointer.GetComponent<PointerColisionHandle>();
        cameraAnimationController = mainCamera.GetComponent<CameraAnimationController>();
    }

    private void Update()
    {
        HandleMovement();
        
        HandleInteraction();
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
        
    }
    private void HandleMovement()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == planeRenderer.gameObject)
        {
            Cursor.visible = false;
            // UV'den ekran koordinatlarına çevir
            Vector2 uv = hit.textureCoord;
            Vector2 screenPoint = new Vector2(
                uv.x * renderTexture.width,
                uv.y * renderTexture.height
            );
            RectTransform rectTransform = pointer.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = (screenPoint/251)*1920;

        }
        else
        {
            Cursor.visible = true;
        }
        
    }
    private void HandleInteraction()
    {
        if (pointerColisionHandle.GetCurrentTrigger())
        {
            GameObject currentTrigger = pointerColisionHandle.GetCurrentTrigger();
            button = currentTrigger.GetComponent<Button>();
            if (button != null)
            {
                ButtonSelectedAnimaton(button);
            }
        }
        else
        {
            if (button != null)
            {
                ButtonDeselectAnimation(button);
            }
        }

        
    }
    private void HandleClick()
    {
        if (pointerColisionHandle.GetCurrentTrigger())
        {
            Button btn = pointerColisionHandle.GetCurrentTrigger().GetComponent<Button>();
            Cursor.visible = true;
            cameraAnimationController.SetAnimatonBool(true);
            btn.onClick.Invoke();

        }
    }
    private void ButtonSelectedAnimaton(Button button)
    {
        TextMeshProUGUI btntxt = button.GetComponentInChildren<TextMeshProUGUI>();
        btntxt.fontSize = 80;
        btntxt.color = Color.white;
    }
    private void ButtonDeselectAnimation(Button button)
    {
        TextMeshProUGUI btntxt = button.GetComponentInChildren<TextMeshProUGUI>();

        btntxt.fontSize = 70;
        btntxt.color = new Color(0.5f, 0.5f, 0.5f);
    }
}
