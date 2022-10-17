using CustomBuildSystem.Placing;
using UnityEngine;
using UnityEditor;


public class TempEditor
{
    [MenuItem("Utils/AutoAssign")]
    public static void HalfAll()
    {
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            PlaceableMonoBase mono = gameObject.GetComponent<PlaceableMonoBase>();
            foreach (Transform child in gameObject.transform)
            {
                if (child.name.ToLower().Contains("okay")) mono.placingOkay = child.gameObject;
                if (child.name.ToLower().Contains("error")) mono.placingError = child.gameObject;
                if (child.name.ToLower().Contains("placed")) mono.placed = child.gameObject;
            }
            EditorUtility.SetDirty(gameObject);
        }

        AssetDatabase.Refresh();
    }
}