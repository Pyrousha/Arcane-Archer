using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "List of Sprite Assets")]
public class ListOfTmpSpriteAssets : ScriptableObject
{
    public List<TMP_SpriteAsset> SpriteAssets;
}


#if UNITY_EDITOR
[CustomEditor(typeof(ListOfTmpSpriteAssets))]
public class ListOfTmpSpriteAssets_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update"))
        {
            List<TMP_SpriteAsset> spriteAssets = ((ListOfTmpSpriteAssets)target).SpriteAssets;

            foreach (var asset in spriteAssets)
            {
                foreach (var glyph in asset.spriteGlyphTable)
                {
                    glyph.scale = 2;

                    var currMetrics = glyph.metrics;
                    currMetrics.horizontalBearingY = currMetrics.height * 3f / 4f;

                    glyph.metrics = currMetrics;
                }
            }
        }
    }
}
#endif