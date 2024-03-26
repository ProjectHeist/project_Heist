using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public void SetRunning(bool run)
    {
        animator.SetBool("run", run);
    }
}
