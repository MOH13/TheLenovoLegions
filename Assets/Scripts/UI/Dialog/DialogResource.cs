using System;
using System.Collections.Generic;
using UnityEngine;

namespace LL.UI.Dialog
{
    public enum DialogPositionHorizontal
    {
        Center,
        Left,
        Right,
    }

    public enum DialogPositionVertical
    {
        Bottom,
        Top,
    }

    [Serializable]
    public struct DialogFrame
    {
        [field: SerializeField, TextArea]
        public string Text { get; set; }

        [field: SerializeField]
        public Sprite? LeftIcon { get; set; }

        [field: SerializeField]
        public Sprite? RightIcon { get; set; }

        [field: SerializeField]
        public AudioClip? Audio { get; set; }

        [field: SerializeField]
        public DialogPositionHorizontal PositionHorizontal { get; set; }

        [field: SerializeField]
        public DialogPositionVertical PositionVertical { get; set; }
    }

    [CreateAssetMenu(fileName = "Dialog", menuName = "UI/Dialog", order = 10)]
    public class DialogResource : ScriptableObject
    {
        public ReadOnlySpan<DialogFrame> Frames => frames;

        [SerializeField]
        DialogFrame[] frames;

        public static DialogResource Create(DialogFrame[] frames)
        {
            var dialog = CreateInstance<DialogResource>();
            dialog.frames = frames;
            return dialog;
        }
    }
}

