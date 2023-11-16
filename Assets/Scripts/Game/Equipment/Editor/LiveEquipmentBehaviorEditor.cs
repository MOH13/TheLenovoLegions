using UnityEditor;
using Unity.VisualScripting;
using UnityEngine;

namespace LL.Game.Equipment.Editor
{
    [CustomEditor(typeof(LiveEquipmentBehavior))]
    public class LiveEquipmentBehaviorEditor : UnityEditor.Editor
    {
        bool equipmentFoldout = false;

        public override void OnInspectorGUI()
        {
            var equipment = (LiveEquipmentBehavior)target;

            EditorGUILayout.PropertyField(serializedObject.FindPropertyOrFail("equipment").FindPropertyRelativeOrFail("<Container>k__BackingField"));

            EditorGUILayout.PropertyField(serializedObject.FindPropertyOrFail("stats"));

            equipmentFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(equipmentFoldout, "Equipment");

            if (equipmentFoldout)
            {
                EditorGUI.indentLevel++;

                var slots = equipment.Container.GetSlots();
                for (int i = 0; i < slots.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    var slot = slots[i].Slot;

                    EquipmentResource? piece = null;

                    if (Application.isPlaying)
                    {
                        piece = equipment.GetAtIndex(i);
                    }

                    var pieceStr =
                        piece != null
                        ? piece.EquipmentName ?? "EMPTY"
                        : "EMPTY";

                    GUILayout.Box(piece?.Icon, GUILayout.Width(50), GUILayout.Height(50));

                    EditorGUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField($"{slot.SlotName}", GUILayout.MinWidth(50));

                    GUI.enabled = piece != null;

                    if (GUILayout.Button(pieceStr))
                    {
                        Selection.activeObject = piece;
                    }

                    GUI.enabled = true;

                    if (Application.isPlaying)
                    {
                        EditorGUILayout.LabelField("Replace:", GUILayout.Width(65));
                        var newPiece = (EquipmentResource?)EditorGUILayout.ObjectField(null, typeof(EquipmentResource), false, GUILayout.Width(80));
                        if (newPiece != null)
                        {
                            equipment.EquipAtIndex(newPiece, i);
                        }
                        if (GUILayout.Button("X", GUILayout.Width(25)))
                        {
                            equipment.UnequipAtIndex(i);
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
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