using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using System.IO;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LL.Game.Equipment
{
    [CreateAssetMenu(fileName = "EquipmentDatabase", menuName = "Equipment/Database", order = 3)]
    public class EquipmentDatabase : ScriptableObject
    {
        static EquipmentDatabase Instance => Resources.Load<EquipmentDatabase>("EquipmentDatabase");

        [SerializeField]
        EquipmentResource[] pieces = new EquipmentResource[0];

        public static EquipmentResource? GetFromAssetName(string assetName)
        {
            foreach (var piece in Instance.pieces)
            {
                if (piece.name == assetName)
                    return piece;
            }
            return null;
        }

#if UNITY_EDITOR
        [MenuItem("LL/Update equipment database")]
        public static void UpdateDatabase()
        {
            var instance = Instance;
            Undo.RecordObject(instance, "Update database");
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(EquipmentResource)}", null);
            List<EquipmentResource> pieces = new();
            foreach (string guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path))
                    continue;
                var piece = AssetDatabase.LoadAssetAtPath<EquipmentResource>(path);
                pieces.Add(piece);
            }
            instance.pieces = pieces.ToArray();
            EditorUtility.SetDirty(instance);
        }
#endif
    }
}