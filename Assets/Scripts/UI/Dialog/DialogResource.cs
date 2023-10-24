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
        [field: SerializeField]
        public string Text { get; private set; }

        [field: SerializeField]
        public Texture2D LeftIcon { get; private set; }

        [field: SerializeField]
        public Texture2D RightIcon { get; private set; }

        [field: SerializeField]
        public AudioClip Audio { get; private set; }

        [field: SerializeField]
        public DialogPositionHorizontal PositionHorizontal { get; private set; }

        [field: SerializeField]
        public DialogPositionVertical PositionVertical { get; private set; }
    }

    [CreateAssetMenu(fileName = "Dialog", menuName = "UI/Dialog", order = 10)]
    public class DialogResource : ScriptableObject
    {
        public ReadOnlySpan<DialogFrame> Frames => frames;

        [SerializeField]
        DialogFrame[] frames;
    }
}

