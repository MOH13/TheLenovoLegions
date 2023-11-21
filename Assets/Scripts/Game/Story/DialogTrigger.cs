using Cinemachine;
using LL.UI.Dialog;
using UnityEngine;

namespace LL.Game.Story
{
    public class DialogTrigger : MonoBehaviour
    {
        [SerializeField]
        DialogUIBehavior dialogUI;

        [SerializeField]
        CinemachineVirtualCamera cam;

        [SerializeField]
        LayerMask layerMask;

        [SerializeField]
        DialogResource dialog;

        [SerializeField]
        bool canShow = true;

        [SerializeField]
        bool onlyShowOnce = true;

        public LayerMask LayerMask => layerMask;

        public void SetDialog(DialogResource dialog)
        {
            this.dialog = dialog;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (canShow && ((1 << col.gameObject.layer) & layerMask) != 0)
            {
                dialogUI.SetDialog(dialog);
                if (cam != null)
                {
                    cam.enabled = true;
                    dialogUI.OnFinish += OnFinish;
                }
                if (onlyShowOnce)
                {
                    canShow = false;
                }
            }
        }

        private void OnFinish()
        {
            cam.enabled = false;
            dialogUI.OnFinish -= OnFinish;
        }
    }
}