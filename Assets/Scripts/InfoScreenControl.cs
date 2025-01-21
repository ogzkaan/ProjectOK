using UnityEngine;

public class InfoScreenControl : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (animator.GetBool("InfoTransition"))
            {
                animator.SetBool("InfoTransition", false);
            }
            else
            {
                animator.SetBool("InfoTransition", true);
            }
        }
    }
}
