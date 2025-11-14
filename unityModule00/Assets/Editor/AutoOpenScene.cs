using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class AutoOpenScene
{
    static AutoOpenScene()
    {
        // Path relative to Assets folder
        string path = "Assets/Scenes/FloorIsLavaScene.unity";

        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorSceneManager.OpenScene(path);
        }
    }
}
