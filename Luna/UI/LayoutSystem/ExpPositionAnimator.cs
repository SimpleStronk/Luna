using System;

namespace Luna.UI.LayoutSystem
{
    internal class ExpPositionAnimator : IPositionAnimator
    {
        private LVector2 currentPosition = new LVector2(0, 0), targetPosition = new LVector2(0, 0);
        private float dampingFactor = 5f;

        public void Update()
        {
            currentPosition += (targetPosition - currentPosition) / dampingFactor;
        }

        public void SetPosition(LVector2 position)
        {
            targetPosition = position;
        }

        public void ForcePosition(LVector2 position)
        {
            currentPosition = targetPosition = position;
        }

        public void MoveToTarget()
        {
            currentPosition = targetPosition;
        }

        public LVector2 GetCurrentPosition()
        {
            return currentPosition;
        }

        public LVector2 GetTargetPosition()
        {
            return targetPosition;
        }
    }
}