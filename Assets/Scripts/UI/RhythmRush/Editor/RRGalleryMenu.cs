using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RhythmRush.UI;

namespace RhythmRush.UI.EditorTools
{
    /// <summary>
    /// Convenience menu items so you don't have to wire anything by hand:
    /// spawn the screen gallery into the open scene, or open a fresh scene with it.
    /// Find them under <b>Tools ▸ RhythmRush</b>.
    /// </summary>
    public static class RRGalleryMenu
    {
        [MenuItem("Tools/RhythmRush/Create Screen Gallery (current scene)")]
        public static void CreateInCurrentScene()
        {
            var go = Spawn();
            Undo.RegisterCreatedObjectUndo(go, "Create RhythmRush Screen Gallery");
            Selection.activeGameObject = go;
            EditorGUIUtility.PingObject(go);
            Debug.Log("[RhythmRush] Screen Gallery created — press Play, then use ← / → (or 1–4) to switch screens.");
        }

        [MenuItem("Tools/RhythmRush/Open Screen Gallery (new scene)")]
        public static void OpenInNewScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "RhythmRush Screens";
            Spawn();
            Debug.Log("[RhythmRush] Empty scene with the Screen Gallery is open — press Play to view the screens.");
        }

        static GameObject Spawn()
        {
            var go = new GameObject("RhythmRush Screen Gallery");
            go.AddComponent<RRScreenGallery>();
            return go;
        }
    }
}
