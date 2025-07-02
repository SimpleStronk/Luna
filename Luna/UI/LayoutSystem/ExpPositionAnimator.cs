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

        /// <summary>
        /// Sets the target position for this PositionAnimator
        /// </summary>
        public void SetPosition(LVector2 position)
        {
            targetPosition = position;
        }

        /// <summary>
        /// Moves this PositionAnimator immediately to the given position
        /// </summary>
        public void ForcePosition(LVector2 position)
        {
            currentPosition = targetPosition = position;
        }

        /// <summary>
        /// Skip animation
        /// </summary>
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