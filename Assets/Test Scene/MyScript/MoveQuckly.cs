using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class RectTeleportArea : MonoBehaviour
{
    [Header("Rectangle Area Settings")]
    // Drag any scene object to control area center, higher priority than areaCenter vector
    public GameObject areaCenterRef;
    public Vector3 areaSize = new Vector3(5, 3, 5);
    public bool drawAreaOutline = true;

    [Header("Teleport Config")]
    public GameObject checkTarget;
    // Drag scene object as teleport destination marker
    public GameObject teleportMarkerObj;

    private Vector3 lastTelePos;

    // Get real area center position, auto follow reference object if assigned
    private Vector3 GetActualAreaCenter()
    {
        if (areaCenterRef != null)
            return areaCenterRef.transform.position;
        return transform.position;
    }

    void Update()
    {
        if (checkTarget == null || teleportMarkerObj == null) return;

        // Prevent infinite teleport loop inside region
        if (Vector3.Distance(checkTarget.transform.position, lastTelePos) < 0.05f)
            return;

        if (IsPointInsideRectArea(checkTarget.transform.position))
        {
            ExecuteTeleport();
        }
    }

    /// <summary>
    /// Pure mathematical AABB volume detection, no collider or trigger
    /// </summary>
    bool IsPointInsideRectArea(Vector3 point)
    {
        Vector3 center = GetActualAreaCenter();
        Vector3 halfExtent = areaSize * 0.5f;

        bool insideX = point.x >= center.x - halfExtent.x && point.x <= center.x + halfExtent.x;
        bool insideY = point.y >= center.y - halfExtent.y && point.y <= center.y + halfExtent.y;
        bool insideZ = point.z >= center.z - halfExtent.z && point.z <= center.z + halfExtent.z;
        return insideX && insideY && insideZ;
    }

    /// <summary>
    /// Move target object to teleport marker's world position
    /// </summary>
    void ExecuteTeleport()
    {
        Vector3 dest = teleportMarkerObj.transform.position;
        checkTarget.transform.position = dest;
        lastTelePos = dest;
        Debug.Log($"[Teleport Trigger] {checkTarget.name} teleported to {teleportMarkerObj.name}");
    }

    /// <summary>
    /// Draw visualization in Scene view
    /// </summary>
    void OnDrawGizmos()
    {
        if (!drawAreaOutline) return;
        Vector3 center = GetActualAreaCenter();

        // Draw rectangular detection zone wireframe
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, areaSize);

        // Draw area center marker
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(center, 0.15f);

        // Draw teleport destination marker
        if (teleportMarkerObj != null)
        {
            Gizmos.color = Color.magenta;
            Vector3 tpPos = teleportMarkerObj.transform.position;
            Gizmos.DrawWireSphere(tpPos, 0.2f);
            Gizmos.DrawLine(center, tpPos);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Scene view editing handles, backup control when no reference object assigned
    /// </summary>
    void OnSceneGUI()
    {
        RectTeleportArea self = this;
        Vector3 center = GetActualAreaCenter();

        // Only show manual position handle if no reference object linked
        if (self.areaCenterRef == null)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(center, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(self, "Adjust Area Center");
                self.transform.position = newPos;
                EditorUtility.SetDirty(self);
            }
        }
    }
#endif
}