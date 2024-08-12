using System;
using System.Collections;
using UnityEngine;

namespace Extensions
{
    public static class TimerUtility
    {
        public static void SetTimer(MonoBehaviour invoker, Action action, float delay)
        {
            invoker.StartCoroutine(ExecuteAfterDelay(action, delay));
        }

        private static IEnumerator ExecuteAfterDelay(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}