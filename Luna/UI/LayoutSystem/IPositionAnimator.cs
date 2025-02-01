using System;

namespace Luna.UI.LayoutSystem
{
    internal interface IPositionAnimator
    {
        public void Update();

        public void SetPosition(LVector2 position);

        public void ForcePosition(LVector2 position);

        public void MoveToTarget();

        public LVector2 GetCurrentPosition();
        
        public LVector2 GetTargetPosition();
    }
}