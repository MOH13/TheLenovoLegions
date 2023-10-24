using System;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace LL.UI.Dialog
{
    public class DialogUIBehavior : MonoBehaviour
    {
        [SerializeField]
        UIDocument? document;

        [SerializeField]
        DialogResource? currentDialog;

        Logic? logic;

        void OnEnable()
        {
            if (this.document != null && this.currentDialog != null)
                this.logic = new Logic(currentDialog, document);
        }

        [ContextMenu("Next frame")]
        public bool NextFrame()
        {
            if (this.logic != null)
                return this.logic.NextFrame();
            return false;
        }

        private class Logic
        {
            public const string POSITION_HORIZONTAL_LEFT = "dialog-left";
            public const string POSITION_HORIZONTAL_RIGHT = "dialog-right";
            public const string POSITION_HORIZONTAL_CENTER = "dialog-center";
            public const string POSITION_VERTICAL_TOP = "dialog-top";
            public const string POSITION_VERTICAL_BOTTOM = "dialog-bottom";

            readonly DialogResource currentDialog;

            int frameIndex;

            readonly VisualElement rootElem;

            Label textElem;

            readonly VisualElement leftIcon;

            readonly VisualElement rightIcon;

            public Logic(DialogResource currentDialog, UIDocument document)
            {
                this.currentDialog = currentDialog;
                this.frameIndex = 0;

                rootElem = document.rootVisualElement;
                textElem = rootElem.Q<Label>("dialog-text");
                leftIcon = rootElem.Q<VisualElement>("left-icon");
                rightIcon = rootElem.Q<VisualElement>("right-icon");

                if (currentDialog.Frames.Length > 0)
                    Refresh();
            }

            public bool NextFrame()
            {
                this.frameIndex++;

                if (this.frameIndex < this.currentDialog.Frames.Length)
                {
                    Refresh();
                    return true;
                }
                return false;
            }

            private void Refresh()
            {
                var currentFrame = currentDialog.Frames[frameIndex];

                switch (currentFrame.PositionHorizontal)
                {
                    case DialogPositionHorizontal.Center:
                        rootElem.RemoveFromClassList(POSITION_HORIZONTAL_LEFT);
                        rootElem.RemoveFromClassList(POSITION_HORIZONTAL_RIGHT);
                        rootElem.AddToClassList(POSITION_HORIZONTAL_CENTER);
                        break;
                    case DialogPositionHorizontal.Left:
                        rootElem.AddToClassList(POSITION_HORIZONTAL_LEFT);
                        rootElem.RemoveFromClassList(POSITION_HORIZONTAL_RIGHT);
                        rootElem.RemoveFromClassList(POSITION_HORIZONTAL_CENTER);
                        break;
                    case DialogPositionHorizontal.Right:
                        rootElem.RemoveFromClassList(POSITION_HORIZONTAL_LEFT);
                        rootElem.AddToClassList(POSITION_HORIZONTAL_RIGHT);
                        rootElem.RemoveFromClassList(POSITION_HORIZONTAL_CENTER);
                        break;
                }

                switch (currentFrame.PositionVertical)
                {
                    case DialogPositionVertical.Bottom:
                        rootElem.AddToClassList(POSITION_VERTICAL_BOTTOM);
                        rootElem.RemoveFromClassList(POSITION_VERTICAL_TOP);
                        break;
                    case DialogPositionVertical.Top:
                        rootElem.RemoveFromClassList(POSITION_VERTICAL_BOTTOM);
                        rootElem.AddToClassList(POSITION_VERTICAL_TOP);
                        break;
                }

                textElem.text = currentFrame.Text;
                leftIcon.style.backgroundImage = currentFrame.LeftIcon;
                rightIcon.style.backgroundImage = currentFrame.RightIcon;
            }
        }
    }
}

