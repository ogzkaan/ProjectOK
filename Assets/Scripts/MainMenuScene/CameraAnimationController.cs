using UnityEngine;

public class CameraAnimationController : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void SetAnimatonBool(bool animationbool)
    {
        animator.SetBool("CameraAnim",animationbool);
    }
    public void OnAnimationComplete()
    {
        MainMenu.Instance.StartSceneCouroutine();
    }

}
