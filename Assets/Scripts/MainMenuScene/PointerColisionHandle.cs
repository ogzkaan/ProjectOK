using UnityEngine;

public class PointerColisionHandle : MonoBehaviour
{
    private GameObject currentTrigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentTrigger = collision.gameObject;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        currentTrigger = null;
    }
    public GameObject GetCurrentTrigger()
    {
        return currentTrigger;
    }
}
