#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.IO;
using System.Collections.Generic;

public class MonsterBatchAnimatorTool : EditorWindow
{
    AnimatorController templateController;
    string animationsFolder = "Assets/LSJ/05 Animation/Monsters/";

    [MenuItem("Tools/Monster Batch Animator Tool")]
    public static void ShowWindow()
    {
        GetWindow<MonsterBatchAnimatorTool>("Monster Batch Animator Tool");
    }

    private void OnGUI()
    {
        templateController = (AnimatorController)EditorGUILayout.ObjectField("Template Controller", templateController, typeof(AnimatorController), false);
        animationsFolder = EditorGUILayout.TextField("Animations Folder", animationsFolder);

        if (GUILayout.Button("Apply All Monsters"))
        {
            ApplyAllMonsters();
        }
    }

    private void ApplyAllMonsters()
    {
        if (templateController == null)
        {
            Debug.LogError("Template Controller를 지정해주세요!");
            return;
        }

        // 폴더 내 모든 애니메이션 파일 가져오기
        string[] files = Directory.GetFiles(animationsFolder, "*.anim", SearchOption.AllDirectories);

        // 몬스터 이름별로 Idle/Attack/Dead 매칭
        Dictionary<string, Dictionary<string, AnimationClip>> monsterClips = new Dictionary<string, Dictionary<string, AnimationClip>>();

        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(file);

            // 파일명 규칙: MonsterName_State (예: Goblin_Idle)
            string[] parts = fileName.Split('_');
            if (parts.Length != 2) continue;

            string monsterName = parts[0];
            string stateName = parts[1];

            if (!monsterClips.ContainsKey(monsterName))
                monsterClips[monsterName] = new Dictionary<string, AnimationClip>();

            monsterClips[monsterName][stateName] = clip;
        }

        // 몬스터별 Override Controller 생성
        foreach (var kvp in monsterClips)
        {
            string monsterName = kvp.Key;
            var clips = kvp.Value;

            AnimatorOverrideController overrideController = new AnimatorOverrideController(templateController);

            if (clips.ContainsKey("Idle")) overrideController["Idle"] = clips["Idle"];
            if (clips.ContainsKey("Attack")) overrideController["Attack"] = clips["Attack"];
            if (clips.ContainsKey("Dead")) overrideController["Dead"] = clips["Dead"];

            // Override Controller 저장
            string savePath = $"{animationsFolder}{monsterName}_Override.controller";
            AssetDatabase.CreateAsset(overrideController, savePath);

            Debug.Log($"{monsterName} Override Controller 생성 완료: {savePath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif