using System;
using LL.Game.Story;
using LL.UI.Dialog;
using UnityEngine;

namespace LL.Game.Equipment
{
    public class EquipmentPickup : MonoBehaviour
    {
        [SerializeField]
        DialogTrigger dialogTrigger;

        [SerializeField]
        EquipmentResource equipment;

        [SerializeField]
        AudioClip? activationAudio;
        [SerializeField]
        SpriteRenderer? spriteRenderer;

        void Start()
        {
            dialogTrigger.SetDialog(CreateDialog());
            UpdateSpriteRenderer();
        }

        void OnValidate()
        {
            UpdateSpriteRenderer();
        }

        void UpdateSpriteRenderer()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = equipment.Icon;
            }
        }

        private DialogResource CreateDialog()
        {
            var frames = new DialogFrame[1] {
                new() {
                    Text = $"You found an item: <b>{equipment.EquipmentName}</b>!\n{equipment.PickupText}",
                    LeftIcon = equipment.Icon,
                    PositionHorizontal = DialogPositionHorizontal.Left,
                    PositionVertical = DialogPositionVertical.Bottom,
                    Audio = activationAudio,
                }
            };
            return DialogResource.Create(frames);
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if ((col.gameObject.layer | dialogTrigger.LayerMask) != 0)
            {
                col.gameObject.GetComponent<InventoryBehavior>().AddToInventory(equipment);
                gameObject.SetActive(false);
            }
        }
    }
}

