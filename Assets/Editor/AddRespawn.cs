using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AddRespawn
{
    [InitializeOnLoadMethod]
    static void AutoRun()
    {
        EditorApplication.delayCall -= Run;
        EditorApplication.delayCall += Run;
    }

    [MenuItem("Tools/Fix/Add PlayerRespawn to Levels")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        bool anyAdded = false;
        string[] sceneNames = { "Level1", "Level2" };

        foreach (var name in sceneNames)
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene " + name);
            if (guids.Length == 0) continue;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);

            bool alreadyOpen = false;
            Scene target = default;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (s.path == path) { target = s; alreadyOpen = true; break; }
            }
            if (!alreadyOpen)
                target = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

            bool sceneChanged = false;
            foreach (var root in target.GetRootGameObjects())
            {
                var ph = root.GetComponentInChildren<PlayerHealth>(true);
                if (ph == null) continue;

                var go = ph.gameObject;
                if (go.GetComponent<PlayerRespawn>() != null) continue;

                var pr = go.AddComponent<PlayerRespawn>();
                pr.killPlaneY = -15f;
                pr.fallDamage = 25f;
                EditorUtility.SetDirty(go);
                sceneChanged = true;
                anyAdded = true;
                Debug.Log("[AddRespawn] Added PlayerRespawn to player in " + name);
            }

            if (sceneChanged) EditorSceneManager.SaveScene(target);
            if (!alreadyOpen) EditorSceneManager.CloseScene(target, true);
        }

        if (!anyAdded)
            Debug.Log("[AddRespawn] PlayerRespawn already present in all levels.");
    }
}
