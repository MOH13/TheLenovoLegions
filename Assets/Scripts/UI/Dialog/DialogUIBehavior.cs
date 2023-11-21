using System;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace LL.UI.Dialog
{
    public class DialogUIBehavior : MonoBehaviour
    {
        public event Action? OnNextFrame;
        public event Action? OnFinish;

        [SerializeField]
        UIDocument? document;

        [SerializeField]
        DialogResource? currentDialog;

        [SerializeField]
        AudioSource? audioSource;

        Logic? logic;

        void OnEnable()
        {
            SetDialog(currentDialog);
        }

        [ContextMenu("Next frame")]
        public bool NextFrame()
        {
            if (this.logic != null)
                return this.logic.NextFrame();
            return false;
        }

        public void SetDialog(DialogResource? dialog)
        {
            this.currentDialog = dialog;
            if (document != null)
            {
                document.enabled = currentDialog != null;
                if (currentDialog != null)
                {
                    this.logic = new Logic(currentDialog, document, audioSource);
                    this.logic.OnNextFrame += LogicOnNextFrame;
                    this.logic.OnFinish += LogicOnFinish;
                }
            }
        }

        private void LogicOnNextFrame()
        {
            OnNextFrame?.Invoke();
        }

        private void LogicOnFinish()
        {
            SetDialog(null);
            OnFinish?.Invoke();
        }

        private class Logic
        {
            public event Action? OnNextFrame;
            public event Action? OnFinish;

            public const string POSITION_HORIZONTAL_LEFT = "dialog-left";
            public const string POSITION_HORIZONTAL_RIGHT = "dialog-right";
            public const string POSITION_HORIZONTAL_CENTER = "dialog-center";
            public const string POSITION_VERTICAL_TOP = "dialog-top";
            public const string POSITION_VERTICAL_BOTTOM = "dialog-bottom";

            readonly DialogResource currentDialog;

            readonly AudioSource? audioSource;

            int frameIndex;

            readonly VisualElement rootElem;

            Label textElem;

            readonly VisualElement leftIcon;

            readonly VisualElement rightIcon;

            public Logic(DialogResource currentDialog, UIDocument document, AudioSource audioSource)
            {
                this.currentDialog = currentDialog;
                this.audioSource = audioSource;
                this.frameIndex = 0;

                rootElem = document.rootVisualElement;
                textElem = rootElem.Q<Label>("dialog-text");
                leftIcon = rootElem.Q<VisualElement>("left-icon");
                rightIcon = rootElem.Q<VisualElement>("right-icon");

                rootElem.RegisterCallback<ClickEvent>(OnClick);

                if (currentDialog.Frames.Length > 0)
                {
                    PlaySound();
                    Refresh();
                }
            }

            private void OnClick(ClickEvent evt)
            {
                NextFrame();
            }

            public bool NextFrame()
            {
                this.frameIndex++;

                if (this.frameIndex < this.currentDialog.Frames.Length)
                {
                    PlaySound();
                    Refresh();

                    OnNextFrame?.Invoke();

                    return true;
                }

                OnFinish?.Invoke();

                return false;
            }

            private void PlaySound()
            {
                var sound = this.currentDialog.Frames[this.frameIndex].Audio;

                if (audioSource != null && sound != null)
                {
                    audioSource.clip = sound;
                    audioSource.Play();
                }
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

