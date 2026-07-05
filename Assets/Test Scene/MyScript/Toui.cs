using UnityEngine;
using System.Collections;

public class Toui : MonoBehaviour
{
    [Header("Delay Settings")]
    // Waiting time before activating target object (unit: seconds)
    public float waitSeconds = 3f;

    [Header("Target Object")]
    // The object that will be activated after delay
    public GameObject targetObj;

    private bool isTriggered = false;

    void Start()
    {
        // Start timing coroutine when game begins
        StartCoroutine(DelayActiveCoroutine());
    }

    // Countdown coroutine
    IEnumerator DelayActiveCoroutine()
    {
        // Skip if already triggered
        if (isTriggered) yield break;

        // Wait for specified time
        yield return new WaitForSeconds(waitSeconds);

        // Safety check
        if (targetObj != null)
        {
            targetObj.SetActive(true);
            isTriggered = true;
        }
    }

    // Right-click component menu to reset timer for testing
    [ContextMenu("Reset Timer & Deactivate Target")]
    private void ResetTimer()
    {
        isTriggered = false;
        StopAllCoroutines();
        if (targetObj != null)
        {
            targetObj.SetActive(false);
        }
        // Restart countdown
        StartCoroutine(DelayActiveCoroutine());
    }
}