using UnityEditor;

public static class ToolkitUpdateTest
{
    [MenuItem("Game-Toolkit/Developer/Update Test")]
    private static void ShowUpdateTest()
    {
        EditorUtility.DisplayDialog(
            "Toolkit Update Test",
            "Success! This menu item was installed by the toolkit update.",
            "OK");
    }
}
