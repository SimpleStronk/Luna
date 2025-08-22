using Luna.ManagerClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Luna.UI.LayoutSystem;
using Microsoft.Xna.Framework.Input;

namespace Luna.UI
{
    internal abstract class UIComponent : ILayoutable
    {
        protected static Texture2D pixelTexture;
        protected UITheme cascadeTheme, overrideTheme = new UITheme();
        protected LUIVA.Layout layout = new LUIVA.Layout();
        protected UITransform transform = new UITransform();
        protected UIComponent? parent;
        protected List<UIComponent> children = new List<UIComponent>();
        private List<UIComponent> childQueue = new List<UIComponent>();
        private List<UIComponent> removeChildQueue = new List<UIComponent>();
        protected int priority = -1;
        protected bool focused = false, hovered = false;
        protected bool focusIgnore = false;
        protected Action onHover, onUnhover;
        protected Action<Action, Action, int> checkFocusCallback;
        private static Action<Rectangle> updateScissorRectangle;
        private Action onDestroy;
        protected IColourAnimator colourAnimator = new ExpColourAnimator();
        protected bool debugMode = false;
        private bool renderDefaultRect = true;

        public enum FocusDependence { Dependent, Always };
        protected bool visible = true;
        protected bool ignoreScissorRect = false;
        protected int elementId;
        protected bool textObject = false;
        protected bool scrollable = false;
        protected string name = "";

        private static int currentElement = 0;

        public void Initialise()
        {
            elementId = NewElementId();
            SetDebugActions();
            transform.Size.OnChanged(OnResize);
            transform.OnResize(OnResize);
        }

        /// <summary>
        /// Called on all UIComponents before the main update function
        /// </summary>
        public void PreUpdate()
        {
            // Reports to an outside class if this object is hovered, if a deeper
            // object is hovered then focus is updated to that later.
            CheckFocused(TestMouseCollision());
            transform.Update();

            foreach (UIComponent c in children)
            {
                c.PreUpdate();
            }
        }

        /// <summary>
        /// Performs base update functionality on self and all child components recursively
        /// </summary>
        public void BaseUpdate()
        {
            SyncChildren();
            transform.Padding = layout.Padding;

            if (focused) CheckScroll();

            if (!visible) { colourAnimator.SetColour(new Color(0, 0, 0, 0)); return; }

            foreach (UIComponent c in children)
            {
                c.BaseUpdate();
            }

            // Only set hovered if the mouse is over component AND no
            // deeper components are hovered (I.E. this object has focus).
            SetHovered(TestMouseCollision() && focused);

            //Virtual update function, overridden by derived classes
            Update();
        }

        /// <summary>
        /// Called on all UIComponents after the main update function
        /// </summary>
        public void PostUpdate()
        {
            colourAnimator.Update();

            foreach (UIComponent c in children)
            {
                c.PostUpdate();
            }
        }

        /// <summary>
        /// Default drawing behaviour of the UIComponent
        /// </summary>
        /// <param name="s"></param>
        /// <exception cref="Exception"></exception>
        public void BaseDraw(SpriteBatch s)
        {
            // Are we abiding by the scissor rectangle? If not, use root component's scissor rect.
            updateScissorRectangle(ignoreScissorRect ? GetRootRectangle() : CalculateScissorRectangle());

            if (renderDefaultRect)
            {
                if (pixelTexture == null) throw new Exception("pixelTexture not initialised in class UIComponent");

                (int tl, int tr, int bl, int br) = UITheme.GetCornerRadius(GetCorrectTheme(overrideTheme.CornerRadiusChanged), transform);

                // If we can ignore the rounded corners, just ignore them
                if (UITheme.GetCornerRadius(GetCorrectTheme(overrideTheme.CornerRadiusChanged), transform) == (0, 0, 0, 0) || !GetCorrectTheme(overrideTheme.RoundedChanged).Rounded)
                {
                    s.Draw(pixelTexture, transform.GetGlobalRect(), new Rectangle(0, 0, 1, 1),
                        colourAnimator.GetColour(), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                }
                else
                {
                    UITheme correctTheme = GetCorrectTheme(overrideTheme.CornerRadiusChanged);
                    // Draw each rounded corner, only if it is rounded
                    if (tl > 0) s.Draw(UITheme.TopLeftTexture(correctTheme, transform), UITheme.TopLeftRect(correctTheme, transform), new Rectangle(0, 0, tl, tl), colourAnimator.GetColour(), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    if (tr > 0) s.Draw(UITheme.TopRightTexture(correctTheme, transform), UITheme.TopRightRect(correctTheme, transform), new Rectangle(tr, 0, tr, tr), colourAnimator.GetColour(), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    if (bl > 0) s.Draw(UITheme.BottomLeftTexture(correctTheme, transform), UITheme.BottomLeftRect(correctTheme, transform), new Rectangle(0, bl, bl, bl), colourAnimator.GetColour(), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    if (br > 0) s.Draw(UITheme.BottomRightTexture(correctTheme,transform), UITheme.BottomRightRect(correctTheme, transform), new Rectangle(br, br, br, br), colourAnimator.GetColour(), 0, new Vector2(0, 0), SpriteEffects.None, 1);

                    foreach (Rectangle r in UITheme.FillRectangles(correctTheme, transform))
                    {
                        s.Draw(pixelTexture, r, new Rectangle(0, 0, 1, 1), colourAnimator.GetColour(), 0, new Vector2(0, 0), SpriteEffects.None, 1);
                    }
                }
            }
            
            // Virtual draw function, overridden in derived classes
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
            return Rectangle.Intersect(CalculateScissorRectangle(), transform.GetGlobalRect()).Contains(MouseHandler.Position);
        }

        /// <summary>
        /// Recursively designates hierarchy priority in a tree-like fashion, where succeeding siblings have
        /// greater priority than preceding siblings and all their children.
        /// </summary>
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

        /// <summary>
        /// Recursively intersects this UIComponent's rectangle with parent rectangles to calculate
        /// appropriate scissor bounds
        /// </summary>
        Rectangle CalculateScissorRectangle()
        {
            if (!layout.ClipChildren) return parent.CalculateScissorRectangle();

            return parent == null ? transform.GetGlobalRect() :
                Rectangle.Intersect(transform.GetGlobalRect(), parent.CalculateScissorRectangle());
        }

        /// <summary>
        /// Get the bounds rectangle of the root component
        /// </summary>
        Rectangle GetRootRectangle()
        {
            if (parent == null) return transform.GetGlobalRect();
            else return parent.GetRootRectangle();
        }

        public void Destroy()
        {
            parent?.RemoveChild(this);
            onDestroy?.Invoke();
        }

        public void OnDestroy(Action onDestroy)
        {
            this.onDestroy += onDestroy;
        }

        /// <summary>
        /// Check whether the scroll wheel, or page up/down keys have been used
        /// </summary>
        private void CheckScroll()
        {
            if (!scrollable) { parent?.CheckScroll(); return; }

            if (KeyboardHandler.IsKeyJustPressed(Keys.PageUp)) transform.Scroll(240);
            if (KeyboardHandler.IsKeyJustPressed(Keys.PageDown)) transform.Scroll(-240);

            transform.Scroll(MouseHandler.DeltaScroll);
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

        /// <summary>
        /// Attach debug info to behaviour callbacks
        /// </summary>
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
            if (!mouseOver || focusIgnore || !visible) return;

            //Passes the outside class a function to call if this has priority
            checkFocusCallback(() => SetFocused(true), () => SetFocused(false), priority);
        }

        protected virtual void SetFocused(bool focused)
        {
            this.focused = focused;
        }

        /// <summary>
        /// Recursively sets the CheckFocus callback of this and all children
        /// </summary>
        /// <param name="e"></param>
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

        /// <summary>
        /// Unhover this and all child components
        /// </summary>
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
            overrideTheme.UpdateTheme(theme);
            colourAnimator.SetColour(textObject ? theme.GetColourPalatte(cascadeTheme).TextColour : theme.GetColourPalatte(cascadeTheme).MainColour);
        }

        public UITheme GetTheme()
        {
            return overrideTheme;
        }

        /// <summary>
        /// Set the theme for this component and all child components
        /// </summary>
        /// <param name="cascadeTheme"></param>
        public void CascadeTheme(UITheme cascadeTheme)
        {
            if (cascadeTheme == null) return;

            this.cascadeTheme = cascadeTheme;
            colourAnimator.SetColour(textObject ? overrideTheme.GetColourPalatte(cascadeTheme).TextColour : overrideTheme.GetColourPalatte(cascadeTheme).MainColour);

            foreach (UIComponent c in children)
            {
                c.CascadeTheme(cascadeTheme);
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

        public UIComponent GetParent()
        {
            return parent;
        }

        public void RemoveParent()
        {
            parent = null;
            transform.Parent = null;
        }

        public virtual void AddChild(params UIComponent[] children)
        {
            foreach (UIComponent c in children)
            {
                c.SetParent(this);
                childQueue.Add(c);
            }
        }

        /// <summary>
        /// Make sure all new child components have the same CheckFocus callback and cascade theme,
        /// and clean up after all child objects pending termination
        /// </summary>
        private void SyncChildren()
        {
            foreach (UIComponent c in childQueue)
            {
                c.CascadeTheme(cascadeTheme);
                if (checkFocusCallback != null) c.SetCheckFocusCallback(checkFocusCallback);
                children.Add(c);
            }

            foreach (UIComponent c in removeChildQueue)
            {
                c.RemoveParent();
                c.onDestroy?.Invoke();
                children.Remove(c);
            }
            
            childQueue.Clear();
            removeChildQueue.Clear();
        }

        public void RemoveChild(UIComponent child)
        {
            removeChildQueue.Add(child);
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

        public IColourAnimator ColourAnimator
        {
            get { return colourAnimator; }
        }

        public bool Visible
        {
            get { return visible; }
            set { CascadeUnhover(); visible = value; }
        }

        public void ForceTransparent()
        {
            colourAnimator.ForceColour(new Color(0, 0, 0, 0));
            foreach (UIComponent c in children)
            {
                c.ForceTransparent();
            }
        }

        public bool IgnoreScissorRect
        {
            get { return ignoreScissorRect; }
            set { ignoreScissorRect = value; }
        }

        public bool Scrollable
        {
            get { return scrollable; }
            set { scrollable = value; transform.Scrollable = value; }
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

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string GetName()
        {
            return name;
        }

        public bool RenderDefaultRect
        {
            get { return renderDefaultRect; }
            set { renderDefaultRect = value; }
        }

        protected UITheme GetCorrectTheme(bool propertyChanged)
        {
            // Returns cascadeTheme or theme depending on whether theme's property has been changed
            return propertyChanged ? overrideTheme : cascadeTheme;
        }

        #endregion

        protected virtual void OnResize()
        {

        }
    }
}
