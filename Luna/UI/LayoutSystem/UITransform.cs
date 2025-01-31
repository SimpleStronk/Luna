using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private LVector2 position = new LVector2();
        private LVector2 size = new LVector2();
        private Action onResize;

        #region getters_setters
        public Rectangle GetLocalRect()
        {
            return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public Rectangle GetGlobalRect()
        {
            return new Rectangle((int)(position.X + GetParentPosition().X), (int)(position.Y + GetParentPosition().Y), (int)size.X, (int)size.Y);
        }

        public LVector2 GetLocalCentre()
        {
            return position + (size / 2);
        }

        public LVector2 GetGlobalCentre()
        {
            return position + GetParentPosition() + (size / 2);
        }

        private LVector2 GetParentPosition()
        {
            if (parent == null) return LVector2.Zero;

            return parent.GlobalPosition;
        }

        public UITransform Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public LVector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public LVector2 GlobalPosition
        {
            get { return position + GetParentPosition(); }
            set { position = value - GetParentPosition(); }
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
