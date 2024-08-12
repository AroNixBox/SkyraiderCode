using System;
using System.Collections;
using UnityEngine;

public static class AnimationHelper
{
    public static void WaitForAnimation(MonoBehaviour monoBehaviorToExecuteOn, Animator animator, string animationName, int layerIndex, Action callback)
    {
        monoBehaviorToExecuteOn.StartCoroutine(CoWaitForAnimation(animator, animationName, layerIndex, callback));
    }

    private static IEnumerator CoWaitForAnimation(Animator animator, string animationName, int layerIndex, Action callback)
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(animationName));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime > 0.95f);
        callback?.Invoke();
    }
}