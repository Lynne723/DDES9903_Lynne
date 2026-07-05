using UnityEngine;

[ExecuteInEditMode]
public class ControlMusic : MonoBehaviour
{
    [Header("Rectangle Area Settings")]
    // Size of rectangular detection area
    public Vector3 areaSize = new Vector3(4, 2, 3);
    // Toggle display of area wireframe in Scene view
    public bool showOutline = true;
    // Drag this target object to move the entire area center
    public Transform centerFollowObj;

    [Header("Target Objects")]
    // Object P to detect position
    public Transform objP;
    // Object B to toggle active state
    public Transform objB;

    // Record previous inside state to avoid repeated SetActive calls
    private bool lastIsInside = false;

    void Update()
    {
        SyncAreaCenterPosition();
        CheckObjectPositionState();
    }

    // Synchronize area center with assigned follow object
    void SyncAreaCenterPosition()
    {
        if (centerFollowObj != null)
        {
            transform.position = centerFollowObj.position;
        }
    }

    // Calculate bounds and judge if objP is inside rectangle area
    void CheckObjectPositionState()
    {
        if (objP == null || objB == null) return;

        Bounds areaBounds = new Bounds(transform.position, areaSize);
        bool currentIsInside = areaBounds.Contains(objP.position);

        // Only execute state switch when inside state changes
        if (currentIsInside != lastIsInside)
        {
            objB.gameObject.SetActive(currentIsInside);
            lastIsInside = currentIsInside;
        }
    }

    // Draw rectangular wireframe outline and center marker in Scene view
    void OnDrawGizmos()
    {
        if (!showOutline) return;

        Bounds areaBounds = new Bounds(transform.position, areaSize);
        // Draw area wireframe
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(areaBounds.center, areaBounds.size);

        // Draw red sphere mark for area center
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(areaBounds.center, 0.12f);
    }

    // Right click component menu to reset state for testing
    [ContextMenu("Reset Detection State")]
    private void ResetState()
    {
        lastIsInside = false;
        if (objB != null)
        {
            objB.gameObject.SetActive(false);
        }
    }
}