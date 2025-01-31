using System;
using Microsoft.Xna.Framework;

namespace Luna.UI.LayoutSystem
{
    internal interface IColourAnimator
    {
        public void Update();

        public void SetColour(Color colour);

        public void ForceColour(Color color);

        public Color GetColour();

        public void OnTransitionAction(Action e);
    }
}