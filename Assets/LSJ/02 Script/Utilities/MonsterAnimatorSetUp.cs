#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

public class MonsterAnimatorSetup : EditorWindow
{
    AnimatorController templateController;
    AnimationClip idleClip;
    AnimationClip attackClip;
    AnimationClip deadClip;

    [MenuItem("Tools/Monster Animator Setup")]
    public static void ShowWindow()
    {
        GetWindow<MonsterAnimatorSetup>("Monster Animator Setup");
    }

    private void OnGUI()
    {
        templateController = (AnimatorController)EditorGUILayout.ObjectField("Template Controller", templateController, typeof(AnimatorController), false);
        idleClip = (AnimationClip)EditorGUILayout.ObjectField("Idle Clip", idleClip, typeof(AnimationClip), false);
        attackClip = (AnimationClip)EditorGUILayout.ObjectField("Attack Clip", attackClip, typeof(AnimationClip), false);
        deadClip = (AnimationClip)EditorGUILayout.ObjectField("Dead Clip", deadClip, typeof(AnimationClip), false);

        if (GUILayout.Button("Apply Clips"))
        {
            foreach (var layer in templateController.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    if (state.state.name == "idle") state.state.motion = idleClip;
                    if (state.state.name == "attack") state.state.motion = attackClip;
                    if (state.state.name == "dead") state.state.motion = deadClip;
                }
            }
        }
    }
}
#endif