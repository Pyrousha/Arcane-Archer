#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LevelsParent : MonoBehaviour
{
    public void SetIndicesOfChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<LevelButton>().Index = i;
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(LevelsParent))]
public class LevelsParentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelsParent myScript = (LevelsParent)target;
        if (GUILayout.Button("Set Indices of Children"))
        {
            myScript.SetIndicesOfChildren();
        }
    }
}
#endif