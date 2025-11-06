#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject.Editor
{
    public static class PlayerStatsResetEditor
    {
        [MenuItem("PLAYER TWO/Reset PlayerStats Defaults (all)")]
        public static void ResetAllPlayerStatsDefaults()
        {
            // Default values from PlayerStats.cs
            float defaultTopSpeed = 6f;
            float defaultRunningTopSpeed = 7.5f;
            float defaultMaxJumpHeight = 17f;
            float defaultMinJumpHeight = 10f;

            var guids = AssetDatabase.FindAssets("t:PlayerStats");
            int count = 0;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path) as ScriptableObject;
                if (asset == null) continue;

                var playerStats = asset as PLAYERTWO.PlatformerProject.PlayerStats;
                if (playerStats == null) continue;

                Undo.RecordObject(playerStats, "Reset PlayerStats Defaults");
                playerStats.topSpeed = defaultTopSpeed;
                playerStats.runningTopSpeed = defaultRunningTopSpeed;
                playerStats.maxJumpHeight = defaultMaxJumpHeight;
                playerStats.minJumpHeight = defaultMinJumpHeight;
                EditorUtility.SetDirty(playerStats);
                count++;
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"Reset {count} PlayerStats asset(s) to defaults.");
        }
    }
}
#endif
