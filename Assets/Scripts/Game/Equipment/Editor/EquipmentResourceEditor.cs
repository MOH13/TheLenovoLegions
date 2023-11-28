using UnityEditor;
using UnityEngine;

namespace LL.Game.Equipment.Editor
{
    [CustomEditor(typeof(EquipmentResource))]
    public class EquipmentResourceEditor : UnityEditor.Editor
    {
        public override Texture2D? RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {

            EquipmentResource piece = (EquipmentResource)target;

            if (piece == null || piece.Icon == null)
                return null;

            // example.PreviewIcon must be a supported format: ARGB32, RGBA32, RGB24,
            // Alpha8 or one of float formats
            Texture2D tex = new(width, height);
            EditorUtility.CopySerialized(piece.Icon.texture, tex);

            return tex;
        }
    }
}
