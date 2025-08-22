using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.DXGI;

namespace Luna.UI.LayoutSystem
{
    internal class LUIVA
    {
        public enum Alignment { Begin, Middle, End, Ignore }
        public enum Direction { Forward, Backward };

        private ILayoutable rootLayout;
        private int displayWidth;
        private int displayHeight;
        private bool debugMode = false;

        /// <summary>
        /// Sets the root layout for the layout algorithm
        /// </summary>
        /// <param name="root"></param>
        public void SetRootLayout(ILayoutable root)
        {
            rootLayout = root;
            rootLayout.SetLayout(new Layout() { ClipChildren = true });
        }

        /// <param name="debugMode">If <c>true</c>, LUIVA will output all of its procedures</param>
        public void DebugLayout(bool debugMode)
        {
            this.debugMode = debugMode;
        }

        /// <summary>
        /// Calculates the sizing and positioning of all elements
        /// </summary>
        public void CalculateLayout()
        {
            DebugInfo("Performing horizontal layout");
            CalculateLayoutScale(null, new List<ILayoutable>() { rootLayout }, displayWidth, LVector2.HORIZONTAL, false);
            DebugInfo("Performing vertical layout");
            CalculateLayoutScale(null, new List<ILayoutable>() { rootLayout }, displayHeight, LVector2.VERTICAL, true);
            DebugInfo("Performing horizontal positioning");
            CalculateLayoutPosition(null, new List<ILayoutable>() {rootLayout}, LVector2.HORIZONTAL, false);
            DebugInfo("Performing vertical positioning");
            CalculateLayoutPosition(null, new List<ILayoutable>() {rootLayout}, LVector2.VERTICAL, true);
        }

        /// <summary>
        /// Works out where every UI element should be on the screen on the given axis,
        /// based on parent position and size
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="elements"></param>
        /// <param name="axis"></param>
        /// <param name="isPrimaryAxis"></param>
        private void CalculateLayoutPosition(ILayoutable? parent, IEnumerable<ILayoutable> elements, int axis, bool isPrimaryAxis)
        {
            if (parent == null)
            {
                ILayoutable element = elements.ElementAt(0);
                CalculateLayoutPosition(element, element.GetChildren(), axis, element.GetLayout().LayoutAxis == axis);
            }
            else
            {
                // Primary Axis Logic
                if (isPrimaryAxis)
                {
                    // The parent element's size and padding on this axis, as well as spacing and alignment
                    int parentSize = (int)parent.GetTransform().Size.GetComponent(axis);
                    LVector2 parentPadding = GetElementPaddingByAxis(parent, axis);
                    int parentSpacing = parent.GetLayout().Spacing;
                    Alignment parentAlignment = parent.GetLayout().GetAlignmentFromAxis(axis);

                    // The combined size of all children
                    int totalElementsSize = GetTotalAxisSize(elements, parentSpacing, axis);

                    // Where to start layout from
                    int startingPosition = 0;

                    switch (parentAlignment)
                    {
                        case Alignment.Begin:
                            {
                                startingPosition = (int)parentPadding.X;
                                break;
                            }
                        case Alignment.Middle:
                            {
                                startingPosition = (int)(((float)parentSize / 2) - ((float)totalElementsSize / 2));
                                break;
                            }
                        case Alignment.End:
                            {
                                startingPosition = parentSize - (int)parentPadding.Y - totalElementsSize;
                                break;
                            }
                    }

                    // The current element is the first to be considered, so put it at the start of the layout
                    int currentPosition = startingPosition;

                    for (int i = 0; i < elements.Count(); i++)
                    {
                        // The currently considered child element, with its size
                        ILayoutable element = elements.ElementAt(i);
                        int elementSize = (int)element.GetTransform().Size.GetComponent(axis);

                        // If we're ignoring alignment, we don't want to change it at all
                        if (parentAlignment == Alignment.Ignore) continue;

                        // Non-inline elements should be placed based on the starting position, rather than the current position
                        if (!element.GetLayout().Inline)
                        {
                            element.GetTransform().SetPositionComponentValue(startingPosition, axis);
                            continue;
                        }

                        // Place at the current position (taking into account scroll offset)
                        // and increment currentPosition
                        element.GetTransform().SetPositionComponentValue(currentPosition + element.GetTransform().Parent.ScrollOffset.GetComponent(axis), axis);
                        currentPosition += elementSize + parentSpacing;
                    }
                }
                // Seconday Axis Logic
                else
                {
                    // The parent element's size and padding on this axis, as well as alignment
                    int parentSize = (int)parent.GetTransform().Size.GetComponent(axis);
                    LVector2 parentPadding = GetElementPaddingByAxis(parent, axis);
                    Alignment parentAlignment = parent.GetLayout().GetAlignmentFromAxis(axis);

                    for (int i = 0; i < elements.Count(); i++)
                    {
                        // The currently considered child element
                        ILayoutable element = elements.ElementAt(i);

                        int elementSize = (int)element.GetTransform().Size.GetComponent(axis);
                        float position = 0;

                        // Set position based on parent alignment (for middle, is subject to the width of the child element)
                        switch (parentAlignment)
                        {
                            case Alignment.Begin:
                                {
                                    position = parentPadding.X;
                                    break;
                                }
                            case Alignment.Middle:
                                {
                                    position = (int)(((float)parentSize / 2) - ((float)elementSize / 2));
                                    break;
                                }
                            case Alignment.End:
                                {
                                    position = (parentSize - elementSize) - (int)parentPadding.Y;
                                    break;
                                }
                        }

                        // If we're ignoring alignment, we don't want to change it at all
                        if (parentAlignment == Alignment.Ignore) continue;
                        
                        // Place at the right position, taking into account scroll offset
                        element.GetTransform().SetPositionComponentValue(position + element.GetTransform().Parent.ScrollOffset.GetComponent(axis), axis);
                    }
                }

                // Recursively call this function for all child elements
                foreach (ILayoutable e in elements)
                {
                    CalculateLayoutPosition(e, e.GetChildren(), axis, e.GetLayout().LayoutAxis == axis);
                }
            }
        }

        /// <summary>
        /// Calculates the total size of all given elements along the given axis, taking into account the spacing between them
        /// </summary>
        /// <param name="elements">The elements to measure</param>
        /// <param name="spacing">The space between the given elements</param>
        /// <param name="axis">The current axis to consider</param>
        private int GetTotalAxisSize(IEnumerable<ILayoutable> elements, int spacing, int axis)
        {
            if (elements.Count() == 0) return 0;

            // Start by adding all the spacings to the total
            int totalSize = (elements.Count() - 1) * spacing;

            foreach (ILayoutable e in elements)
            {
                // Ignore non-inline elements, as they don't contribute
                if (!e.GetLayout().Inline) continue;
                
                totalSize += (int)e.GetTransform().Size.GetComponent(axis);
            }

            return totalSize;
        }

        /// <summary>
        /// Works out how large each UI element should be on the screen on the given axis,
        /// based on parent position and size
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="elements"></param>
        /// <param name="growScaling"></param>
        /// <param name="axis"></param>
        /// <param name="isPrimaryAxis"></param>
        /// <returns>The size calculated for the parent value, for use in recursive size calculation</returns>
        /// <exception cref="Exception"></exception>
        public int CalculateLayoutScale(ILayoutable? parent, IEnumerable<ILayoutable> elements, float growScaling, int axis, bool isPrimaryAxis)
        {
            // The padding, Sizing and spacing of the parent element (or default values if there is no parent)
            LVector2 parentAxisPadding = parent == null ? LVector2.Zero : GetElementPaddingByAxis(parent, axis);
            Sizing parentSizing = parent == null ? Sizing.Grow(growScaling) : parent.GetLayout().GetSizingFromAxis(axis);
            int parentSpacing = parent == null ? 0 : parent.GetLayout().Spacing;

            DebugInfo($"Scaling axis: {axis}, is primary: {isPrimaryAxis}.");

            if (parent == null)
            {
                DebugInfo($"Parent is null, has {elements.Count()} children");

                // Primary Axis Logic
                if (isPrimaryAxis)
                {
                    DebugInfo("Primary Axis");
                    // Calculates the total fixed size of the child elements, and the total stretch size (the sum of
                    // all values in the Sizing.Grow() function
                    (int fixedSize, float stretchSize) = CalculateScalingValues(elements, axis);

                    // For a null parent, we use the growScaling value to convey the container size
                    int containerSize = (int)growScaling;
                    DebugInfo($"Set container size to {containerSize}");

                    // Work out how many pixels a grow value of 1 equates to
                    float childGrowScaling = CalculateAxisGrowScaling(fixedSize, stretchSize, containerSize, (int)parentAxisPadding.X, (int)parentAxisPadding.Y, parentSpacing, elements.Count());
                    DebugInfo($"Setting child grow scaling to {childGrowScaling}");

                    // Recursively calculate the scaling of all child elements
                    foreach (ILayoutable e in elements)
                    {
                        CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                    }
                }
                // Secondary Axis Logic
                else
                {
                    DebugInfo("Secondary Axis");
                    // Calculate the greatest grow value on this axis
                    float maxStretchSize = CalculateSecondaryScalingValue(elements, axis);

                    int containerSize = (int)growScaling;
                    DebugInfo($"Set container size to {containerSize}");
                    // Equate a grow value of 1 with the correct number of pixels - ensures that a grow value of maxStretchSize
                    // will correctly fill the parent
                    float childGrowScaling = (containerSize - parentAxisPadding.X - parentAxisPadding.Y) / maxStretchSize;
                    DebugInfo($"Setting child grow scaling to {childGrowScaling}");

                    // Recursively calculate the scaling of all child elements
                    foreach (ILayoutable e in elements)
                    {
                        CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                    }
                }

                return 0;
            }

            // Primary Axis Logic, where parent exists
            if (isPrimaryAxis)
            {
                DebugInfo($"Parent is {parent.GetTag()}, element has {elements.Count()} children");

                // Calculates the total fixed size of the child elements, and the total stretch size (the sum of
                // all values in the Sizing.Grow() function
                (int fixedSize, float stretchSize) = CalculateScalingValues(elements, axis);

                // The size of the parent on this axis
                int containerSize = (int)parent.GetTransform().Size.GetComponent(axis);

                // How many pixels a grow value of 1 should equate to
                float childGrowScaling = CalculateAxisGrowScaling(fixedSize, stretchSize, containerSize, (int)parentAxisPadding.X, (int)parentAxisPadding.Y, parentSpacing, parent.GetChildCount());

                switch (parentSizing.ScalingMode)
                {
                    // Parent is of a fixed size - we can update the parent size and then recursively
                    // calculate the sizes of all the child elements
                    case Sizing.Mode.Fixed:
                        {
                            containerSize = parentSizing.FixedSize;

                            // Set parent size
                            parent.GetTransform().Size.SetComponentValue(containerSize, axis);

                            // Recursively calculate child sizes
                            foreach (ILayoutable e in elements)
                            {
                                CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                            }

                            return parentSizing.FixedSize;
                        }
                    // Parent grows to fill its container - we can update the parent size and then recursively
                    //  calculate the sizes of all the child elements
                    case Sizing.Mode.Grow:
                        {
                            // How large of an area we have allocated for the parent
                            containerSize = (int)(parentSizing.GrowRatio * growScaling);
                            
                            // Set parent size
                            parent.GetTransform().Size.SetComponentValue(containerSize, axis);
                            DebugInfo($"Parent grow ratio: {parentSizing.GrowRatio}, grow scaling: {growScaling}, axis: {axis}");
                            DebugInfo($"Set parent size to {containerSize}, parent size registering as {parent.GetTransform().Size}");

                            // Recursively calculate child sizes
                            foreach (ILayoutable e in elements)
                            {
                                CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                            }

                            return (int)(parentSizing.GrowRatio * growScaling);
                        }
                    // Parent is sized to fit exactly around all of the child elements - we need to recursively
                    // calculate all child sizes and then set the parent size
                    case Sizing.Mode.Wrap:
                        {
                            // Consider the total dimension as just the total padding and spacing
                            int totalDimension = (int)(parentAxisPadding.X + parentAxisPadding.Y + (parentSpacing * (parent.GetChildCount() - 1)));

                            // Increase totalDimension by the size of each element (while recursively calculating child sizes)
                            foreach (ILayoutable e in elements)
                            {
                                // DO NOT ALLOW A GROW SIZING WITHIN A WRAP SIZING
                                if (e.GetLayout().GetSizingFromAxis(axis).ScalingMode == Sizing.Mode.Grow) throw new Exception(string.Format("Attempted to use an ILayoutable with sizing mode Grow ({0}) as a child of one with sizing mode Match ({1})", e.GetName(), parent.GetName()));

                                totalDimension += CalculateLayoutScale(e, e.GetChildren(), 0, axis, e.GetLayout().LayoutAxis == axis);
                            }

                            // Set parent size to wrap around all children
                            parent.GetTransform().Size.SetComponentValue(totalDimension, axis);
                            return totalDimension;
                        }
                    // Parent shouldn't abide by LUIVA sizing. Don't change parent size, just calculate child sizes
                    case Sizing.Mode.Ignore:
                        {
                            // Recursively calculate child sizes
                            foreach (ILayoutable e in elements)
                            {
                                CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                            }

                            return (int)parent.GetTransform().Size.GetComponent(axis);
                        }
                }
            }
            // Secondary Axis Logic, where parent exists
            else
            {
                // Calculate the greatest grow value on this axis
                float maxStretchSize = CalculateSecondaryScalingValue(elements, axis);

                // The size of the parent on this axis
                int containerSize = (int)parent.GetTransform().Size.GetComponent(axis);

                // Equate a grow value of 1 with the correct number of pixels - ensures that a grow value of maxStretchSize
                // will correctly fill the parent
                float childGrowScaling = (containerSize - parentAxisPadding.X - parentAxisPadding.Y) / maxStretchSize;

                switch (parentSizing.ScalingMode)
                {
                    // Parent is of a fixed size - we can update the parent size and then recursively
                    // calculate the sizes of all the child elements
                    case Sizing.Mode.Fixed:
                        {
                            containerSize = parentSizing.FixedSize;

                            // Set parent size
                            parent.GetTransform().Size.SetComponentValue(containerSize, axis);

                            // Recursively calculate child sizes
                            foreach (ILayoutable e in elements)
                            {
                                CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                            }

                            return parentSizing.FixedSize;
                        }
                    // Parent grows to fill its container - we can update the parent size and then recursively
                    //  calculate the sizes of all the child elements
                    case Sizing.Mode.Grow:
                        {
                            containerSize = (int)(parentSizing.GrowRatio * growScaling);

                            // Set parent size
                            parent.GetTransform().Size.SetComponentValue(containerSize, axis);

                            // Recursively calcalate child sizes
                            foreach (ILayoutable e in elements)
                            {
                                CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                            }

                            return (int)(parentSizing.GrowRatio * growScaling);
                        }
                    // Parent is sized to fit exactly around all of the child elements - we need to recursively
                    // calculate all child sizes and then set the parent size
                    case Sizing.Mode.Wrap:
                        {
                            int dimPadding = (int)(parentAxisPadding.X + parentAxisPadding.Y);
                            int maxDimension = 0;

                            // Recursively calculate child sizes, while keeping track of the largest child element
                            foreach (ILayoutable e in elements)
                            {
                                // DO NOT ALLOW A GROW SIZING WITHIN A WRAP SIZING
                                if (e.GetLayout().GetSizingFromAxis(axis).ScalingMode == Sizing.Mode.Grow) throw new Exception("Attempted to use an ILayoutable with sizing mode Grow as a child of one with sizig mode Wrap");

                                maxDimension = Math.Max(maxDimension, CalculateLayoutScale(e, e.GetChildren(), 0, axis, e.GetLayout().LayoutAxis == axis));
                            }

                            // Set parent size
                            parent.GetTransform().Size.SetComponentValue(dimPadding + maxDimension, axis);

                            return dimPadding + maxDimension;
                        }
                    // Parent shouldn't abide by LUIVA sizing. Don't change parent size, just calculate child sizes
                    case Sizing.Mode.Ignore:
                        {
                            // Recursively calculate child sizes
                            foreach (ILayoutable e in elements)
                            {
                                CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                            }

                            return (int)parent.GetTransform().Size.GetComponent(axis);
                        }
                }
            }
            return 0;
        }

        /// <summary>
        /// Calculates the number of pixels that a grow value of 1 should equate to
        /// </summary>
        /// <param name="primaryFixedSize">The total size of all fixed-size child elements</param>
        /// <param name="primaryStretchSize">The total of all grow values</param>
        /// <param name="containerSize">Parent element size</param>
        /// <param name="paddingStart">The top/left padding dependent on axis</param>
        /// <param name="paddingEnd">The bottom/right padding dependent on axis</param>
        /// <param name="spacing">The spacing between elements</param>
        /// <param name="childCount">The number of child elements</param>
        private float CalculateAxisGrowScaling(float primaryFixedSize, float primaryStretchSize, int containerSize, int paddingStart, int paddingEnd, int spacing, int childCount)
        {
            return (containerSize - (paddingStart + paddingEnd + primaryFixedSize + (childCount - 1) * spacing)) / primaryStretchSize;
        }

        private Tetra GetElementPadding(ILayoutable element)
        {
            return element == null ? Tetra.Zero : element.GetLayout().Padding;
        }

        private LVector2 GetElementPaddingByAxis(ILayoutable element, int axis)
        {
            Tetra elementPadding = GetElementPadding(element);
            int paddingStart = axis == LVector2.HORIZONTAL ? elementPadding.left : elementPadding.Top;
            int paddingEnd = axis == LVector2.HORIZONTAL ? elementPadding.Right : elementPadding.Bottom;
            return new LVector2(paddingStart, paddingEnd);
        }

        /// <summary>
        /// Works out the total size of all fixed-size elements and the total of all grow values
        /// </summary>
        /// <param name="elements">The elements to consider</param>
        /// <param name="layoutAxis">The axis to consider</param>
        /// <returns>A tuple containing total fixed size and total stretch ratio</returns>
        private (int, float) CalculateScalingValues(IEnumerable<ILayoutable> elements, int layoutAxis)
        {
            int totalFixedWidth = 0;
            float totalStretchRatio = 0;

            for (int i = 0; i < elements.Count(); i++)
            {
                Layout layout = elements.ElementAt(i).GetLayout();
                Sizing axisSizing = layout.GetSizingFromAxis(layoutAxis);

                // Ignore non-inline
                if (!layout.Inline) continue;

                switch (axisSizing.ScalingMode)
                {
                    // Add grow ratio of grow element to total stretch ratio
                    case Sizing.Mode.Grow: totalStretchRatio += axisSizing.GrowRatio; break;

                    // Add fixed size of fixed element to total fixed size
                    case Sizing.Mode.Fixed: totalFixedWidth += axisSizing.FixedSize; break;

                    // Adds the actual size of ignore element to the total fixed size
                    case Sizing.Mode.Ignore: totalFixedWidth += (int)elements.ElementAt(i).GetTransform().Size.GetComponent(layoutAxis); break;

                    // Adds the actual size of wrap element to the total fixed size
                    case Sizing.Mode.Wrap: totalFixedWidth += (int)elements.ElementAt(i).GetTransform().Size.GetComponent(layoutAxis); break;
                }
            }

            DebugInfo($"Calculated scaling values to be - fixed size: {totalFixedWidth}, stretch ratio: {totalStretchRatio}");

            return (totalFixedWidth, totalStretchRatio);
        }

        /// <summary>
        /// Works out the greatest grow value of all the given elements, on the given axis
        /// </summary>
        /// <param name="elements">The elements to consider</param>
        /// <param name="layoutAxis">The axis to consider</param>
        private float CalculateSecondaryScalingValue(IEnumerable<ILayoutable> elements, int layoutAxis)
        {
            float maxStretchRatio = 0;

            for (int i = 0; i < elements.Count(); i++)
            {
                Sizing axisSizing = elements.ElementAt(i).GetLayout().GetSizingFromAxis(layoutAxis);

                if (axisSizing.ScalingMode == Sizing.Mode.Grow) maxStretchRatio = Math.Max(maxStretchRatio, axisSizing.GrowRatio);
            }

            return maxStretchRatio;
        }

        public int DisplayWidth
        {
            get { return displayWidth; }
        }

        public int DisplayHeight
        {
            get { return displayHeight; }
        }

        public void SetDisplayDimensions(int displayWidth, int displayHeight)
        {
            this.displayWidth = displayWidth;
            this.displayHeight = displayHeight;
        }

        private void DebugInfo(string info)
        {
            if (debugMode) Console.WriteLine(info);
        }

        public class Layout
        {
            private Tetra padding = Tetra.Zero;
            private int layoutAxis = 0;
            private Alignment[] alignment;
            public enum FitMode { MinFit, MaxFit };
            private FitMode imageFitMode;
            private Alignment imageAlignment;
            private Sizing[] layoutSizing;
            private int spacing;
            private bool clipChildren = true;
            private bool inline = true;

            // Records true whenever a particular property is changed, so that the layout can be updated by
            // passing a different, updated object, and only the changed values overriding the original values
            private bool paddingChanged, layoutAxisChanged,
                horizontalAlignmentChanged, verticalAlignmentChanged, imageFitModeChanged,
                imageAlignmentChanged, layoutWidthChanged, LayoutHeightChanged, spacingChanged,
                clipChildrenChanged, inlineChanged;

            public Layout()
            {
                alignment = new Alignment[2];
                layoutSizing = new Sizing[2];

                // Initialise horizontal and vertical sizing as otherwise errors happen
                layoutSizing[0] = layoutSizing[1] = Sizing.Fixed(100);
            }

            public Tetra Padding
            {
                get { return padding; }
                set { padding = value; paddingChanged = true; }
            }

            public int LayoutAxis
            {
                get { return layoutAxis; }
                set { layoutAxis = value; layoutAxisChanged = true; }
            }

            // public Direction LayoutDirection
            // {
            //     get { return layoutDirection; }
            //     set { layoutDirection = value; layoutDirectionChanged = true; }
            // }

            public Alignment HorizontalAlignment
            {
                get { return alignment[LVector2.HORIZONTAL]; }
                set { alignment[LVector2.HORIZONTAL] = value; horizontalAlignmentChanged = true; }
            }

            public Alignment VerticalAlignment
            {
                get { return alignment[LVector2.VERTICAL]; }
                set { alignment[LVector2.VERTICAL] = value; verticalAlignmentChanged = true; }
            }

            public FitMode ImageFitMode
            {
                get { return imageFitMode; }
                set { imageFitMode = value; imageFitModeChanged = true;}
            }

            public Alignment ImageAlignment
            {
                get { return imageAlignment; }
                set { imageAlignment = value; imageAlignmentChanged = true; }
            }

            public Sizing LayoutWidth
            {
                get { return layoutSizing[LVector2.HORIZONTAL]; }
                set { layoutSizing[LVector2.HORIZONTAL] = value; layoutWidthChanged = true; }
            }

            public Sizing LayoutHeight
            {
                get { return layoutSizing[LVector2.VERTICAL]; }
                set { layoutSizing[LVector2.VERTICAL] = value; LayoutHeightChanged = true; }
            }

            /// <param name="axis">Use LVector2.HORIZONTAL or LVector2.VERTICAL</param>
            public Sizing GetSizingFromAxis(int axis)
            {
                return layoutSizing[axis];
            }

            /// <param name="axis">Use LVector2.HORIZONTAL or LVector2.VERTICAL</param>
            public Alignment GetAlignmentFromAxis(int axis)
            {
                return alignment[axis];
            }

            public int Spacing
            {
                get { return spacing; }
                set { spacing = value; spacingChanged = true; }
            }

            public bool ClipChildren
            {
                get { return clipChildren; }
                set { clipChildren = value; clipChildrenChanged = true; }
            }

            public bool Inline
            {
                get { return inline; }
                set { inline = value; inlineChanged = true; }
            }

            public static int AltAxis(int axis)
            {
                return 1 - axis;
            }

            /// <summary>
            /// Changes only the values of this object which have been updated in the given Layout object
            /// </summary>
            public void UpdateLayout(Layout layout)
            {
                if (layout.paddingChanged) padding = layout.padding;
                if (layout.layoutAxisChanged) layoutAxis = layout.layoutAxis;
                // if (layout.layoutDirectionChanged) layoutDirection = layout.layoutDirection;
                if (layout.horizontalAlignmentChanged) alignment[LVector2.HORIZONTAL] = layout.alignment[LVector2.HORIZONTAL];
                if (layout.verticalAlignmentChanged) alignment[LVector2.VERTICAL] = layout.alignment[LVector2.VERTICAL];
                if (layout.imageFitModeChanged) imageFitMode = layout.imageFitMode;
                if (layout.imageAlignmentChanged) imageAlignment = layout.imageAlignment;
                if (layout.layoutWidthChanged) layoutSizing[LVector2.HORIZONTAL] = layout.layoutSizing[LVector2.HORIZONTAL];
                if (layout.LayoutHeightChanged) layoutSizing[LVector2.VERTICAL] = layout.layoutSizing[LVector2.VERTICAL];
                if (layout.spacingChanged) spacing = layout.spacing;
                if (layout.clipChildrenChanged) clipChildren = layout.clipChildren;
                if (layout.inlineChanged) inline = layout.inline;
            }
        }

        public class Tetra
        {
            public int top, bottom, left, right;

            public Tetra(int all)
            {
                top = bottom = left = right = all;
            }

            public Tetra(int top, int bottom, int left, int right)
            {
                this.top = top;
                this.bottom = bottom;
                this.left = left;
                this.right = right;
            }

            public int Top
            {
                get { return top; }
                set { top = value; }
            }

            public int Bottom
            {
                get { return bottom; }
                set { bottom = value; }
            }

            public int Left
            {
                get { return left; }
                set { left = value; }
            }

            public int Right
            {
                get { return right; }
                set { right = value; }
            }

            public int GetAxis(int axis)
            {
                if (axis == LVector2.HORIZONTAL) return Left + Right;
                if (axis == LVector2.VERTICAL) return Top + Bottom;
                return 0;
            }

            public int AxisStart(int axis)
            {
                if (axis == LVector2.HORIZONTAL) return Left;
                if (axis == LVector2.VERTICAL) return Top;
                return 0;
            }

            public int AxisEnd(int axis)
            {
                if (axis == LVector2.HORIZONTAL) return Right;
                if (axis == LVector2.VERTICAL) return Bottom;
                return 0;
            }

            public static Tetra Zero
            {
                get { return new Tetra(0); }
            }
        }

        public class Sizing
        {
            public enum Mode { Grow, Fixed, Wrap, Ignore }
            private Mode mode;
            private int size;
            private float ratio;

            public static Sizing Grow(float ratio)
            {
                Sizing ls = new Sizing();
                ls.ratio = ratio;
                ls.mode = Mode.Grow;
                return ls;
            }

            public static Sizing Fixed(int size)
            {
                Sizing ls = new Sizing();
                ls.size = size;
                ls.mode = Mode.Fixed;
                return ls;
            }

            public static Sizing Wrap()
            {
                Sizing ls = new Sizing();
                ls.mode = Mode.Wrap;
                return ls;
            }

            public static Sizing Ignore()
            {
                Sizing ls = new Sizing();
                ls.mode = Mode.Ignore;
                return ls;
            }

            public Mode ScalingMode
            {
                get { return mode; }
            }

            public int FixedSize
            {
                get { return size; }
            }

            public float GrowRatio
            {
                get { return ratio; }
            }
        }
    }
}
