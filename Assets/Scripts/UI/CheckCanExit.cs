using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SharedBetweenAnimatorsAttribute]
public class CheckCanExit : StateMachineBehaviour
{
    public static int count = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("CanExit", false);
        count++;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (count + animator.GetInteger("UIInterrupt") > 1)
        {
            count--;
            animator.SetBool("CanExit", true);
            animator.SetInteger("UIInterrupt", 0);
        }
    }
}
