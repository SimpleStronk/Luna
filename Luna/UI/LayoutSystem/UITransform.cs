using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Luna.UI.LayoutSystem
{
    internal class UITransform
    {
        private UITransform? parent;
        private IPositionAnimator positionAnimator = new ExpPositionAnimator();
        private LVector2 size = new LVector2();
        private Action onResize;

        public void Update()
        {
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
        #endregion

        public void OnResize(Action e)
        {
            onResize += e;
        }
    }
}
