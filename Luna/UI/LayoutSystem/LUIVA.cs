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

        public void SetRootLayout(ILayoutable root)
        {
            rootLayout = root;
            rootLayout.SetLayout(new Layout() { ClipChildren = true });
        }

        public void DebugLayout(bool debugMode)
        {
            this.debugMode = debugMode;
        }

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

        private void CalculateLayoutPosition(ILayoutable? parent, IEnumerable<ILayoutable> elements, int axis, bool isPrimaryAxis)
        {
            if (parent == null)
            {
                ILayoutable element = elements.ElementAt(0);
                CalculateLayoutPosition(element, element.GetChildren(), axis, element.GetLayout().LayoutAxis == axis);
            }
            else
            {

                if (isPrimaryAxis)
                {
                    //Primary Axis Logic

                    int parentSize = (int)parent.GetTransform().Size.GetComponent(axis);
                    LVector2 parentPadding = GetElementPaddingByAxis(parent, axis);
                    int parentSpacing = parent.GetLayout().Spacing;
                    int currentPosition = -1;
                    int totalElementsSize = GetTotalAxisSize(elements, parentSpacing, axis);

                    for (int i = 0; i < elements.Count(); i++)
                    {
                        ILayoutable element = elements.ElementAt(i);
                        Alignment alignment = parent.GetLayout().GetAlignmentFromAxis(axis);
                        int elementSize = (int)element.GetTransform().Size.GetComponent(axis);

                        if (currentPosition == -1)
                        {
                            switch (alignment)
                            {
                                case Alignment.Begin:
                                {
                                    currentPosition = (int)parentPadding.X;
                                    break;
                                }
                                case Alignment.Middle:
                                {
                                    currentPosition = (int)(((float)parentSize / 2) - ((float)totalElementsSize / 2));
                                    break;
                                }
                                case Alignment.End:
                                {
                                    currentPosition = parentSize - (int)parentPadding.Y - totalElementsSize;
                                    break;
                                }
                            }
                        }
                        
                        if (!element.GetLayout().Inline) continue;

                        element.GetTransform().SetPositionComponentValue(currentPosition, axis);
                        currentPosition += elementSize + parentSpacing;
                    }
                }
                else
                {
                    //Secondary Axis Logic
                    
                    int parentSize = (int)parent.GetTransform().Size.GetComponent(axis);
                    LVector2 parentPadding = GetElementPaddingByAxis(parent, axis);

                    for (int i = 0; i < elements.Count(); i++)
                    {
                        ILayoutable element = elements.ElementAt(i);

                        if (!element.GetLayout().Inline) continue;

                        Alignment alignment = parent.GetLayout().GetAlignmentFromAxis(axis);
                        int elementSize = (int)element.GetTransform().Size.GetComponent(axis);
                        float position = 0;

                        switch (alignment)
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

                        element.GetTransform().SetPositionComponentValue(position, axis);
                    }
                }

                foreach (ILayoutable e in elements)
                {
                    CalculateLayoutPosition(e, e.GetChildren(), axis, e.GetLayout().LayoutAxis == axis);
                }
            }
        }

        private int GetTotalAxisSize(IEnumerable<ILayoutable> elements, int spacing, int axis)
        {
            if (elements.Count() == 0) return 0;

            int totalSize = (elements.Count() - 1) * spacing;

            foreach (ILayoutable e in elements)
            {
                totalSize += (int)e.GetTransform().Size.GetComponent(axis);
            }

            return totalSize;
        }

        public int CalculateLayoutScale(ILayoutable? parent, IEnumerable<ILayoutable> elements, float growScaling, int axis, bool isPrimaryAxis)
        {
            LVector2 parentAxisPadding = parent == null ? LVector2.Zero : GetElementPaddingByAxis(parent, axis);
            Sizing parentSizing = parent == null ? Sizing.Grow(growScaling) : parent.GetLayout().GetSizingFromAxis(axis);
            int parentSpacing = parent == null ? 0 : parent.GetLayout().Spacing;

            DebugInfo($"Axis: {axis}, is primary: {isPrimaryAxis}.");

            if (parent == null)
            {
                DebugInfo($"Parent is null, has {elements.Count()} children");
                if (isPrimaryAxis)
                {
                    DebugInfo("Primary Axis");
                    (int fixedSize, float stretchSize) = CalculateScalingValues(elements, axis);

                    int containerSize = (int)growScaling;
                    DebugInfo($"Set container size to {containerSize}");
                    float childGrowScaling = CalculateAxisGrowScaling(fixedSize, stretchSize, containerSize, (int)parentAxisPadding.X, (int)parentAxisPadding.Y, parentSpacing, elements.Count());
                    DebugInfo($"Setting child grow scaling to {childGrowScaling}");

                    foreach (ILayoutable e in elements)
                    {
                        CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                    }
                }
                else
                {
                    DebugInfo("Secondary Axis");
                    float maxStretchSize = CalculateSecondaryScalingValue(elements, axis);

                    int containerSize = (int)growScaling;
                    DebugInfo($"Set container size to {containerSize}");
                    float childGrowScaling = (containerSize - parentAxisPadding.X - parentAxisPadding.Y) / maxStretchSize;
                    DebugInfo($"Setting child grow scaling to {childGrowScaling}");

                    foreach (ILayoutable e in elements)
                    {
                        CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                    }
                }

                return 0;
            }

            //  TODO - GO THROUGH ALL THIS AND MAKE SURE COMPONENTS THAT ARE NOT INLINE ARE CALCULATED DIFFERENTLY
            //  THEY ALREADY DON'T CONTRIBUTE TO THE SCALING VALUES, BUT MAKE SURE THEIR VALUE IS SET RELATIVE TO THE
            //  CONTAINER SIZE RATHER THAN THE NORMAL GROW SIZING
            if (isPrimaryAxis)
            {
                DebugInfo($"Parent is {parent.GetTag()}, element has {elements.Count()} children");
                (int fixedSize, float stretchSize) = CalculateScalingValues(elements, axis);
                int containerSize = (int)parent.GetTransform().Size.GetComponent(axis);
                float childGrowScaling = CalculateAxisGrowScaling(fixedSize, stretchSize, containerSize, (int)parentAxisPadding.X, (int)parentAxisPadding.Y, parentSpacing, parent.GetChildCount());

                switch (parentSizing.ScalingMode)
                {
                    case Sizing.Mode.Fixed:
                    {
                        containerSize = parentSizing.FixedSize;
                        childGrowScaling = CalculateAxisGrowScaling(fixedSize, stretchSize, containerSize, (int)parentAxisPadding.X, (int)parentAxisPadding.Y, parentSpacing, parent.GetChildCount());
                        parent.GetTransform().Size.SetComponentValue(containerSize, axis);

                        foreach (ILayoutable e in elements)
                        {
                            CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                        }

                        return parentSizing.FixedSize;
                    }
                    case Sizing.Mode.Grow:
                    {
                        containerSize = (int)(parentSizing.GrowRatio * growScaling);
                        childGrowScaling = CalculateAxisGrowScaling(fixedSize, stretchSize, containerSize, (int)parentAxisPadding.X, (int)parentAxisPadding.Y, parentSpacing, parent.GetChildCount());
                        LVector2 newParentSize = LVector2.SetComponentValue(parent.GetTransform().Size, containerSize, axis);
                        parent.GetTransform().Size = newParentSize;
                        DebugInfo($"Parent grow ratio: {parentSizing.GrowRatio}, grow scaling: {growScaling}, axis: {axis}");
                        DebugInfo($"Set parent size to {newParentSize}, parent size registering as {parent.GetTransform().Size}");

                        foreach (ILayoutable e in elements)
                        {
                            CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                        }

                        return (int)(parentSizing.GrowRatio * growScaling);
                    }
                    case Sizing.Mode.Wrap:
                    {
                        int totalDimension = (int)(parentAxisPadding.X + parentAxisPadding.Y + (parentSpacing * (parent.GetChildCount() - 1)));

                        foreach (ILayoutable e in elements)
                        {
                            if (e.GetLayout().GetSizingFromAxis(axis).ScalingMode == Sizing.Mode.Grow) throw new Exception("Attempted to use an ILayoutable with sizing mode Grow as a child of one with sizig mode Match");
                            
                            totalDimension += CalculateLayoutScale(e, e.GetChildren(), 0, axis, e.GetLayout().LayoutAxis == axis);
                        }

                        parent.GetTransform().Size.SetComponentValue(totalDimension, axis);
                        return totalDimension;
                    }
                    case Sizing.Mode.Ignore:
                    {
                        foreach (ILayoutable e in elements)
                        {
                            CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                        }

                        return (int)parent.GetTransform().Size.GetComponent(axis);
                    }
                }
            }
            else
            {
                float maxStretchSize = CalculateSecondaryScalingValue(elements, axis);
                int containerSize = (int)parent.GetTransform().Size.GetComponent(axis);
                float childGrowScaling = (containerSize - parentAxisPadding.X - parentAxisPadding.Y) / maxStretchSize;

                switch (parentSizing.ScalingMode)
                {
                    case Sizing.Mode.Fixed:
                    {
                        containerSize = parentSizing.FixedSize;
                        childGrowScaling = (containerSize - parentAxisPadding.X - parentAxisPadding.Y) / maxStretchSize;
                        parent.GetTransform().Size.SetComponentValue(containerSize, axis);

                        foreach (ILayoutable e in elements)
                        {
                            CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                        }

                        return parentSizing.FixedSize;
                    }
                    case Sizing.Mode.Grow:
                    {
                        containerSize = (int)(parentSizing.GrowRatio * growScaling);
                        childGrowScaling = (containerSize - parentAxisPadding.X - parentAxisPadding.Y) / maxStretchSize;
                        parent.GetTransform().Size.SetComponentValue(containerSize, axis);

                        foreach (ILayoutable e in elements)
                        {
                            CalculateLayoutScale(e, e.GetChildren(), e.GetLayout().Inline ? childGrowScaling : containerSize, axis, e.GetLayout().LayoutAxis == axis);
                        }

                        return (int)(parentSizing.GrowRatio * growScaling);
                    }
                    case Sizing.Mode.Wrap:
                    {
                        int dimPadding = (int)(parentAxisPadding.X + parentAxisPadding.Y);
                        int maxDimension = 0;

                        foreach (ILayoutable e in elements)
                        {
                            if (e.GetLayout().GetSizingFromAxis(axis).ScalingMode == Sizing.Mode.Grow) throw new Exception("Attempted to use an ILayoutable with sizing mode Grow as a child of one with sizig mode Match");
                            
                            maxDimension = Math.Max(maxDimension, CalculateLayoutScale(e, e.GetChildren(), 0, axis, e.GetLayout().LayoutAxis == axis));
                        }

                        parent.GetTransform().Size.SetComponentValue(dimPadding + maxDimension, axis);
                        return dimPadding + maxDimension;
                    }
                    case Sizing.Mode.Ignore:
                    {
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

        private (int, float) CalculateScalingValues(IEnumerable<ILayoutable> elements, int layoutAxis)
        {
            int totalFixedWidth = 0;
            float totalStretchRatio = 0;

            for (int i = 0; i < elements.Count(); i++)
            {
                Layout layout = elements.ElementAt(i).GetLayout();
                Sizing axisSizing = layout.GetSizingFromAxis(layoutAxis);

                if (!layout.Inline) continue;

                switch (axisSizing.ScalingMode)
                {
                    case Sizing.Mode.Grow: totalStretchRatio += axisSizing.GrowRatio; break;
                    case Sizing.Mode.Fixed: totalFixedWidth += axisSizing.FixedSize; break;
                }
            }

            DebugInfo($"Calculated scaling values to be - fixed size: {totalFixedWidth}, stretch ratio: {totalStretchRatio}");

            return (totalFixedWidth, totalStretchRatio);
        }

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

            private bool paddingChanged, layoutAxisChanged,
                horizontalAlignmentChanged, verticalAlignmentChanged, imageFitModeChanged,
                imageAlignmentChanged, layoutWidthChanged, LayoutHeightChanged, spacingChanged,
                inlineChanged;

            public Layout()
            {
                alignment = new Alignment[2];
                layoutSizing = new Sizing[2];
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
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="axis">Use Layout.HORIZONTAL or VERTICAL</param>
            /// <returns></returns>
            public Sizing GetSizingFromAxis(int axis)
            {
                return layoutSizing[axis];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="axis">Use Layout.HORIZONTAL or VERTICAL</param>
            /// <returns></returns>
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
                set { clipChildren = value; }
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
