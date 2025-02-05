using System;
using Luna.ManagerClasses;
using Luna.UI.LayoutSystem;
using static Luna.UI.LayoutSystem.LUIVA;

namespace Luna.UI
{
    internal class ScrollBar : Button, ISlidable
    {
        private Button scrollHandle;
        private Action<float> onValueChanged;
        private int axis;
        private float value;
        private bool dragging = false;

        public ScrollBar(int axis, UITheme.ColorType colorType) : base(VisualResponse.ColourChange, colorType)
        {
            this.axis = axis;
            scrollHandle = new Button(UITheme.ColorType.ScrollBar);
            scrollHandle.OnClick(() => { dragging = true; });
            scrollHandle.OnUnclick(() => { dragging = false; });
            layout.Padding = new Tetra(2);
            Initialise();
            SetHandlePosition(value);
            
            switch (axis)
            {
                case LVector2.HORIZONTAL:
                {
                    layout.HorizontalAlignment = Alignment.Ignore;
                    scrollHandle.SetLayout(new Layout()
                    {
                        LayoutWidth = Sizing.Fixed(100),
                        LayoutHeight = Sizing.Grow(1)
                    });
                    break;
                }
                case LVector2.VERTICAL:
                {
                    layout.VerticalAlignment = Alignment.Ignore;
                    scrollHandle.SetLayout(new Layout()
                    {
                        LayoutWidth = Sizing.Grow(1),
                        LayoutHeight = Sizing.Fixed(100)
                    });
                    break;
                }
            }

            AddChild(scrollHandle);
        }

        protected override void Update()
        {
            base.Update();

            if (dragging)
            {
                MoveHandle();
                HardSetValue(CalculateValueFromPosition());
            }
        }

        private void SetHandlePosition(float value)
        {
            float handleSize = scrollHandle.GetTransform().Size.GetComponent(axis);
            float scrollBounds = GetTransform().Size.GetComponent(axis) - layout.Padding.GetAxis(axis) - handleSize;
            float axisStart = layout.Padding.AxisStart(axis);
            
            scrollHandle.GetTransform().SetPositionComponentValue(value * scrollBounds + axisStart, axis);
        }

        private void MoveHandle()
        {
            float handlePosition = scrollHandle.GetTransform().TargetPosition.GetComponent(axis);
            float handleSize = scrollHandle.GetTransform().Size.GetComponent(axis);
            float scrollBounds = GetTransform().Size.GetComponent(axis) - layout.Padding.GetAxis(axis) - handleSize;
            float axisStart = layout.Padding.AxisStart(axis);

            float newPosition = Math.Clamp(handlePosition + new LVector2(MouseHandler.DeltaPosition).GetComponent(axis), axisStart, axisStart + scrollBounds);
            scrollHandle.GetTransform().SetPositionComponentValue(newPosition, axis);
        }

        private float CalculateValueFromPosition()
        {
            float handleSize = scrollHandle.GetTransform().Size.GetComponent(axis);
            float scrollBounds = GetTransform().Size.GetComponent(axis) - layout.Padding.GetAxis(axis) - handleSize;
            float axisStart = layout.Padding.AxisStart(axis);
            float handlePosition = scrollHandle.GetTransform().TargetPosition.GetComponent(axis);
            return (handlePosition - axisStart) / scrollBounds;
        }

        public float GetValue()
        {
            return value;
        }

        public void SoftSetValue(float value)
        {
            this.value = value;
            SetHandlePosition(value);
        }

        public void HardSetValue(float value)
        {
            SoftSetValue(value);
            onValueChanged?.Invoke(value);
        }

        public void OnValueChanged(Action<float> e)
        {
            onValueChanged += e;
        }

        public void SetHandleSizeRatio(float ratio)
        {
            float scrollBounds = GetTransform().Size.GetComponent(axis) - layout.Padding.GetAxis(axis);
            
            switch (axis)
            {
                case LVector2.HORIZONTAL:
                {
                    scrollHandle.SetLayout(new Layout() { LayoutWidth = Sizing.Fixed((int)(scrollBounds * Math.Clamp(ratio, 0, 1))) });
                    break;
                }
                case LVector2.VERTICAL:
                {
                    scrollHandle.SetLayout(new Layout() { LayoutHeight = Sizing.Fixed((int)(scrollBounds * Math.Clamp(ratio, 0, 1))) });
                    break;
                }
            }
        }
    }
}