using UnityEditor;
using UnityEngine;

namespace LL.Game.Equipment.Editor
{
    [CustomEditor(typeof(InventoryBehavior))]
    public class InventoryBehaviorEditor : UnityEditor.Editor
    {
        bool inventoryFoldout = false;

        EquipmentResource? pieceToAdd = null;

        public override void OnInspectorGUI()
        {
            var inventory = (InventoryBehavior)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("inventory").FindPropertyRelative("<CurrentEquipment>k__BackingField"));

            if (Application.isPlaying)
            {
                inventoryFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(inventoryFoldout, "Current Inventory");

                EquipmentResource? pieceToEquip = null;

                if (inventoryFoldout)
                {
                    pieceToAdd = (EquipmentResource?)EditorGUILayout.ObjectField("Equipment to add", pieceToAdd, typeof(EquipmentResource), false);

                    if (GUILayout.Button("Add") && pieceToAdd != null)
                    {
                        inventory.AddToInventory(pieceToAdd);
                    }

                    EditorGUI.indentLevel++;

                    var equipment = inventory.GetView();
                    foreach (var piece in equipment)
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Box(piece.Icon, GUILayout.Width(50), GUILayout.Height(50));

                        EditorGUILayout.BeginVertical();
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField($"{piece.Slot.SlotName}", GUILayout.MinWidth(50));

                        if (GUILayout.Button(piece.EquipmentName))
                        {
                            Selection.activeObject = piece;
                        }

                        GUI.enabled = true;

                        if (Application.isPlaying)
                        {
                            if (GUILayout.Button("Equip"))
                            {
                                pieceToEquip = piece;
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

                if (pieceToEquip != null) inventory.Equip(pieceToEquip);
            }

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