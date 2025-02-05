using System;
using System.Numerics;

namespace Luna.UI
{
    internal interface ISlidable
    {
        public void OnValueChanged(Action<float> e);

        public float GetValue();

        public void SoftSetValue(float value);

        public void HardSetValue(float value);
    }
}