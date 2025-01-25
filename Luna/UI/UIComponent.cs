using Luna.ManagerClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Luna.UI.LayoutSystem;

namespace Luna.UI
{
    internal abstract class UIComponent : ILayoutable
    {
        protected static Texture2D pixelTexture;
        protected UITheme theme, cascadeTheme;
        protected LUIVA.Layout layout = new LUIVA.Layout();
        protected UITransform transform = new UITransform();
        protected UIComponent? parent;
        protected List<UIComponent> children = new List<UIComponent>();
        private List<UIComponent> childQueue = new List<UIComponent>();
        protected int priority = -1;
        protected bool focused = false, hovered = false;
        protected bool focusIgnore = false;
        protected Action onHover, onUnhover;
        protected Action<Action, Action, int> checkFocusCallback;
        private static Action<Rectangle> updateScissorRectangle;
        //REPLACE WITH COLOUR ANIMATOR
        protected Color colour;
        protected bool debugMode = false;
        private bool renderDefaultRect = true;

        public enum FocusDependence { Dependent, Always };
        protected bool visible = true;
        protected bool ignoreScissorRect = false;
        protected int elementId;

        private static int currentElement = 0;

        public void Initialise()
        {
            elementId = NewElementId();
            SetDebugActions();
            transform.OnResize(OnResize);
        }

        public void PreUpdate()
        {
            CheckFocused(TestMouseCollision());

            foreach (UIComponent c in children)
            {
                c.PreUpdate();
            }
        }


        public void BaseUpdate()
        {
            SyncChildren();

            if (!visible) return;

            foreach (UIComponent c in children)
            {
                c.BaseUpdate();
            }

            SetHovered(TestMouseCollision() && focused);
            Update();
        }

        public void BaseDraw(SpriteBatch s)
        {
            if (!visible) return;

            updateScissorRectangle(ignoreScissorRect ? GetRootRectangle() : CalculateScissorRectangle());

            if (renderDefaultRect)
            {
                if (pixelTexture == null) throw new Exception("pixelTexture not initialised in class UIComponent");

                if (theme.CornerRadius == (0, 0, 0, 0))
                {
                    s.Draw(pixelTexture, transform.GetGlobalRect(), new Rectangle(0, 0, 1, 1),
                        colour, 0, new Vector2(0, 0), SpriteEffects.None, 1);
                }
                else
                {
                    if (theme.CornerRadius.TopLeft > 0) s.Draw(theme.TopLeftTexture, UITheme.TopLeftRect(theme, transform), new Rectangle(0, 0, theme.CornerRadius.TopLeft, theme.CornerRadius.TopLeft), colour, 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    if (theme.CornerRadius.TopRight > 0) s.Draw(theme.TopRightTexture, UITheme.TopRightRect(theme, transform), new Rectangle(theme.CornerRadius.TopRight, 0, theme.CornerRadius.TopRight, theme.CornerRadius.TopRight), colour, 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    if (theme.CornerRadius.BottomLeft > 0) s.Draw(theme.BottomLeftTexture, UITheme.BottomLeftRect(theme, transform), new Rectangle(0, theme.CornerRadius.BottomLeft, theme.CornerRadius.BottomLeft, theme.CornerRadius.BottomLeft), colour, 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    if (theme.CornerRadius.BottomRight > 0) s.Draw(theme.BottomRightTexture, UITheme.BottomRightRect(theme, transform), new Rectangle(theme.CornerRadius.BottomRight, theme.CornerRadius.BottomRight, theme.CornerRadius.BottomRight, theme.CornerRadius.BottomRight), colour, 0, new Vector2(0, 0), SpriteEffects.None, 1);

                    foreach (Rectangle r in UITheme.FillRectangles(theme, transform))
                    {
                        s.Draw(pixelTexture, r, new Rectangle(0, 0, 1, 1), colour, 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    }
                }
            }
            
            Draw(s);

            foreach (UIComponent c in children)
            {
                c.BaseDraw(s);
            }

            if (parent != null) updateScissorRectangle(parent.CalculateScissorRectangle());
        }

        protected abstract void Update();

        protected abstract void Draw(SpriteBatch s);

        private bool TestMouseCollision()
        {
            return transform.GetGlobalRect().Contains(MouseHandler.Position);
        }

        public int SetPriority(int p)
        {
            priority = p;
            int _priority = ++p;

            foreach (UIComponent c in children)
            {
                _priority = c.SetPriority(_priority++);
            }

            return _priority;
        }

        Rectangle CalculateScissorRectangle()
        {
            if (!layout.ClipChildren) return parent.CalculateScissorRectangle();

            return parent == null ? transform.GetGlobalRect() :
                Rectangle.Intersect(transform.GetGlobalRect(), parent.CalculateScissorRectangle());
        }

        Rectangle GetRootRectangle()
        {
            if (parent == null) return transform.GetGlobalRect();
            else return parent.transform.GetGlobalRect();
        }

        #region debug_system
        public bool DebugMode
        {
            get
            {
                return debugMode;
            }
            set
            {
                debugMode = value;
            }
        }
        private void SetDebugActions()
        {
            onHover += () => ConditionalDebugAction("Hover");
            onUnhover += () => ConditionalDebugAction("Unhover");
        }

        protected void ConditionalDebugAction(string action)
        {
            if (!debugMode) return;

            DebugAction(action);
        }

        protected virtual void DebugAction(string action)
        {
            Console.WriteLine($"UIComponent: {action}");
        }
        #endregion

        #region focus_system
        //Calls back to outside class to report that it's being moused
        private void CheckFocused(bool mouseOver)
        {
            if (!mouseOver || focusIgnore) return;

            //Passes the outside class a function to call if this has priority
            checkFocusCallback(() => SetFocused(true), () => SetFocused(false), priority);
        }

        protected virtual void SetFocused(bool focused)
        {
            this.focused = focused;
        }

        public void SetCheckFocusCallback(Action<Action, Action, int> e)
        {
            checkFocusCallback = e;

            foreach (UIComponent c in children)
            {
                c.SetCheckFocusCallback(e);
            }
        }

        public bool FocusIgnore
        {
            get { return focusIgnore; }
            set { focusIgnore = value; }
        }
        #endregion

        #region hover_system
        private void SetHovered(bool hovered)
        {
            if (hovered != this.hovered)
            {
                this.hovered = hovered;

                if (hovered) onHover?.Invoke();
                else onUnhover?.Invoke();
            }
        }

        private void CascadeUnhover()
        {
            SetHovered(false);

            foreach (UIComponent c in children)
            {
                c.CascadeUnhover();
            }
        }

        public void OnHover(Action e)
        {
            onHover += e;
        }

        public void OnUnhover(Action e)
        {
            onUnhover += e;
        }
        #endregion

        #region getters_setters
        public static void SetPixelTexture(Texture2D pixelTexture)
        {
            Button.pixelTexture = pixelTexture;
        }

        public void SetTheme(UITheme theme)
        {
            this.theme = theme;
            colour = theme.MainColour;
        }

        public void CascadeTheme(UITheme theme)
        {
            this.cascadeTheme = theme;
            this.theme = theme;
            colour = theme.MainColour;

            foreach (UIComponent c in children)
            {
                if (c.cascadeTheme == null) c.PassiveCascadeTheme(theme);
            }
        }

        private void PassiveCascadeTheme(UITheme theme)
        {
            this.theme = theme;
            colour = theme.MainColour;

            foreach (UIComponent c in children)
            {
                if (c.cascadeTheme == null) c.CascadeTheme(theme);
            }
        }

        public LUIVA.Layout GetLayout()
        {
            return layout;
        }

        public void SetLayout(LUIVA.Layout layout)
        {
            this.layout.UpdateLayout(layout);
        }

        public UITransform GetTransform()
        {
            return transform;
        }

        public void SetParent(UIComponent parent)
        {
            this.parent = parent;
            transform.Parent = parent.transform;
        }

        public void RemoveParent()
        {
            this.parent = null;
            transform.Parent = null;
        }

        public void AddChild(params UIComponent[] children)
        {
            foreach (UIComponent c in children)
            {
                c.SetParent(this);
                if (checkFocusCallback != null) c.SetCheckFocusCallback(checkFocusCallback);
                childQueue.Add(c);
            }
        }

        private void SyncChildren()
        {
            foreach (UIComponent c in childQueue)
            {
                children.Add(c);
            }
            
            childQueue.Clear();
        }

        public void ForceSynchChildren()
        {
            SyncChildren();

            foreach (UIComponent c in children)
            {
                c.ForceSynchChildren();
            }
        }

        public void RemoveChild(UIComponent child)
        {
            child.RemoveParent();
            children.Remove(child);
        }

        public IEnumerable<ILayoutable> GetChildren()
        {
            return children;
        }

        public int GetChildCount()
        {
            return children.Count;
        }

        public static void SetUpdateScissorRectangleAction(Action<Rectangle> updateScissorRectangle)
        {
            UIComponent.updateScissorRectangle = updateScissorRectangle;
        }

        public bool Visible
        {
            get { return visible; }
            set { CascadeUnhover(); visible = value; }
        }

        public bool IgnoreScissorRect
        {
            get { return ignoreScissorRect; }
            set { ignoreScissorRect = value; }
        }

        private int NewElementId()
        {
            return ++currentElement;
        }

        protected virtual string GetComponentType()
        {
            return "UIComponent";
        }

        public virtual string GetTag()
        {
            return $"{elementId}:{GetComponentType()}";
        }

        public bool RenderDefaultRect
        {
            get { return renderDefaultRect; }
            set { renderDefaultRect = value; }
        }
        #endregion

        protected virtual void OnResize()
        {
            
        }
    }
}
