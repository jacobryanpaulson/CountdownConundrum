using UnityEngine;
using UnityEditor;

public class PuzzleWarpTool : EditorWindow
{
    private int selectedIndex = 0;

    [MenuItem("Tools/Puzzle Warper")]
    public static void ShowWindow()
    {
        GetWindow<PuzzleWarpTool>("Puzzle Warper");
    }

    private void OnGUI()
    {
        GUILayout.Label("Instant Puzzle Warper", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        
        GameManager gm = FindFirstObjectByType<GameManager>();

        if (gm == null)
        {
            EditorGUILayout.HelpBox("GameManager not found in this scene!", MessageType.Error);
            return;
        }

      
        if (gm.spawnPoints == null || gm.spawnPoints.Length == 0 || gm.puzzleCam == null || gm.puzzleCam.Length == 0)
        {
            EditorGUILayout.HelpBox("Make sure Spawn Points and Puzzle Cameras are assigned on GameManager.", MessageType.Warning);
            return;
        }

       
        int maxIndex = gm.spawnPoints.Length - 1;
        selectedIndex = EditorGUILayout.IntSlider("Target Puzzle Index", selectedIndex, 0, maxIndex);

       
        EditorGUILayout.LabelField($"Active GameManager Index: {gm.currentPuzzleIndex}");

        EditorGUILayout.Space();

        
        if (Application.isPlaying && gm.CurrentState == GameState.MainMenu)
        {
            EditorGUILayout.HelpBox("The game is paused in MainMenu. Warp will work, but you may need to start the game to move.", MessageType.Info);
        }

        if (GUILayout.Button("Warp to Selected Puzzle", GUILayout.Height(35)))
        {
            ExecuteWarp(gm);
        }
    }

    private void ExecuteWarp(GameManager gm)
    {
       
        if (gm.levelGoals != null)
        {
            for (int i = 0; i < gm.levelGoals.Length; i++)
            {
                if (gm.levelGoals[i] != null)
                {
                    Undo.RecordObject(gm.levelGoals[i], "Reset Goal State");
                    gm.levelGoals[i].ResetGoal();
                }
            }
        }

        
        gm.currentPuzzleIndex = selectedIndex;

       
        if (gm.player != null)
        {
            Undo.RecordObject(gm.player.transform, "Warp Player Position");
            
            Transform targetSpawn = gm.spawnPoints[selectedIndex];
            if (targetSpawn != null)
            {
                gm.player.transform.position = targetSpawn.position;
                gm.player.transform.rotation = targetSpawn.rotation;
                
                // Update LoopManager's tracking coordinates
                if (LoopManager.Instance != null)
                {
                    LoopManager.Instance.UpdateSpawnPoint(targetSpawn);
                }
            }

            // Sync structural parameters to the PlayerController
            if (gm.player.TryGetComponent<PlayerController>(out PlayerController pc))
            {
                Undo.RecordObject(pc, "Update Player Variables");

                // Assign the target puzzle's LevelGoal references
                if (gm.levelGoals != null && selectedIndex < gm.levelGoals.Length)
                {
                    pc.UpdatePuzzleReferences(gm.levelGoals[selectedIndex]);
                }

                // Update move limits for the target index
                if (gm.puzzleMoveLimits != null && selectedIndex < gm.puzzleMoveLimits.Length)
                {
                    pc.ResetMoves(gm.puzzleMoveLimits[selectedIndex]);
                }
            }
        }

      
        if (gm.puzzleCam != null)
        {
            for (int i = 0; i < gm.puzzleCam.Length; i++)
            {
                if (gm.puzzleCam[i] != null)
                {
                    Undo.RecordObject(gm.puzzleCam[i].gameObject, "Toggle Puzzle Camera");
                    gm.puzzleCam[i].gameObject.SetActive(i == selectedIndex);
                }
            }
        }

        
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gm.gameObject.scene);
        }

        Debug.Log($"[DevTool] Safely forced warp to puzzle scene index: {selectedIndex}");
    }
}
