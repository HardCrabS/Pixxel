using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taskboard : MonoBehaviour
{
    [SerializeField] GameObject taskboard;
    Animator animator;
    // Use this for initialization
    void Start()
    {
        animator = taskboard.GetComponent<Animator>();
    }

    public void ShowBoard()
    {
        animator.SetBool("slidingIn", !animator.GetBool("slidingIn"));
    }
}
