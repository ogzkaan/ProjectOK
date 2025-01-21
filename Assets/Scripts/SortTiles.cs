using UnityEngine;
using UnityEngine.InputSystem;

public class SortTiles : MonoBehaviour
{
    private Camera _camera;

    private GameObject _hoveredObject;
    private Animator _hoveredAnimator;
    private void Awake()
    {
        _camera = Camera.main;
    }
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckSort();
        }
        HoverAnimation();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void CheckSort()
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.rigidbody != null && hit.rigidbody.transform.CompareTag("Sort"))
            {
                if (hit.transform.gameObject.name =="Number")
                {
                    OkeyGameManager.instance.SortByNumber();
                }
                else if(hit.transform.gameObject.name == "Color")
                {
                    OkeyGameManager.instance.SortByColor();
                }
                else if (hit.transform.gameObject.name == "Open")
                {
                    OkeyGameManager.instance.OpenHand();
                }
                OkeyGameManager.instance.ResetSetAndHighlight();
            }

        }
    }
    private void HoverAnimation()
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.rigidbody != null && hit.rigidbody.transform.CompareTag("Sort"))
            {
                GameObject hitObject = hit.transform.gameObject;
                if (_hoveredObject != hitObject)
                {
                    ResetHover();
                    _hoveredObject = hitObject;

                    if (_hoveredObject.CompareTag("Sort"))
                    {
                        _hoveredAnimator = _hoveredObject.GetComponent<Animator>();
                        if (_hoveredAnimator != null)
                        {
                            _hoveredAnimator.SetBool("IsHovered", true);
                        }
                    }
                }   
            }
            else
            {
                ResetHover();
            }
        }
    }
    private void ResetHover()
    {
        if (_hoveredAnimator != null)
        {
            _hoveredAnimator.SetBool("IsHovered", false);
        }

        _hoveredObject = null;
        _hoveredAnimator = null;
    }
}
