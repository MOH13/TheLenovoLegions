using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LL.UI
{
    public class ShowTooltipEvent : EventBase<ShowTooltipEvent>
    {
        public string Tooltip { get; private set; }
        public int PointerId { get; private set; }
        public Vector2 Position { get; private set; }

        public ShowTooltipEvent()
        {
            Init();
            Tooltip = "";
        }

        protected override void Init()
        {
            base.Init();
            bubbles = true;
            tricklesDown = false;
        }

        public static ShowTooltipEvent GetPooled(IEventHandler target, string tooltip, int pointerId, Vector2 position)
        {
            var evt = GetPooled();
            evt.target = target;
            evt.Tooltip = tooltip;
            evt.PointerId = pointerId;
            evt.Position = position;
            return evt;
        }
    }

    public class HideTooltipEvent : EventBase<HideTooltipEvent>
    {
        public HideTooltipEvent()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();
            bubbles = true;
            tricklesDown = false;
        }

        public static HideTooltipEvent GetPooled(IEventHandler target)
        {
            var evt = GetPooled();
            evt.target = target;
            return evt;
        }
    }

    public class TooltipShowerElement : VisualElement
    {
        public const string TOOLTIP_CLASS = "tooltip";

        VisualElement contentRoot, tooltipsRoot;

        Dictionary<IEventHandler, (VisualElement, int)> activeTooltips;

        public TooltipShowerElement()
        {
            activeTooltips = new();
            contentRoot = new();
            contentRoot.style.flexGrow = 1;
            tooltipsRoot = new()
            {
                pickingMode = PickingMode.Ignore,
                usageHints = UsageHints.DynamicTransform,
            };
            tooltipsRoot.style.position = Position.Absolute;
            this.hierarchy.Add(contentRoot);
            this.hierarchy.Add(tooltipsRoot);

            this.RegisterCallback<ShowTooltipEvent>(OnShowTooltipEvent);
            this.RegisterCallback<HideTooltipEvent>(OnHideTooltipEvent);
            this.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }

        public override VisualElement contentContainer => this.contentRoot;

        private VisualElement CreateNewTooltip(ShowTooltipEvent evt)
        {
            var label = new Label(evt.Tooltip)
            {
                pickingMode = PickingMode.Ignore
            };
            label.AddToClassList(TOOLTIP_CLASS);
            return label;
        }

        private void PositionElement(VisualElement elem, Vector2 position)
        {
            var pos = this.WorldToLocal(RuntimePanelUtils.ScreenToPanel(this.panel, position));
            elem.style.left = pos.x;
            elem.style.top = pos.y;
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            PositionElement(tooltipsRoot, evt.position);
        }

        private void OnShowTooltipEvent(ShowTooltipEvent evt)
        {
            if (!activeTooltips.ContainsKey(evt.target))
            {
                var tooltip = CreateNewTooltip(evt);

                activeTooltips.Add(evt.target, (tooltip, evt.PointerId));
                tooltipsRoot.Add(tooltip);
                evt.StopPropagation();
            }
        }

        private void OnHideTooltipEvent(HideTooltipEvent evt)
        {
            if (activeTooltips.TryGetValue(evt.target, out var val))
            {
                var (tooltip, pointerId) = val;

                tooltipsRoot.Remove(tooltip);
                activeTooltips.Remove(evt.target);
                evt.StopPropagation();
            }

        }

        public new class UxmlFactory : UxmlFactory<TooltipShowerElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
            }
        }
    }
}

