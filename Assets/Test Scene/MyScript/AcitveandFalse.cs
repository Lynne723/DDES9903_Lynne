using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class RectAreaSwitcher : MonoBehaviour
{
    [Header("Area Settings")]
    public GameObject areaCenterMarker;
    public Vector3 areaSize = new(6, 4, 6);
    public bool showOutline = true;

    [Header("Object Targets")]
    public GameObject objA;
    public GameObject objB;
    public GameObject objC;

    [Header("Scene Jump Settings")]
    public string targetSceneName;
    public float waitSeconds = 10f;

    private bool triggered;
    private float timer;
    private bool bActivated;

    void Update()
    {
        // Skip if core objects missing
        if (objA == null || objB == null || objC == null) return;

        // First trigger: A enter area, switch B/C state
        if (!triggered)
        {
            if (IsInsideArea(objA.transform.position))
            {
                objB.SetActive(true);
                objC.SetActive(false);
                triggered = true;
                bActivated = true;
                Debug.Log($"{objA.name} entered area, switched objects");
            }
        }

        // Countdown and load scene after B activated
        if (bActivated)
        {
            timer += Time.deltaTime;
            if (timer >= waitSeconds)
            {
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }

    // Get real area center position
    private Vector3 GetCenter() => areaCenterMarker ? areaCenterMarker.transform.position : transform.position;

    // AABB math detection, no collision/trigger
    private bool IsInsideArea(Vector3 point)
    {
        Vector3 center = GetCenter();
        Vector3 half = areaSize / 2;
        bool x = point.x.IsBetween(center.x - half.x, center.x + half.x);
        bool y = point.y.IsBetween(center.y - half.y, center.y + half.y);
        bool z = point.z.IsBetween(center.z - half.z, center.z + half.z);
        return x && y && z;
    }

    // Draw scene view outline
    void OnDrawGizmos()
    {
        if (!showOutline) return;
        Vector3 center = GetCenter();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, areaSize);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(center, 0.15f);
    }

#if UNITY_EDITOR
    // Drag handle for area center when no marker assigned
    void OnSceneGUI()
    {
        if (areaCenterMarker != null) return;
        RectAreaSwitcher self = this;
        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.PositionHandle(GetCenter(), Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(self, "Move Area Center");
            self.transform.position = newPos;
            EditorUtility.SetDirty(self);
        }
    }
#endif
}

// Extension method to simplify range judgment
internal static class FloatExtension
{
    public static bool IsBetween(this float value, float min, float max)
    {
        return value >= min && value <= max;
    }
}