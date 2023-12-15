using System;
using System.Collections.Generic;
using LL.Game.Equipment;
using LL.Game.Stats;
using UnityEngine;
using UnityEngine.UIElements;


namespace LL.UI
{
    public class InventoryUIBehavior : MonoBehaviour
    {
        [SerializeField]
        UIDocument document;

        [SerializeField]
        InventoryBehavior inventory;

        [SerializeField]
        LiveStatsBehavior stats;

        Logic? logic;

        private class Logic
        {
            readonly InventoryBehavior inventory;

            readonly LiveStatsBehavior stats;

            readonly VisualElement equipmentRoot;
            readonly List<SlotVisualElement> slotElements;

            readonly VisualElement inventoryRoot;
            readonly List<EquipmentVisualElement> inventoryElements;

            EquipmentVisualElement? selectedInventoryElement;

            readonly List<Label> statValueLabels;
            readonly List<float> previousStatValues;

            public Logic(InventoryBehavior inventory, LiveStatsBehavior stats, UIDocument document, Action closeAction)
            {
                this.inventory = inventory;
                this.stats = stats;

                // Init slot elements
                equipmentRoot = document.rootVisualElement.Q("equipment-slots");
                equipmentRoot.Clear();
                slotElements = new List<SlotVisualElement>();

                // Init inventory elements
                inventoryRoot = document.rootVisualElement.Q("inventory-items");
                inventoryRoot.Clear();
                inventoryElements = new List<EquipmentVisualElement>();

                var closeButton = document.rootVisualElement.Q<Button>("close-button");
                closeButton.clicked += closeAction;
                document.rootVisualElement.RegisterCallback<NavigationCancelEvent>((_) => closeAction());

                statValueLabels = new();
                previousStatValues = new();
                var statsNamesRoot = document.rootVisualElement.Q("stats-names");
                statsNamesRoot.Clear();
                var statsValuesRoot = document.rootVisualElement.Q("stats-values");
                statsValuesRoot.Clear();
                foreach (var statEntry in stats.Container.GetStats())
                {
                    statsNamesRoot.Add(new Label(statEntry.Stat.StatName));

                    var valueLabel = new Label();
                    statValueLabels.Add(valueLabel);
                    statsValuesRoot.Add(valueLabel);
                    previousStatValues.Add(-1);
                }
                foreach (var view in document.rootVisualElement.Query<ScrollView>().Build())
                {
                    var thisView = view;
                    thisView.RegisterCallback<FocusInEvent>((evt) => { try { thisView.ScrollTo(evt.target as VisualElement); } catch (Exception) { } });
                }
            }

            public void Refresh()
            {
                RefreshSlots();

                RefreshInventory();

                RefreshStats();
            }

            void RefreshSlots()
            {
                var slots = inventory.Container.GetSlots();
                for (int i = 0; i < slots.Length; i++)
                {
                    var slot = slots[i];
                    var equipment = inventory.GetAtIndex(i);
                    SetSlot(i, slot.Slot, equipment);
                }
            }

            void RefreshInventory()
            {
                int i = 0;
                foreach (var item in inventory.GetView())
                {
                    SetInventorySlot(i, item);
                    i++;
                }
                for (; i < 4; i++)
                {
                    SetInventorySlot(i, null);
                }
            }

            void RefreshStats()
            {
                var entries = stats.Container.GetStats();
                for (int i = 0; i < entries.Length; i++)
                {
                    var statEntry = entries[i];
                    var val = stats.GetValue(statEntry.Stat);
                    if (!Mathf.Approximately(val, previousStatValues[i]))
                    {
                        var text = $"{val:0.##}";

                        var diff = val - statEntry.BaseValue;

                        if (!Mathf.Approximately(diff, 0))
                        {
                            if (diff > 0)
                                text += $" (+{diff:0.##})";
                            else
                                text += $" ({diff:0.##})";
                        }
                        statValueLabels[i].text = text;
                        previousStatValues[i] = val;
                    }
                }
            }

            void OnSlotSubmit(NavigationSubmitEvent evt, int i)
            {
                HandleClickOnSlot(i);
            }

            void OnSlotClick(MouseDownEvent evt, int i)
            {
                HandleClickOnSlot(i);
            }

            void HandleClickOnSlot(int i)
            {
                var element = slotElements[i];
                if (selectedInventoryElement == null)
                {
                    inventory.UnequipAtIndex(i);
                    return;
                };
                if (element.Slot == selectedInventoryElement.Equipment.Slot)
                {
                    inventory.EquipAtIndex(selectedInventoryElement.Equipment, i);
                    SetSelectedInventoryElement(null);
                }
            }

            void OnInventorySubmit(NavigationSubmitEvent evt, EquipmentVisualElement element)
            {
                if (element.Equipment == null) return;

                SetSelectedInventoryElement(element);
            }

            void OnInventoryMouseDown(MouseDownEvent evt, EquipmentVisualElement element)
            {
                if (element.Equipment == null) return;

                if (evt.button == 0) // left
                    SetSelectedInventoryElement(element);
                if (evt.button == 1) // right
                {
                    SetSelectedInventoryElement(null);
                    inventory.Equip(element.Equipment);
                }
            }

            void SetSelectedInventoryElement(EquipmentVisualElement? element)
            {
                foreach (var e in inventoryElements)
                {
                    e.SetHighlight(false);
                }
                foreach (var e in slotElements)
                {
                    e.SetHighlight(false);
                }
                if (element == selectedInventoryElement)
                {
                    selectedInventoryElement = null;
                }
                else
                {
                    selectedInventoryElement = element;
                    if (element != null)
                    {
                        element.SetHighlight(true);
                        foreach (var e in slotElements)
                        {
                            e.SetHighlight(e.Slot == element.Equipment.Slot);
                        }
                    }
                }
            }

            void SetSlot(int index, EquipmentSlotResource slot, EquipmentResource? equipment)
            {
                for (int i = slotElements.Count; i < index + 1; i++)
                {
                    var slotElement = new SlotVisualElement();
                    int elementIndex = i;
                    slotElement.RegisterCallback<NavigationSubmitEvent, int>(OnSlotSubmit, elementIndex);
                    slotElement.RegisterCallback<MouseDownEvent, int>(OnSlotClick, elementIndex);
                    equipmentRoot.Add(slotElement);
                    slotElements.Add(slotElement);
                }
                var element = slotElements[index];
                element.SetSlot(slot);
                element.SetEquipment(equipment);
            }

            void SetInventorySlot(int index, EquipmentResource? equipment)
            {
                for (int i = inventoryElements.Count; i < index + 1; i++)
                {
                    var equipmentElement = new EquipmentVisualElement();
                    equipmentElement.RegisterCallback<NavigationSubmitEvent, EquipmentVisualElement>(OnInventorySubmit, equipmentElement);
                    equipmentElement.RegisterCallback<MouseDownEvent, EquipmentVisualElement>(OnInventoryMouseDown, equipmentElement);
                    inventoryRoot.Add(equipmentElement);
                    inventoryElements.Add(equipmentElement);
                }
                var element = inventoryElements[index];
                element.SetEquipment(equipment);
            }
        }


        void OnEnable()
        {
            logic = new(inventory, stats, document, () => gameObject.SetActive(false));
        }

        void LateUpdate()
        {
            Refresh();
        }

        [ContextMenu("Refresh inventory")]
        public void Refresh()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Cannot refresh inventory UI while not playing");
                return;
            }

            logic?.Refresh();
        }
    }
}
