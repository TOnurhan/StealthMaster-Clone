using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Waypoint : MonoBehaviour
{
    private void Start()
    {
        transform.name = transform.parent.name + "'s Waypoints";
        transform.SetParent(transform.parent.parent);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.GetChild(i).transform.position, 0.5f);
        }
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.GetChild(i).transform.position, transform.GetChild(i + 1).position);
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Waypoint))]
[System.Serializable]
class WaypointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Waypoint script = (Waypoint)target;
        if (GUILayout.Button("Add Waypoint", GUILayout.MinWidth(100), GUILayout.Width(100)))
        {
            GameObject yeniObje = new GameObject();
            yeniObje.transform.parent = script.transform;
            yeniObje.transform.position = script.transform.position;
            yeniObje.name = script.transform.childCount.ToString();
        }
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
}
#endif
