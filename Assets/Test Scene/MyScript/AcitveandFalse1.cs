using UnityEngine;

[ExecuteInEditMode]
public class RectAreaMonitor1 : MonoBehaviour
{
    [Header("Rectangle Area Settings")]
    public Vector3 areaSize = new Vector3(4, 2, 3);
    public bool showOutline = true;
    public Transform centerFollowObj; // Drag this object to move the area center

    [Header("Detection Target Settings")]
    public Transform objP;
    public Transform objB;

    private bool hasTriggered = false; // Flag to ensure trigger only fires once

    void Update()
    {
        SyncAreaCenter();

        // Skip detection if already triggered
        if (hasTriggered) return;

        JudgeInsideArea();
    }

    // Make area center follow the assigned object
    void SyncAreaCenter()
    {
        if (centerFollowObj != null)
            transform.position = centerFollowObj.position;
    }

    // Pure mathematical judgment, no colliders or triggers used
    void JudgeInsideArea()
    {
        if (objP == null || objB == null) return;

        Bounds areaBox = new Bounds(transform.position, areaSize);
        bool inArea = areaBox.Contains(objP.position);

        if (inArea)
        {
            objB.gameObject.SetActive(false);
            hasTriggered = true; // Lock state, no further execution
        }
    }

    // Draw rectangular wireframe outline in Scene view
    void OnDrawGizmos()
    {
        if (!showOutline) return;

        Gizmos.color = Color.cyan;
        Bounds box = new Bounds(transform.position, areaSize);
        Gizmos.DrawWireCube(box.center, box.size);

        // Draw red marker for area center point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(box.center, 0.12f);
    }

    // Optional: Reset trigger state for testing in editor
    [ContextMenu("Reset Trigger State")]
    private void ResetTriggerFlag()
    {
        hasTriggered = false;
        if (objB != null)
            objB.gameObject.SetActive(true);
    }
}