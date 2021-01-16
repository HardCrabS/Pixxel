using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    [SerializeField] GameObject objectToDisable;
    public void PlayAnimationAndDisableGO(string animation)
    {
        Animator animator = objectToDisable.GetComponent<Animator>();
        animator.Play(animation);
        StartCoroutine(DisableGameObjectDelayed(animator.GetCurrentAnimatorStateInfo(0).length));
    }

    IEnumerator DisableGameObjectDelayed(float time)
    {
        yield return new WaitForSeconds(time);
        objectToDisable.SetActive(false);
    }
}
