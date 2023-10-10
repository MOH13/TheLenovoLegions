using UnityEditor;
using Unity.VisualScripting;
using UnityEngine;
using LL.Framework.Stats;

namespace LL.Game.Stats.Editor
{
    [CustomEditor(typeof(LiveStatsBehavior))]
    public class LiveStatsBehaviorEditor : UnityEditor.Editor
    {
        bool statsFoldout = false, effectsFoldout = false;

        EffectResource? effectToAdd;

        public override void OnInspectorGUI()
        {
            var stats = (LiveStatsBehavior)target;

            EditorGUILayout.PropertyField(serializedObject.FindPropertyOrFail("stats").FindPropertyRelativeOrFail("<Container>k__BackingField"));

            statsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(statsFoldout, "Stats");

            if (statsFoldout)
            {
                EditorGUI.indentLevel++;

                if (GUILayout.Button("Recalculate"))
                {
                    stats.RecalculateStats();
                }

                foreach (var item in stats.Container.GetStats())
                {
                    var statString =
                        Application.isPlaying
                        ? $"{stats.GetValue(item.Stat)} (base {item.BaseValue})"
                        : item.BaseValue.ToString();

                    EditorGUILayout.LabelField(item.Stat.StatName, statString);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (Application.isPlaying)
            {
                effectsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(effectsFoldout, "Effects");

                if (effectsFoldout)
                {
                    EditorGUI.indentLevel++;

                    effectToAdd = (EffectResource?)EditorGUILayout.ObjectField("Effect to add", effectToAdd, typeof(EffectResource), false);

                    if (GUILayout.Button("Add") && effectToAdd != null)
                    {
                        stats.ApplyEffect(effectToAdd);
                    }

                    EffectResource? toRemove = null, toAdd = null;

                    foreach (var (effect, amount) in stats.GetEffectCollectionView())
                    {
                        EditorGUILayout.BeginHorizontal();

                        var effectLabel = $"{EffectUtils.Describe<StatResource, EffectResource>(effect)} ({effect.EffectName})";

                        EditorGUILayout.LabelField(effectLabel);
                        EditorGUILayout.LabelField(amount.ToString(), GUILayout.Width(50));

                        if (GUILayout.Button("-", GUILayout.Width(50)))
                        {
                            toRemove = effect;
                        }
                        if (GUILayout.Button("+", GUILayout.Width(50)))
                        {
                            toAdd = effect;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    if (toRemove != null) stats.RemoveEffect(toRemove);
                    if (toAdd != null) stats.ApplyEffect(toAdd);

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }

        // public override VisualElement CreateInspectorGUI()
        // {
        //     var root = new VisualElement();

        //     var stats = (LiveStatsBehavior)target;

        //     var statsFoldout = new Foldout();
        //     statsFoldout.text = "Stats";
        //     root.Add(statsFoldout);

        //     if (stats.Container != null)
        //     {
        //         foreach (var item in stats.Container.GetStats())
        //         {
        //             var itemElement = new VisualElement();
        //             itemElement.style.flexDirection = FlexDirection.Row;
        //             statsFoldout.Add(itemElement);

        //             var statElement = new Label(item.Stat.StatName);
        //             itemElement.Add(statElement);

        //             var valueElement = new Label(item.BaseValue.ToString());
        //             itemElement.Add(valueElement);
        //         }
        //     }

        //     return root;
        // }
    }
}