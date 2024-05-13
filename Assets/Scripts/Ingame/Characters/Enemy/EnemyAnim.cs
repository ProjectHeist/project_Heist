using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnim : MonoBehaviour
{
    public Animator animator;
    public void SetRunning(bool run)
    {
        animator.SetBool("run", run);
    }
}
