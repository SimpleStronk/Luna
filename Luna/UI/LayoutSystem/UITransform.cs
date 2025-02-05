using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Luna.UI.LayoutSystem.LUIVA;

namespace Luna.UI.LayoutSystem
{
    internal class UITransform
    {
        private UITransform? parent;
        private IPositionAnimator positionAnimator = new ExpPositionAnimator();
        private LVector2 size = new LVector2();
        private Action onResize;
        private int overflowAxis = -1;
        private LVector2 overflowAmount = new LVector2();
        private LVector2 scrollOffset = new LVector2();
        private Tetra padding;
        private float scrollAmount;
        private float scrollMax;
        private float scrollSensitivity = 0.4f;
        private bool scrollable = false;
        private Action<float> onScrollChanged;

        public void Update()
        {
            if (scrollable)
            {
                CalculateOverflow();
                MoveDisplay();
            }

            positionAnimator.Update();
        }

        #region getters_setters
        public Rectangle GetLocalRect()
        {
            return new Rectangle((int)positionAnimator.GetCurrentPosition().X, (int)positionAnimator.GetCurrentPosition().Y, (int)size.X, (int)size.Y);
        }

        public Rectangle GetGlobalRect()
        {
            return new Rectangle((int)(positionAnimator.GetCurrentPosition().X + GetParentPosition().X), (int)(positionAnimator.GetCurrentPosition().Y + GetParentPosition().Y), (int)size.X, (int)size.Y);
        }

        public LVector2 GetLocalCentre()
        {
            return positionAnimator.GetCurrentPosition() + (size / 2);
        }

        public LVector2 GetGlobalCentre()
        {
            return positionAnimator.GetCurrentPosition() + GetParentPosition() + (size / 2);
        }

        private LVector2 GetParentPosition()
        {
            if (parent == null) return LVector2.Zero;

            return parent.GlobalPosition;
        }

        public void SetPositionComponentValue(float component, int axis)
        {
            LVector2 targetPos = positionAnimator.GetTargetPosition();
            targetPos.SetComponentValue(component, axis);
            positionAnimator.SetPosition(targetPos);
        }

        public void SetGlobalPositionComponentValue(float component, int axis)
        {
            LVector2 targetPos = positionAnimator.GetTargetPosition();
            targetPos.SetComponentValue(component - GetParentPosition().GetComponent(axis), axis);
            positionAnimator.SetPosition(targetPos);
        }

        public UITransform Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public LVector2 CurrentPosition
        {
            get { return positionAnimator.GetCurrentPosition(); }
            set { positionAnimator.SetPosition(value); }
        }

        public LVector2 TargetPosition
        {
            get { return positionAnimator.GetTargetPosition(); }
            set { positionAnimator.SetPosition(value); }
        }

        public LVector2 GlobalPosition
        {
            get { return positionAnimator.GetCurrentPosition() + GetParentPosition(); }
            set { positionAnimator.SetPosition(value - GetParentPosition()); }
        }

        public LVector2 Size
        {
            get { return size; }
            set { size = value; onResize?.Invoke(); }
        }

        public int MinDimension
        {
            get { return (int)Math.Min(size.X, size.Y); }
        }

        public int MaxDimension
        {
            get { return (int)Math.Max(size.X, size.Y); }
        }

        private int OverflowAxis
        {
            get { return overflowAxis; }
            set { if (overflowAxis != value) { overflowAxis = value; scrollAmount = 0; } }
        }

        public bool Scrollable
        {
            get { return scrollable; }
            set { scrollable = value; }
        }

        public LVector2 ScrollOffset
        {
            get { return scrollOffset; }
        }

        public Tetra Padding
        {
            set { padding = value; }
        }

        public void OnScrollChanged(Action<float> e)
        {
            onScrollChanged += e;
        }
        #endregion

        public void OnResize(Action e)
        {
            onResize += e;
        }

        public void Scroll(int scroll)
        {
            if (scroll == 0) return;

            scrollAmount += scroll;
            ClampScroll();
            onScrollChanged?.Invoke(-scrollAmount / scrollMax);
        }

        public void SetScrollRatio(float normalisedValue)
        {
            scrollAmount = -normalisedValue * scrollMax;
        }

        private void MoveDisplay()
        {
            scrollOffset.SetComponentValue(scrollAmount * scrollSensitivity, overflowAxis);
        }

        private void ClampScroll()
        {
            scrollAmount = Math.Clamp(scrollAmount, -scrollMax, 0);
        }

        private void CalculateOverflow()
        {
            if (parent == null)
            {
                OverflowAxis = -1;
                return;
            }

            overflowAmount = new LVector2(size.X - (parent.size.X - (parent.padding.Left + parent.padding.Right)), size.Y - (parent.size.Y - (parent.padding.Top + parent.padding.Bottom)));

            //  If Y overflow amount is positive, regard axis as vertical regardless of horizontal state
            if (overflowAmount.Y >= 0) OverflowAxis = LVector2.VERTICAL;
            else if (overflowAmount.X >= 0) OverflowAxis = LVector2.HORIZONTAL;
            else OverflowAxis = -1;

            scrollMax = overflowAmount.GetComponent(OverflowAxis) / scrollSensitivity;
        }

        public bool IsOverflowing
        {
            get
            {
                return overflowAmount.GetComponent(overflowAxis) > 0;
            }
        }

        public float GetOverflowRatio()
        {
            return Size.GetComponent(overflowAxis) / (parent.Size.GetComponent(overflowAxis) - parent.padding.GetAxis(overflowAxis));
        }
    }
}
