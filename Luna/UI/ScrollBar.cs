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
            
            // Create scroll handle and configure click and unclick actions
            scrollHandle = new Button(UITheme.ColorType.ScrollBar);
            scrollHandle.OnClick(() => { dragging = true; });
            scrollHandle.OnUnclick(() => { dragging = false; });
            layout.Padding = new Tetra(2);
            Initialise();
            SetHandlePosition(value);

            switch (axis)
            {
                // Horizontal scrollbar
                case LVector2.HORIZONTAL:
                    {
                        // Don't align child objects horizontally
                        layout.HorizontalAlignment = Alignment.Ignore;
                        // Make scroll handle have a fixed width but full height
                        scrollHandle.SetLayout(new Layout()
                        {
                            LayoutWidth = Sizing.Fixed(100),
                            LayoutHeight = Sizing.Grow(1)
                        });
                        break;
                    }
                // Vertical scrollbar
                case LVector2.VERTICAL:
                    {
                        // Don't align child objecs vertically
                        layout.VerticalAlignment = Alignment.Ignore;
                        // Make scroll handle have a fixed height but full width
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

                // Change internal value and notify listeners that value has been changed
                HardSetValue(CalculateValueFromPosition());
            }
        }

        /// <summary>
        /// Move the handle to the correct position based on scroll value
        /// </summary>
        /// <param name="value">The current scroll value of this component (from 0 to 1)</param>
        private void SetHandlePosition(float value)
        {
            float handleSize = scrollHandle.GetTransform().Size.GetComponent(axis);
            float scrollBounds = GetTransform().Size.GetComponent(axis) - layout.Padding.GetAxis(axis) - handleSize;
            float axisStart = layout.Padding.AxisStart(axis);
            
            scrollHandle.GetTransform().SetPositionComponentValue(value * scrollBounds + axisStart, axis);
        }

        /// <summary>
        /// Changes the handle's target position based on mouse movement
        /// </summary>
        private void MoveHandle()
        {
            // Get current handle target position
            float handlePosition = scrollHandle.GetTransform().TargetPosition.GetComponent(axis);
            // Get important handle info to prevent over-scrolling
            float handleSize = scrollHandle.GetTransform().Size.GetComponent(axis);
            float scrollBounds = GetTransform().Size.GetComponent(axis) - layout.Padding.GetAxis(axis) - handleSize;
            float axisStart = layout.Padding.AxisStart(axis);

            // Change handle target position, while preventing over-scrolling
            float newPosition = Math.Clamp(handlePosition + new LVector2(MouseHandler.DeltaPosition).GetComponent(axis), axisStart, axisStart + scrollBounds);
            scrollHandle.GetTransform().SetPositionComponentValue(newPosition, axis);
        }

        /// <summary>
        /// Calculate a 0 to 1 value of this scrollbar based on where it is currently
        /// </summary>
        /// <returns></returns>
        private float CalculateValueFromPosition()
        {
            // Get info for value calculation
            float handleSize = scrollHandle.GetTransform().Size.GetComponent(axis);
            // Size of the scrollable area
            float scrollBounds = GetTransform().Size.GetComponent(axis) - layout.Padding.GetAxis(axis) - handleSize;
            // Where the scrollable region begins
            float axisStart = layout.Padding.AxisStart(axis);
            float handlePosition = scrollHandle.GetTransform().TargetPosition.GetComponent(axis);

            return (handlePosition - axisStart) / scrollBounds;
        }

        public float GetValue()
        {
            return value;
        }

        /// <summary>
        /// Set value without updating listeners
        /// </summary>
        public void SoftSetValue(float value)
        {
            this.value = value;
            SetHandlePosition(value);
        }

        /// <summary>
        /// Set value and update listeners
        /// </summary>
        /// <param name="value"></param>
        public void HardSetValue(float value)
        {
            SoftSetValue(value);
            onValueChanged?.Invoke(value);
        }

        /// <summary>
        /// Called when the value is changed
        /// </summary>
        /// <param name="e"></param>
        public void OnValueChanged(Action<float> e)
        {
            onValueChanged += e;
        }

        /// <summary>
        /// Change the handle size to the given ratio of the scrollable area
        /// </summary>
        /// <param name="ratio">The ratio of scrollable area to set the size to</param>
        public void SetHandleSizeRatio(float ratio)
        {
            float scrollBounds = GetTransform().Size.GetComponent(axis) - layout.Padding.GetAxis(axis);

            switch (axis)
            {
                // Horizontal scrollbar
                case LVector2.HORIZONTAL:
                    {
                        scrollHandle.SetLayout(new Layout() { LayoutWidth = Sizing.Fixed((int)(scrollBounds * Math.Clamp(ratio, 0, 1))) });
                        break;
                    }
                // Vertical scrollbar
                case LVector2.VERTICAL:
                    {
                        scrollHandle.SetLayout(new Layout() { LayoutHeight = Sizing.Fixed((int)(scrollBounds * Math.Clamp(ratio, 0, 1))) });
                        break;
                    }
            }
        }
    }
}